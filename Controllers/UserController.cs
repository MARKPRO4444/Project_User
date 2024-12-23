using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Project_User.Data;
using Project_User.Models;

namespace Project_User.Controllers;

public class UserController : Controller
{
    // объект шифрования
    private readonly IDataProtector dataProtector;

    UserContext userContext;
    public UserController(UserContext userContext, IDataProtectionProvider protectionProvider)
    {
        this.userContext = userContext;

        // указываем ключ для защиты
        dataProtector = protectionProvider.CreateProtector("AuthCookieProtector");
    }

    // метод шифрования данных
    private string ProtectCookieValue(string data)
    {
        return dataProtector.Protect(data);
    }

    // метод расшифрования данных
    private string UnProtectCookieValue(string protectData)
    {
        return dataProtector.Unprotect(protectData);
    }

    // GET: UserController
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Settings()
    {
        SettingsViewModel settingsModel = new SettingsViewModel();

        // если не удается получить значение из cookie auth
        if (!HttpContext.Request.Cookies.TryGetValue("auth", out var protectedCookieValue))
        {
            settingsModel.IsAuthenticated = false;
        }

        try
        {
            // расшифровать
            var cookieValue = UnProtectCookieValue(protectedCookieValue);

            settingsModel.IsAuthenticated = true;
            settingsModel.CurrentUser.Username = cookieValue;
            // HttpContext.Request.Cookies.TryGetValue("role", out var role);
            settingsModel.CurrentUser.Role = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "role").Value;
            settingsModel.Users = userContext.Users.ToList();
        }
        catch
        {
            settingsModel.IsAuthenticated = false;
        }

        return View(settingsModel);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string Username, string password, string role = "customer")
    {
        //DB

        // Можно создавать только 1 user с определенным Username(то есть он уникальный)
        if (userContext.Users.FirstOrDefault(user => user.Username == Username) == null)
        {
            // Hash
            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            userContext.Users.Add(new User()
            {
                Hash = hash,
                Username = Username,
                Role = role
            });

            userContext.SaveChanges();

            Login(Username, password);

            return RedirectToAction("Index");
        }

        return View(model: "This Username is already taken!!");
    }

    [HttpPost]
    public IActionResult SetAdminRole(string Username, SettingsViewModel viewModel)
    {
        // var user = UserRepository.GetUser(Username);
        var user = UserManager.GetUser(Username, userContext);

        if (user != null)
        {
            user.Role = "admin";
            userContext.SaveChanges();
        }

        return RedirectToAction("Settings", viewModel);
    }

    [HttpPost]
    public IActionResult RemoveAdminRole(string username, SettingsViewModel viewModel)
    {
        // var user = UserRepository.GetUser(Username);
        var user = UserManager.GetUser(username, userContext);

        if (user != null)
        {
            user.Role = "customer";
            userContext.SaveChanges();
        }

        return RedirectToAction("Settings", viewModel);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string Username, string password)
    {
        User user = UserManager.GetUser(Username, password, userContext);

        if (user == null)
        {
            return View();
        }

        string cookieValue = user.Username;

        string protectedCookieValue = ProtectCookieValue(cookieValue);

        HttpContext.Response.Cookies.Append("auth", protectedCookieValue, new CookieOptions { SameSite = SameSiteMode.Strict, Secure = false });
        HttpContext.Response.Cookies.Append("role", user.Role, new CookieOptions { SameSite = SameSiteMode.Strict, Secure = false });

        Console.WriteLine("Cookie auth добавлена: " + protectedCookieValue);
        Console.WriteLine("Cookie role добавлена: " + user.Role);

        return RedirectToAction("Index");
    }

    public IActionResult Download()
    {
        // если не удается получить значение из cookie auth
        if (!HttpContext.Request.Cookies.TryGetValue("auth", out var protectedCookieValue))
        {
            return RedirectToAction("Index");
        }

        try
        {
            // расшифровать
            var cookieValue = UnProtectCookieValue(protectedCookieValue);

            return View();
        }
        catch
        {
            return RedirectToAction("Login");
        }
    }
}
