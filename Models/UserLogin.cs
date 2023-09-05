using System.ComponentModel.DataAnnotations;

namespace UserApi.Models;

public class UserLogin
{
    [Required]
    [EmailAddress]
    public string EmailConfirmed { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;
}