using System;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class DonHangChiTiet
{
    public int DonHangId { get; set; }

    public int BienTheId { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal GiamGia { get; set; }

    public decimal Thue { get; set; }

    public virtual SanPhamBienThe BienThe { get; set; } = null!;

    public virtual DonHang DonHang { get; set; } = null!;
}
