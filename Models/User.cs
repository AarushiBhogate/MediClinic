using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public int? RoleReferenceId { get; set; }

    public string? Status { get; set; }
}
