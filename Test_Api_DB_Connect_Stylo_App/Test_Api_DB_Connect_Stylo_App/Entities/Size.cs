using System;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string KyHieu { get; set; } = null!;

    public int ThuTu { get; set; }

    public virtual ICollection<SanPhamBienThe> SanPhamBienThes { get; set; } = new List<SanPhamBienThe>();
}
