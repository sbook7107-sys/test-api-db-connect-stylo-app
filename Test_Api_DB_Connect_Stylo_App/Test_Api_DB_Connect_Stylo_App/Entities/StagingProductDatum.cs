using System;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Models;

public partial class StagingProductDatum
{
    public string? ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Brand { get; set; }

    public string? Category { get; set; }

    public string? SubCategory { get; set; }

    public decimal? Price { get; set; }

    public decimal? Cost { get; set; }

    public decimal? Rating { get; set; }

    public string? ImageUrl { get; set; }
}
