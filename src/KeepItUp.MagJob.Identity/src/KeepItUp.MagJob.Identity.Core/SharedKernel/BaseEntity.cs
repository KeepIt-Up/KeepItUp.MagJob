using Ardalis.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.SharedKernel;

/// <summary>
/// Klasa bazowa dla wszystkich encji w systemie.
/// </summary>
public abstract class BaseEntity : EntityBase<Guid>
{
  /// <summary>
  /// Data utworzenia encji.
  /// </summary>
  public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

  /// <summary>
  /// Data ostatniej aktualizacji encji.
  /// </summary>
  public DateTime? UpdatedAt { get; protected set; }

  /// <summary>
  /// Aktualizuje datę ostatniej modyfikacji encji.
  /// </summary>
  protected void Update()
  {
    UpdatedAt = DateTime.UtcNow;
  }
}
