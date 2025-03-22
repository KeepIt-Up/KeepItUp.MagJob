# System Kolejek Wiadomości i Powiadomień - MagJob

Ten dokument opisuje architekturę i implementację systemu kolejek wiadomości oraz powiadomień w projekcie MagJob.

## Kolejki Wiadomości

### Technologia

- **RabbitMQ**: Jako broker wiadomości dla komunikacji asynchronicznej między mikrousługami
- **Implementacja**: Każda mikrousługa publikuje i konsumuje wiadomości z RabbitMQ

### Architektura Kolejek

![Architektura Kolejek](../images/messaging-architecture.png)

#### Wymiany (Exchanges)

- **magjob.events**: Wymiana typu `topic` dla zdarzeń domenowych
- **magjob.commands**: Wymiana typu `direct` dla komend

#### Kolejki (Queues)

- **organizations.events**: Kolejka dla zdarzeń związanych z organizacjami
- **schedules.events**: Kolejka dla zdarzeń związanych z grafikami
- **workevidence.events**: Kolejka dla zdarzeń związanych z ewidencją czasu pracy
- **notifications.events**: Kolejka dla zdarzeń wymagających powiadomień

### Typy Wiadomości

#### Zdarzenia Domenowe

- **UserCreated**: Utworzenie nowego użytkownika
- **UserUpdated**: Aktualizacja danych użytkownika
- **OrganizationCreated**: Utworzenie nowej organizacji
- **OrganizationUpdated**: Aktualizacja danych organizacji
- **MemberAdded**: Dodanie członka do organizacji
- **MemberRemoved**: Usunięcie członka z organizacji
- **ScheduleCreated**: Utworzenie nowego grafiku
- **ScheduleUpdated**: Aktualizacja grafiku
- **SchedulePublished**: Opublikowanie grafiku
- **TimeEntryRecorded**: Zarejestrowanie czasu pracy
- **TimeEntryApproved**: Zatwierdzenie wpisu czasu pracy
- **AbsenceRequested**: Zgłoszenie nieobecności
- **AbsenceApproved**: Zatwierdzenie nieobecności

#### Komendy

- **SendNotification**: Wysłanie powiadomienia do użytkownika
- **GenerateReport**: Wygenerowanie raportu
- **SyncCalendar**: Synchronizacja z Google Calendar

### Implementacja Publikowania Zdarzeń

```csharp
// W warstwie infrastruktury
public class RabbitMQEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    
    public RabbitMQEventBus(IOptions<RabbitMQOptions> options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            VirtualHost = options.Value.VirtualHost
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _exchangeName = "magjob.events";
        
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
    }
    
    public Task PublishAsync<TEvent>(TEvent @event, string routingKey) where TEvent : class
    {
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);
        
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.MessageId = Guid.NewGuid().ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Headers = new Dictionary<string, object>
        {
            { "event_type", @event.GetType().Name }
        };
        
        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body);
        
        return Task.CompletedTask;
    }
}
```

### Implementacja Konsumowania Zdarzeń

```csharp
// W warstwie infrastruktury
public class RabbitMQEventConsumer : IEventConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly IServiceProvider _serviceProvider;
    
    public RabbitMQEventConsumer(IOptions<RabbitMQOptions> options, IServiceProvider serviceProvider)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            VirtualHost = options.Value.VirtualHost
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _exchangeName = "magjob.events";
        _queueName = options.Value.QueueName;
        _serviceProvider = serviceProvider;
        
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
    }
    
    public void Subscribe<TEvent>(string routingKey) where TEvent : class
    {
        _channel.QueueBind(_queueName, _exchangeName, routingKey);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<TEvent>(message);
            
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
            
            try
            {
                await handler.HandleAsync(@event);
                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception)
            {
                _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };
        
        _channel.BasicConsume(_queueName, autoAck: false, consumer: consumer);
    }
    
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
```

## System Powiadomień

### Typy Powiadomień

- **Email**: Powiadomienia wysyłane na adres email użytkownika
- **Push**: Powiadomienia push wysyłane do przeglądarki
- **In-app**: Powiadomienia wyświetlane w aplikacji

### Architektura Systemu Powiadomień

![Architektura Powiadomień](../images/notifications-architecture.png)

#### Komponenty

- **NotificationService**: Serwis odpowiedzialny za zarządzanie powiadomieniami
- **EmailSender**: Komponent do wysyłania powiadomień email
- **PushNotificationSender**: Komponent do wysyłania powiadomień push
- **NotificationHub**: Hub SignalR do wysyłania powiadomień in-app w czasie rzeczywistym

### Zdarzenia Generujące Powiadomienia

- **MemberAdded**: Powiadomienie dla użytkownika o dodaniu do organizacji
- **SchedulePublished**: Powiadomienie dla pracowników o opublikowaniu nowego grafiku
- **ScheduleEntryCreated**: Powiadomienie dla pracownika o przypisaniu do grafiku
- **TimeEntryApproved/Rejected**: Powiadomienie o zatwierdzeniu/odrzuceniu wpisu czasu pracy
- **AbsenceApproved/Rejected**: Powiadomienie o zatwierdzeniu/odrzuceniu nieobecności
- **ChatMessageReceived**: Powiadomienie o nowej wiadomości w czacie

### Implementacja Serwisu Powiadomień

```csharp
public class NotificationService : INotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IPushNotificationSender _pushSender;
    private readonly INotificationHub _notificationHub;
    private readonly INotificationRepository _notificationRepository;
    
    public NotificationService(
        IEmailSender emailSender,
        IPushNotificationSender pushSender,
        INotificationHub notificationHub,
        INotificationRepository notificationRepository)
    {
        _emailSender = emailSender;
        _pushSender = pushSender;
        _notificationHub = notificationHub;
        _notificationRepository = notificationRepository;
    }
    
    public async Task SendNotificationAsync(Notification notification)
    {
        // Zapisz powiadomienie w bazie danych
        await _notificationRepository.AddAsync(notification);
        
        // Wyślij powiadomienie in-app
        await _notificationHub.SendNotificationAsync(notification.UserId, notification);
        
        // Wyślij powiadomienie email, jeśli wymagane
        if (notification.SendEmail)
        {
            await _emailSender.SendEmailAsync(
                notification.UserEmail,
                notification.Title,
                notification.Body);
        }
        
        // Wyślij powiadomienie push, jeśli wymagane
        if (notification.SendPush)
        {
            await _pushSender.SendPushAsync(
                notification.UserId,
                notification.Title,
                notification.Body,
                notification.Data);
        }
    }
    
    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int page, int pageSize)
    {
        return await _notificationRepository.GetByUserIdAsync(userId, page, pageSize);
    }
    
    public async Task MarkAsReadAsync(Guid notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }
}
```

### Implementacja Hub SignalR

```csharp
public class NotificationHub : Hub, INotificationHub
{
    private static readonly ConcurrentDictionary<Guid, string> _userConnections = new();
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            _userConnections.AddOrUpdate(userGuid, Context.ConnectionId, (_, _) => Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            _userConnections.TryRemove(userGuid, out _);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendNotificationAsync(Guid userId, Notification notification)
    {
        if (_userConnections.TryGetValue(userId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
        }
    }
}
```

## System Czatów Grupowych

### Funkcjonalności

- **Czaty grupowe**: Możliwość tworzenia czatów dla organizacji lub zespołów
- **Wiadomości bezpośrednie**: Możliwość wysyłania wiadomości bezpośrednich między użytkownikami
- **Powiadomienia**: Powiadomienia o nowych wiadomościach
- **Historia wiadomości**: Przechowywanie historii wiadomości

### Implementacja

- **ChatHub**: Hub SignalR do komunikacji w czasie rzeczywistym
- **ChatService**: Serwis do zarządzania czatami i wiadomościami
- **ChatRepository**: Repozytorium do przechowywania wiadomości

### Integracja z Google Calendar

- **Synchronizacja dwukierunkowa**: Synchronizacja grafików z Google Calendar
- **Implementacja**: Wykorzystanie Google Calendar API do synchronizacji
- **Autoryzacja**: OAuth 2.0 do autoryzacji dostępu do kalendarza użytkownika 
