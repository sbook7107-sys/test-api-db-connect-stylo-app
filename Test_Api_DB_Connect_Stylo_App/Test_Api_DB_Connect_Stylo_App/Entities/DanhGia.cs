using System;
using System.Collections.Generic;
using Test_Api_DB_Connect_Stylo_App.Entities;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class DanhGia
{
    public long ReviewId { get; set; }

    public int SanPhamId { get; set; }

    public int? KhachHangId { get; set; }

    public string? ExternalUserId { get; set; }

    public decimal Rating { get; set; }

    public string? ReviewTitle { get; set; }

    public string? ReviewText { get; set; }

    public bool VerifiedPurchase { get; set; }

    public string ReviewSource { get; set; } = null!;

    public DateTime ReviewTime { get; set; }

    public virtual KhachHang? KhachHang { get; set; }

    public virtual SanPham SanPham { get; set; } = null!;
}
