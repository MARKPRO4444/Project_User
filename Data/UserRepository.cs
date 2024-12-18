using System;
using Project_User.Models;

namespace Project_User.Data;

public static class UserRepository
{
    public static List<User> Users = new List<User>();

    public static void AddUser(User user)
    {
        Users.Add(user);
    }

    public static User GetUser(string Username, string password)
    {
        return Users.FirstOrDefault(user => user.Username == Username &&
        BCrypt.Net.BCrypt.Verify(password, user.Hash));
    }

    public static User GetUser(string Username)
    {
        return Users.FirstOrDefault(user => user.Username == Username);
    }

    public static void UpdateUser(User user)
    {
        // search user
        var u = Users.FirstOrDefault(x => x.Username == user.Username);

        // update
        u.Role = user.Role;

        // save
    }
}
