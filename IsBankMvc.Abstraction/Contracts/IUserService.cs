using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Models.User;
using IsBankMvc.Abstraction.Models;
using IsBankMvc.Abstraction.Types;

namespace IsBankMvc.Abstraction.Contracts
{
    public interface IUserService
    {
        Task<OperationResult<UserVM>> GetUser(Guid userId);
        Task<OperationResult<UserVM>> Login(LoginRequest request);
        Task<OperationResult<UserVM>> Register(RegisterRequest request, UserType userType = UserType.Agent);
        Task<OperationResult<PaginatedResponseVM<UserMiniVM>>> Users(PaginatedRequestVM request);
        Task<OperationResult<bool>> UserUpdate(Guid userId, UserUpdateRequest request);
        Task<OperationResult<bool>> ResetPassword(Guid userId, ResetPasswordRequest request);
        Task<OperationResult<bool>> UserDelete(Guid userId, Guid targetId);
        Task<OperationResult<ListItemVM[]>> UsersList();
    }
}
