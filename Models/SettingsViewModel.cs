using System;
using Project_User.Data;

namespace Project_User.Models;

public class SettingsViewModel
{
    public bool IsAuthenticated { get; set; }

    public User CurrentUser { get; set; } = new User();

    public List<User> Users { get; set; }
    // public string Name { get; set; }
}
