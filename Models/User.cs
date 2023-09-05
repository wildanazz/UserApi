using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string EmailConfirmed { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }
}