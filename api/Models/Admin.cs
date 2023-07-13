using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Models;

public record Admin(
    [property: BsonId, BsonRepresentation(BsonType.ObjectId)] string? Id, //hamishe sabet
    string Email,
    string Password,
    string? ConfirmPassword
);