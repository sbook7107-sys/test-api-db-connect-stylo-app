namespace Test_Api_DB_Connect_Stylo_App.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
