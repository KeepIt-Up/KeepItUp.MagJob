using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Endpoint do tworzenia zaproszenia do organizacji.
/// </summary>
/// <remarks>
/// Tworzy nowe zaproszenie do organizacji dla podanego adresu email.
/// </remarks>
public class CreateInvitationEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
	: Endpoint<CreateInvitationRequest, CreateInvitationResponse>
{
  /// <summary>
  /// Konfiguruje endpoint.
  /// </summary>
  public override void Configure()
  {
	Post("api/organizations/{organizationId}/invitations");
	AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
	Description(b => b
		.WithName("CreateInvitation")
		.Produces(201)
		.ProducesProblem(400)
		.ProducesProblem(401)
		.ProducesProblem(403)
		.ProducesProblem(404)
		.ProducesProblem(500));
	Summary(s =>
	{
	  s.Summary = "Tworzy zaproszenie do organizacji";
	  s.Description = "Tworzy nowe zaproszenie do organizacji dla podanego adresu email";
	  s.ExampleRequest = new CreateInvitationRequest { OrganizationId = Guid.NewGuid(), Email = "example@example.com", RoleId = Guid.NewGuid() };
	  s.ResponseExamples[201] = new CreateInvitationResponse { Id = Guid.NewGuid(), Email = "example@example.com" };
	});
  }

  /// <summary>
  /// Obsługuje żądanie POST /api/organizations/{organizationId}/invitations.
  /// </summary>
  /// <param name="req">Żądanie.</param>
  /// <param name="ct">Token anulowania.</param>
  /// <returns>Odpowiedź zawierająca identyfikator utworzonego zaproszenia.</returns>
  public override async Task HandleAsync(CreateInvitationRequest req, CancellationToken ct)
  {
	var userId = currentUserAccessor.GetRequiredCurrentUserId();

	var command = new CreateInvitationCommand()
	{
	  OrganizationId = req.OrganizationId,
	  Email = req.Email,
	  RoleId = req.RoleId,
	  UserId = userId
	};

	var result = await mediator.Send(command, ct);

	if (result.Status == ResultStatus.NotFound)
	{
	  await SendNotFoundAsync(ct);
	  return;
	}

	if (result.Status == ResultStatus.Forbidden)
	{
	  await SendForbiddenAsync(ct);
	  return;
	}

	if (result.Status == ResultStatus.Error)
	{
	  await SendErrorsAsync(500, ct);
	  return;
	}

	if (result.Status == ResultStatus.Invalid)
	{
	  foreach (var error in result.ValidationErrors)
	  {
		AddError(error.ErrorMessage);
	  }
	  await SendErrorsAsync(400, ct);
	  return;
	}

	Response = new CreateInvitationResponse()
	{
	  Id = result.Value,
	  Email = req.Email
	};

	await SendCreatedAtAsync<GetOrganizationInvitationsEndpoint>(
		new { organizationId = req.OrganizationId },
		Response,
		generateAbsoluteUrl: true,
		cancellation: ct);
  }
}

/// <summary>
/// Żądanie utworzenia zaproszenia do organizacji.
/// </summary>
public class CreateInvitationRequest
{
  /// <summary>
  /// Identyfikator organizacji.
  /// </summary>
  public Guid OrganizationId { get; set; }

  /// <summary>
  /// Adres email osoby zapraszanej.
  /// </summary>
  public string Email { get; set; } = string.Empty;

  /// <summary>
  /// Identyfikator roli, która zostanie przypisana po akceptacji zaproszenia.
  /// </summary>
  public Guid RoleId { get; set; }
}

/// <summary>
/// Odpowiedź zawierająca identyfikator utworzonego zaproszenia.
/// </summary>
public class CreateInvitationResponse
{
  /// <summary>
  /// Identyfikator utworzonego zaproszenia.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Adres email osoby zapraszanej.
  /// </summary>
  public string Email { get; set; } = string.Empty;
}
