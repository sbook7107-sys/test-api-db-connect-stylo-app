using System;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
