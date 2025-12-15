using System;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class DanhMuc
{
    public int DanhMucId { get; set; }

    public string Ten { get; set; } = null!;

    public int PhanLoaiId { get; set; }

    public virtual PhanLoai PhanLoai { get; set; } = null!;
}
