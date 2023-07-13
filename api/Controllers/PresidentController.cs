using api.Models;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

// double result = Math.Round(Math.Sqrt(age));

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresidentController : ControllerBase
{
    #region MongoDb
    private readonly IMongoCollection<President> _collection;
    // Dependency Injection
    public PresidentController(IMongoClient client, IMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<President>("presidents");
    }
    #endregion

    [HttpPost("register")]
    public ActionResult<President> Create(President userInput)
    {
        // check if ANY doc with this NationalCode exists
        bool hasDocs = _collection.AsQueryable().Where<President>(p => 
                p.NationalCode == userInput.NationalCode).Any();

        if(hasDocs)
            return BadRequest($"A president with National Code {userInput.NationalCode} is already registered.");

        President president = new President(
            Id: null,
            NationalCode: userInput.NationalCode.Trim(),
            FirstName: userInput.FirstName.Trim(),
            LastName: userInput.LastName.Trim(),
            Age: userInput.Age,
            Education: userInput.Education.Trim(),
            Email: userInput.Email?.ToLower().Trim()
        );

        _collection.InsertOne(president);

        return president;
    }

    [HttpGet("get-all")]
    public ActionResult<IEnumerable<President>> GetAll()
    {
        List<President> presidents = _collection.Find<President>(new BsonDocument()).ToList();

        if(!presidents.Any())
            return NoContent();

        return presidents;
    }
}
