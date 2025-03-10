using Organizations.Application.Common.Interfaces;

namespace Organizations.Application.Features.Users.Get;

public sealed class GetUserHandler(
    IUserAccessor userAccessor
    ) : IRequestHandler<GetUserRequest, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        //check user exists
        // var user = await _userRepository.GetById(userAccessor.GetUserId(), cancellationToken);

        //create user if not exists
        // if (user == null)
        // {
        //     user = await _userRepository.Create(new User()
        //     {
        //         Id = userAccessor.GetUserId(),
        //         FirstName = userAccessor.GetFirstName(),
        //         LastName = userAccessor.GetLastName()
        //     });
        // }

        //return user

        return new GetUserResponse()
        {
            Id = userAccessor.GetUserId(),
            Firstname = "John",
            Lastname = "Doe"
        };
    }
}