using System;
using System.Collections.Generic;
using Test_Api_DB_Connect_Stylo_App.Entities;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class DonHang
{
    public int DonHangId { get; set; }

    public int KhachHangId { get; set; }

    public string TrangThai { get; set; } = null!;

    public string KenhBan { get; set; } = null!;

    public decimal TongTienHang { get; set; }

    public decimal TongGiamGia { get; set; }

    public decimal Thue { get; set; }

    public decimal PhiVanChuyen { get; set; }

    public decimal TongThanhToan { get; set; }

    public DateTime NgayDat { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; } = new List<DonHangChiTiet>();

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual ICollection<VanDon> VanDons { get; set; } = new List<VanDon>();
}
