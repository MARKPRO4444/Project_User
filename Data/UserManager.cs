using System;
using Project_User.Models;

namespace Project_User.Data;

public static class UserManager
{
    public static void AddUser(User user, UserContext context)
    {
        context.Users.Add(user);

        context.SaveChanges();
    }

    public static User GetUser(string Username, string password, UserContext context)
    {
        User user = context.Users.FirstOrDefault(user => user.Username == Username);
        if(user != null && BCrypt.Net.BCrypt.Verify(password, user.Hash))
        {
            return user;
        }

        return null;
    }

    public static User GetUser(string Username, UserContext context)
    {
        return context.Users.FirstOrDefault(user => user.Username == Username);
    }

    public static void UpdateUser(User user, UserContext context)
    {
        // search user
        var u = context.Users.FirstOrDefault(x => x.Username == user.Username);

        // update
        u.Role = user.Role;

        // save
        context.SaveChanges();
    }
}
