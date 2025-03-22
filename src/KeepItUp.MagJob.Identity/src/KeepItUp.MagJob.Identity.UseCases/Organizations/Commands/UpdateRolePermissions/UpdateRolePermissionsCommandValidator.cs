using FluentValidation;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;

/// <summary>
/// Walidator dla komendy <see cref="UpdateRolePermissionsCommand"/>.
/// </summary>
public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateRolePermissionsCommandValidator"/>.
    /// </summary>
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Identyfikator organizacji jest wymagany.");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Identyfikator roli jest wymagany.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");

        RuleFor(x => x.Permissions)
            .NotNull()
            .WithMessage("Lista uprawnień nie może być null.");
    }
} 
