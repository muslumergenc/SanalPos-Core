using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Enums;
using IsBankMvc.Abstraction.Implementation;
using IsBankMvc.Abstraction.Models;
using IsBankMvc.Abstraction.Models.User;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.Business.Helpers;
using IsBankMvc.DataAccess.Contracts;

namespace IsBankMvc.Business.Implementation
{
    public class UserService : IUserService
    {
        private const int MaxFailedAttempt = 50;
        private readonly IJsonService _jsonService;
        private readonly ILoggerService _loggerService;
        private readonly IUserRepository _repository;
        public UserService(IJsonService jsonService, ILoggerService loggerService, IUserRepository repository)
        {
            _jsonService = jsonService;
            _loggerService = loggerService;
            _repository = repository;
        }
        public async Task<OperationResult<UserVM?>> GetUser(Guid userId)
        {
            try
            {
                var user = await _repository.GetUser(userId);
                if (user == null)
                    return OperationResult<UserVM?>.NotFound();

                return OperationResult<UserVM?>.Success(user);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserService.Profile", e);
                return OperationResult<UserVM?>.Failed();
            }
        }
        public async Task<OperationResult<UserVM?>> Login(LoginRequest request)
        {
            try
            {
                var user = await _repository.GetUser(request.Username);
                if (user == null)
                    return OperationResult<UserVM?>.NotFound();

                if (user.BlockedAt.HasValue)
                    return OperationResult<UserVM?>.Rejected(ErrorMessages.AccountIsBlocked);

                var verified = CryptographyHelper.Verify(request.Password, user.Hash, user.Salt);
                if (!verified)
                {
                    user.FailedAttempt++;
                    var block = user.FailedAttempt == MaxFailedAttempt;
                    await _repository.FailedLoginAttempt(user.Id, block);
                    return OperationResult<UserVM?>.Rejected(ErrorMessages.InvalidUsernameOrPassword);
                }

                return OperationResult<UserVM?>.Success(user);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserService.Login", e);
                return OperationResult<UserVM?>.Failed();
            }
        }
        public async Task<OperationResult<UserVM?>> Register(RegisterRequest request, UserType userType = UserType.Agent)
        {
            try
            {
                var user = await _repository.GetUser(request.Username, skipDeleted: false);
                if (user != null)
                    return OperationResult<UserVM?>.Duplicate();

                var salt = CryptographyHelper.GenerateSalt();
                var hash = CryptographyHelper.Hash(request.Password, salt);
                user = new UserVM
                {
                    Id = IncrementalGuid.NewId(),
                    Username = request.Username,
                    CreatedAt = DateTime.UtcNow,
                    Hash = hash,
                    Salt = salt,
                    UserType = userType,
                    Name = request.FirstName,
                    Surname = request.LastName
                };

                var saved = await _repository.CreateUser(user);
                if (!saved) return OperationResult<UserVM?>.Failed();

                return OperationResult<UserVM?>.Success(user);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserService.Register", e);
                if (e.Message.Contains("unique constraint", StringComparison.InvariantCultureIgnoreCase))
                    return OperationResult<UserVM?>.Duplicate();

                return OperationResult<UserVM?>.Failed();
            }
        }
        public async Task<OperationResult<bool>> ResetPassword(Guid userId, ResetPasswordRequest request)
        {
            var user = await _repository.GetUser(userId);
            if (user == null || user.UserType < UserType.Admin)
                return OperationResult<bool>.Rejected();

            var salt = CryptographyHelper.GenerateSalt();
            var hash = CryptographyHelper.Hash(request.Password, salt);
            return await _repository.ResetPassword(request.UserId, hash, salt);
        }
        public async Task<OperationResult<bool>> UserDelete(Guid userId, Guid targetId)
        {
            try
            {
                // TODO: add more logic here...
                return await _repository.UserDelete(userId, targetId);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserService.UserDelete", e);
                return OperationResult<bool>.Failed();
            }
        }
        public Task<OperationResult<PaginatedResponseVM<UserMiniVM>>> Users(PaginatedRequestVM request)
        {
            return _repository.Users(request);
        }
        public Task<OperationResult<ListItemVM[]>> UsersList()
        {
            return _repository.UsersList();
        }
        public async Task<OperationResult<bool>> UserUpdate(Guid userId, UserUpdateRequest request)
        {
            var user = await _repository.GetUser(userId);
            if (user == null || user.UserType < UserType.Admin)
                return OperationResult<bool>.Rejected();
            return await _repository.UserUpdate(userId, request);
        }
    }
}
