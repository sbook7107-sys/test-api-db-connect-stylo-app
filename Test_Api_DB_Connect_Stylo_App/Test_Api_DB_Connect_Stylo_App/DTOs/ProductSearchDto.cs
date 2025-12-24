namespace Test_Api_DB_Connect_Stylo_App.DTOs
{
    public class ProductSearchDto
    {
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
