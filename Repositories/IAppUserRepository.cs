using MovieApi.Models;
using MovieApi.Models.System;

public interface IAppUserRepository
{
    Task<IEnumerable<AppUser>> GetAllUsers();
    Task<AppUser> GetUserById(long id);
    Task<int> CreateUser(RegisterRequest user);
    Task<int> UpdateUser(long id, AppUser user);
    Task<int> DeleteUser(long id);
    Task<AppUser> GetUserByUsernameAndPassword(string username, string password);
    Task<AppUser> GetUserByUsername(string username);
}

