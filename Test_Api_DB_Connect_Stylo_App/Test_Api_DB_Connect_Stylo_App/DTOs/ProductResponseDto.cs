namespace Test_Api_DB_Connect_Stylo_App.DTOs
{
    public class ProductResponseDto
    {
        public int SanPhamID { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
    }
}
