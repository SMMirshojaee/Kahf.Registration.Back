using System;
using System.Collections.Generic;

namespace Registration.API.Entity.Models;

public partial class User
{
    public override int Id { get; set; }

    public override DateTime CreatedDate { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;

    public string Role { get; set; } = null!;
}
