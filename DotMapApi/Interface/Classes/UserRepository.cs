using System.Collections.Generic;
using System.Linq;
using DotMapApi.Interface;
using DotMapApi.Models;
using Microsoft.EntityFrameworkCore;
using DotMapApi;

namespace DotMapApi.Interface.Classes;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    public User GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public User AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public void UpdateUser(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
