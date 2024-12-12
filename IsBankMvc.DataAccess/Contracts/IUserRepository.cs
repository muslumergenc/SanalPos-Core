using IsBankMvc.Abstraction.Models;
using IsBankMvc.Abstraction.Models.User;
using IsBankMvc.Abstraction.Types;

namespace IsBankMvc.DataAccess.Contracts
{
    public interface IUserRepository
    {
        Task<UserVM?> GetUser(Guid id);
        Task<UserVM?> GetUser(string username, string phone = "", bool skipDeleted = true);
        Task<bool> CreateUser(UserVM user);
        Task<bool> FailedLoginAttempt(Guid userId, bool block);
        Task<OperationResult<PaginatedResponseVM<UserMiniVM>>> Users(PaginatedRequestVM request);
        Task<OperationResult<bool>> ResetPassword(Guid userId, string hash, string salt);
        Task<OperationResult<bool>> UserUpdate(Guid userId, UserUpdateRequest request);
        Task<OperationResult<ListItemVM[]>> UsersList();
        Task<OperationResult<bool>> UserDelete(Guid userId, Guid targetId);
    }
}
