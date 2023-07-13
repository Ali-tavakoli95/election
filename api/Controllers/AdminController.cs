using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
     #region MongoDb
    private readonly IMongoCollection<Admin> _collection;
    // Dependency Injection
    public AdminController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Admin>("admins");
    }
    #endregion

    [HttpPost("register")]
    public ActionResult<Admin> Create(Admin adminInput)
    {
        Admin admin = new Admin(
          Id: null,
          Email: adminInput.Email.ToLower().Trim(),
          Password: adminInput.Password,
          ConfirmPassword: adminInput.ConfirmPassword
        );

        _collection.InsertOne(admin);

        return admin;
    }

    [HttpPost("login")]
    public ActionResult<Admin> Login(Admin adminInput)
    {
        Admin admin = _collection.Find<Admin>(admin => 
                admin.Email == adminInput.Email
                && admin.Password == adminInput.Password
            ).FirstOrDefault();

        if(admin is null)
        return BadRequest("Bad username or password");

        return admin;
    }
}
