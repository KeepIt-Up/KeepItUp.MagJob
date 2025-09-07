# System Czatów - Implementacja

## Przegląd

Zaimplementowano kompletny system czatów w Angularze z obsługą WebSocketów dla komunikacji w czasie rzeczywistym. System umożliwia tworzenie czatów, wysyłanie wiadomości i zarządzanie członkami czatu.

## Komponenty

### 1. Główny Komponent (`ChatsComponent`)
- **Lokalizacja**: `src/app/pages/organization/chats/chats.component.ts`
- **Funkcjonalność**: Główny kontener dla systemu czatów
- **Funkcje**:
  - Ładowanie listy czatów dla organizacji
  - Zarządzanie wybranym czatem
  - Obsługa modalu tworzenia czatu

### 2. Lista Czatów (`ChatListComponent`)
- **Lokalizacja**: `src/app/pages/organization/chats/components/chat-list/`
- **Funkcjonalność**: Wyświetlanie listy dostępnych czatów
- **Funkcje**:
  - Lista czatów z podglądem ostatniej wiadomości
  - Informacje o liczbie członków
  - Formatowanie dat (dziś, wczoraj, etc.)

### 3. Wiadomości Czatu (`ChatMessagesComponent`)
- **Lokalizacja**: `src/app/pages/organization/chats/components/chat-messages/`
- **Funkcjonalność**: Wyświetlanie i wysyłanie wiadomości
- **Funkcje**:
  - Real-time wiadomości przez WebSocket
  - Wysyłanie wiadomości
  - Status wiadomości (dostarczona/przeczytana)
  - Auto-scroll do najnowszych wiadomości

### 4. Modal Tworzenia Czatu (`ChatCreateModalComponent`)
- **Lokalizacja**: `src/app/pages/organization/chats/components/chat-create-modal/`
- **Funkcjonalność**: Tworzenie nowego czatu
- **Funkcje**:
  - Formularz z walidacją
  - Wybór członków organizacji
  - Walidacja tytułu i członków

## Serwisy

### 1. ChatService
- **Lokalizacja**: `src/app/features/chats/services/chat.service.ts`
- **Funkcjonalność**: Główny serwis zarządzający czatami
- **Funkcje**:
  - HTTP API dla CRUD operacji
  - Integracja z WebSocket
  - Zarządzanie stanem lokalnym
  - Łączenie z czatami

### 2. WebSocketService
- **Lokalizacja**: `src/app/features/chats/services/websocket.service.ts`
- **Funkcjonalność**: Obsługa komunikacji WebSocket
- **Funkcje**:
  - Połączenie z serwerem WebSocket
  - Subskrypcje na tematy
  - Wysyłanie wiadomości
  - Zarządzanie połączeniem

## Modele Danych

### 1. Chat
```typescript
interface Chat {
  id: string;
  title: string;
  dateOfCreation: Date;
  organizationId: string;
  chatMembers: ChatMember[];
  lastMessage?: ChatMessage;
}
```

### 2. ChatMessage
```typescript
interface ChatMessage {
  id: string;
  content: string;
  dateOfCreation: Date;
  viewedBy: string[];
  attachment?: string;
  firstAndLastName: string;
  chatMember: ChatMember;
  chat: ChatInfo;
}
```

### 3. ChatMember
```typescript
interface ChatMember {
  id: string;
  nickname?: string;
  memberId: string;
  isInvitationAccepted: boolean;
  isAdmin: boolean;
  member?: MemberInfo;
}
```

## Routing

Dodano routing dla czatów w organizacji:
```typescript
{
  path: 'chats',
  loadComponent: () => import('./app/pages/organization/chats/chats.component').then(m => m.ChatsComponent)
}
```

## WebSocket Endpoints

### 1. Subskrypcje
- `/topic/chat/{chatId}` - Wiadomości dla konkretnego czatu
- `/topic/chat` - Ogólne wiadomości czatu

### 2. Wysyłanie
- `/chat/{chatId}/sendMessage` - Wysyłanie wiadomości
- `/chat/{chatId}/messageViewed` - Oznaczenie wiadomości jako przeczytanej

## API Endpoints

### 1. Czaty
- `GET /api/chat/organizations/{organizationId}/chats` - Lista czatów organizacji
- `POST /api/chat/chats` - Tworzenie nowego czatu
- `GET /api/chat/chats/{chatId}/chat-messages` - Wiadomości czatu

### 2. Wiadomości
- `POST /api/chat/chats/{chatId}/messages` - Wysyłanie wiadomości
- `PATCH /api/chat/messages/{id}` - Aktualizacja wiadomości

## Stylowanie

Wykorzystano Tailwind CSS dla nowoczesnego i responsywnego designu:
- Kolory: Niebieski (#3B82F6) jako główny kolor
- Animacje: Fade-in dla wiadomości, hover effects
- Responsywność: Mobile-first approach
- Custom scrollbars dla lepszego UX

## Funkcjonalności

### ✅ Zaimplementowane
- [x] Lista czatów organizacji
- [x] Tworzenie nowego czatu
- [x] Wybór członków czatu
- [x] Wyświetlanie wiadomości
- [x] Wysyłanie wiadomości
- [x] Real-time komunikacja przez WebSocket
- [x] Responsywny design z Tailwind CSS
- [x] Walidacja formularzy
- [x] Loading states i error handling
- [x] Auto-scroll do najnowszych wiadomości

### 🔄 Do Implementacji
- [ ] Edycja nazwy czatu
- [ ] Usuwanie czatu
- [ ] Zarządzanie uprawnieniami członków
- [ ] Powiadomienia o nowych wiadomościach
- [ ] Załączniki do wiadomości
- [ ] Wyszukiwanie w wiadomościach
- [ ] Emoji reactions
- [ ] Typing indicators

## Uruchomienie

1. Upewnij się, że backend ChatAndNotification.API jest uruchomiony
2. Sprawdź konfigurację WebSocket w `gatewayConfiguration.json`
3. Uruchom aplikację Angular: `npm run local`
4. Przejdź do organizacji i kliknij "Chats" w sidebar

## Konfiguracja

### Environment Variables
```typescript
// src/environments/environment.ts
export const environment = {
  apiUrl: 'http://localhost:5000', // API Gateway URL
  // ... inne zmienne
};
```

### WebSocket URL
WebSocket automatycznie konwertuje HTTP URL na WS:
```typescript
private readonly wsUrl = `${environment.apiUrl.replace('http', 'ws')}/ws`;
```

## Troubleshooting

### Problem: WebSocket nie łączy się
- Sprawdź czy backend jest uruchomiony
- Sprawdź konfigurację w `gatewayConfiguration.json`
- Sprawdź logi w konsoli przeglądarki

### Problem: Wiadomości nie są wysyłane
- Sprawdź połączenie WebSocket
- Sprawdź logi w konsoli
- Sprawdź czy użytkownik ma uprawnienia do czatu

### Problem: Czaty się nie ładują
- Sprawdź API endpoint
- Sprawdź autoryzację
- Sprawdź logi w Network tab

## Rozwój

### Dodawanie nowych funkcjonalności
1. Utwórz nowy komponent w odpowiednim folderze
2. Dodaj routing jeśli potrzebny
3. Zaktualizuj modele danych
4. Dodaj odpowiednie serwisy
5. Zaktualizuj WebSocket handling

### Testowanie
- Użyj Angular DevTools
- Sprawdź WebSocket connection w Network tab
- Testuj na różnych urządzeniach
- Sprawdź responsywność

## Wsparcie

W przypadku problemów:
1. Sprawdź logi w konsoli
2. Sprawdź Network tab w DevTools
3. Sprawdź WebSocket connection status
4. Sprawdź czy wszystkie serwisy są uruchomione
