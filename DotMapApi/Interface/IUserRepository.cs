using DotMapApi.Models;

namespace DotMapApi.Interface;

public interface IUserRepository
{
    IEnumerable<User> GetAllUsers();
    User GetUserById(int id);
    User AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(int id);
}