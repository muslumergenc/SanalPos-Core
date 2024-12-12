using IsBankMvc.Abstraction.Contracts;
using IsBankMvc.Abstraction.Models;
using IsBankMvc.Abstraction.Models.User;
using IsBankMvc.Abstraction.Types;
using IsBankMvc.DataAccess.Contexts;
using IsBankMvc.DataAccess.Contracts;
using IsBankMvc.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IsBankMvc.DataAccess.Repositories
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILoggerService _loggerService;
        public UserRepository(ApplicationDbContext dbContext, ILoggerService loggerService)
        {
            _dbContext = dbContext;
            _loggerService = loggerService;
        }
        public async Task<bool> CreateUser(UserVM user)
        {
            try
            {
                await _dbContext.Users.AddAsync(User.FromVM(user));
                var affected = await _dbContext.SaveChangesAsync();
                return affected > 0;
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.CreateUser", e);
                return false;
            }
        }
        public async Task<bool> FailedLoginAttempt(Guid userId, bool block)
        {
            try
            {
                DateTime? blockedAt = block ? DateTime.UtcNow : null;
                var affected = await _dbContext.Users
                    .Where(i => i.Id == userId)
                    .ExecuteUpdateAsync(i =>
                        i.SetProperty(p => p.BlockedAt, p => blockedAt)
                            .SetProperty(p => p.FailedAttempt, p => p.FailedAttempt + 1)
                    );
                return affected > 0;
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.FailedLoginAttempt", e);
                return false;
            }
        }
        public async Task<UserVM?> GetUser(Guid id)
        {
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => !i.DeletedAt.HasValue && i.Id == id);
                return user?.ToVM();
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.GetUser", e);
                return null;
            }
        }
        public async Task<UserVM?> GetUser(string username, string phone = "", bool skipDeleted = true)
        {
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i =>
                        (!skipDeleted || !i.DeletedAt.HasValue) &&
                        (i.Username == username || (!string.IsNullOrEmpty(i.Phone) && i.Phone == phone)));
                return user?.ToVM();
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.GetUser", e);
                return null;
            }
        }
        public async Task<OperationResult<bool>> ResetPassword(Guid userId, string hash, string salt)
        {
            try
            {
                var affected = await _dbContext.Users
                    .Where(i => i.Id == userId && !i.DeletedAt.HasValue)
                    .ExecuteUpdateAsync(i => i
                        .SetProperty(p => p.Salt, salt)
                        .SetProperty(p => p.Hash, hash)
                    );

                if (affected > 0) return OperationResult<bool>.Success(true);
                return OperationResult<bool>.Failed();
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.ResetPassword", e);
                return OperationResult<bool>.Failed();
            }
        }
        public async Task<OperationResult<bool>> UserDelete(Guid userId, Guid targetId)
        {
            var updated = await _dbContext.Users.Where(i => i.Id == targetId).ExecuteUpdateAsync(p =>p.SetProperty(x => x.DeletedAt, c => DateTime.UtcNow)
         );
            if (updated > 0) return OperationResult<bool>.Success();
            return OperationResult<bool>.Failed();
        }
        public async Task<OperationResult<PaginatedResponseVM<UserMiniVM>>> Users(PaginatedRequestVM request)
        {
            try
            {
                var query = _dbContext.Users
                    .Where(u => !u.DeletedAt.HasValue &&
                                (
                                    string.IsNullOrEmpty(request.Query) ||
                                    (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(request.Query)) ||
                                    (!string.IsNullOrEmpty(u.Surname) && u.Surname.Contains(request.Query)) ||
                                    (!string.IsNullOrEmpty(u.Username) && u.Username.Contains(request.Query)) ||
                                    (!string.IsNullOrEmpty(u.Phone) && u.Phone.Contains(request.Query))
                                )
                    ).AsNoTracking();

                var count = await query.CountAsync();
                var page = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToArrayAsync();

                return OperationResult<PaginatedResponseVM<UserMiniVM>>.Success(new PaginatedResponseVM<UserMiniVM>
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = count,
                    Items = page.Select(p => p.ToMiniVM()).ToArray()
                });
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.Users", e);
                return OperationResult<PaginatedResponseVM<UserMiniVM>>.Failed();
            }
        }

        public async Task<OperationResult<ListItemVM[]>> UsersList()
        {
            try
            {
                var users = await _dbContext.Users
                    .Where(i => !i.DeletedAt.HasValue)
                    .OrderBy(i => i.Username)
                    .Select(i => new ListItemVM() { Text = i.Username, Value = i.Id.ToString() })
                    .ToArrayAsync();

                return OperationResult<ListItemVM[]>.Success(users);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.UsersList", e);
                return OperationResult<ListItemVM[]>.Failed();
            }
        }

        public async Task<OperationResult<bool>> UserUpdate(Guid userId, UserUpdateRequest model)
        {
            try
            {
                var user = await _dbContext.Users.SingleOrDefaultAsync(i => i.Id == model.Id);
                if (user == null) return OperationResult<bool>.NotFound();
                model.Email = model.Email.Trim().ToLower();
                if (!string.IsNullOrEmpty(model.Email) && user.Username != model.Email)
                {
                    var exists = await _dbContext.Users.AnyAsync(i => i.Username == model.Email);
                    if (exists) return OperationResult<bool>.Duplicate();
                    user.Username = model.Email;
                }

                user.Name = model.Name;
                user.Nationality = model.Nationality;
                user.Address = model.Address;
                user.Phone = model.Phone;
                user.Surname = model.Surname;
                user.BirthDate = model.BirthDate;
                user.UserType = model.UserType;
                var affected = await _dbContext.SaveChangesAsync();
                return OperationResult<bool>.Success(affected > 0);
            }
            catch (Exception e)
            {
                await _loggerService.Error(e.Message, "UserRepository.UserUpdate", e);
                return OperationResult<bool>.Failed();
            }
        }
        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
