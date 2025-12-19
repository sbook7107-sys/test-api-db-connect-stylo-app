namespace Test_Api_DB_Connect_Stylo_App.DTOs
{
    public class CheckoutRequestDto
    {
        public int KhachHangId { get; set; }
        public string KenhBan { get; set; } = "ONLINE";
        public decimal PhiVanChuyen { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
