using System.ComponentModel.DataAnnotations;

namespace UserApi.Models;

public class UserRegister
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}