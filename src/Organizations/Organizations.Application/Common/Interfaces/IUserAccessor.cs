namespace Organizations.Application.Common.Interfaces;

public interface IUserAccessor
{
    Guid GetUserId();
    string GetFirstName();
    string GetLastName();
    string GetUserEmail();
    string GetUserName();
}