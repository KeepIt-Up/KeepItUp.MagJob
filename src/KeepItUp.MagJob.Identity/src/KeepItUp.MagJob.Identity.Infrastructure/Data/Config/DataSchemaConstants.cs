namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public static class DataSchemaConstants
{
  // Długości pól
  public const int DEFAULT_NAME_LENGTH = 100;
  public const int DEFAULT_DESCRIPTION_LENGTH = 500;
  public const int DEFAULT_EMAIL_LENGTH = 255;
  public const int DEFAULT_TOKEN_LENGTH = 64;
  public const int DEFAULT_COLOR_LENGTH = 10;
  public const int DEFAULT_EXTERNAL_ID_LENGTH = 64;
  public const int DEFAULT_PHONE_NUMBER_LENGTH = 20;
  public const int DEFAULT_ADDRESS_LENGTH = 255;
  public const int DEFAULT_PROFILE_IMAGE_LENGTH = 255;

  // Nazwy schematów
  public const string IDENTITY_SCHEMA = "identity";

  // Nazwy tabel
  public const string USERS_TABLE = "Users";
  public const string ORGANIZATIONS_TABLE = "Organizations";
  public const string MEMBERS_TABLE = "Members";
  public const string ROLES_TABLE = "Roles";
  public const string PERMISSIONS_TABLE = "Permissions";
  public const string INVITATIONS_TABLE = "Invitations";
  public const string MEMBER_ROLES_TABLE = "MemberRoles";
  public const string ROLE_PERMISSIONS_TABLE = "RolePermissions";
}
