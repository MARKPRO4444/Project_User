using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace Project_User.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Hash { get; set; }
    public string Role { get; set; }
}
