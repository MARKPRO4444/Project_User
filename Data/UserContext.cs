using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Project_User.Models;

namespace Project_User.Data;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options) { }
}
