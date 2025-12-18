using Test_Api_DB_Connect_Stylo_App.Data;
using Test_Api_DB_Connect_Stylo_App.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Services
{
    public class ProductService
    {
        private readonly FashionShopContext _context;

        public ProductService(FashionShopContext context)
        {
            _context = context;
        }

        public async Task<HomeScreenResponseDto> GetHomeScreenDataAsync()
        {
            // Thiết lập thời gian chờ tối đa là 120 giây (2 phút) cho tất cả các lệnh SQL trong luồng này.
            _context.Database.SetCommandTimeout(120);

            // 1. Tải toàn bộ PhanLoai (Không thay đổi)
            var phanLoaiDtos = await _context.PhanLoais
                .AsNoTracking()
                .OrderBy(pl => pl.PhanLoaiId)
                .Select(pl => new PhanLoaiDto
                {
                    Id = pl.PhanLoaiId,
                    Ten = pl.Ten
                }).ToListAsync();

            // Sửa từ pageSize (20) thành 100 theo yêu cầu màn hình Home
            const int topCount = 50;

            // 2. Tải 100 Biến Thể đầu tiên (Mới nhất theo ID)
            var bienTheDtos = await _context.SanPhamBienThes
                .AsNoTracking()
                // Eager Loading SanPham cha (để lấy Tên, DanhMucId)
                .Include(bt => bt.SanPham)

                // Sắp xếp theo ID Biến Thể giảm dần (biến thể mới nhất)
                .OrderByDescending(bt => bt.BienTheId)

                .Take(topCount) // Lấy 100
                .Select(bt => new SanPhamBienTheHomeDto
                {
                    BienTheId = bt.BienTheId,
                    GiaBan = bt.GiaBan,
                    Sku = bt.Sku,

                    // Lấy thông tin từ SanPham cha
                    SanPhamId = bt.SanPhamId,
                    TenSanPham = bt.SanPham.TenSanPham,
                    // Giả định Navigation Property: SanPham.DanhMucId
                    DanhMucId = bt.SanPham.DanhMucId,

                    ImageUrl = $"images/variants/{bt.BienTheId}.jpg"
                })
                .ToListAsync();

            // 3. Trả về DTO tổng hợp
            return new HomeScreenResponseDto
            {
                PhanLoaiList = phanLoaiDtos,
                InitialVariants = bienTheDtos,
                TotalVariantCount = 0 // Đã bỏ CountAsync
            };
        }

        // PHƯƠNG THỨC MỚI: Tải 20 Biến Thể theo PhanLoaiID
        public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetTop20VariantsByPhanLoaiAsync(int phanLoaiId)
        {
            _context.Database.SetCommandTimeout(60);

            var bienTheDtos = await _context.SanPhamBienThes
                .AsNoTracking()
                // Chỉ Include SanPham, không cần ThenInclude DanhMuc/PhanLoai
                .Include(bt => bt.SanPham)

                // SỬA ĐỔI ĐIỀU KIỆN WHERE: Sử dụng Subquery để lọc
                .Where(bt => bt.SanPham.DanhMucId.HasValue &&
                             _context.DanhMucs
                                     .Where(dm => dm.PhanLoaiId == phanLoaiId)
                                     // Kiểm tra xem DanhMucID của Sản phẩm có nằm trong list DanhMuc thuộc PhanLoai đó không
                                     .Select(dm => dm.DanhMucId)
                                     .Contains(bt.SanPham.DanhMucId.Value)
                )

                .OrderByDescending(bt => bt.BienTheId)
                .Take(20)
                .Select(bt => new SanPhamBienTheHomeDto
                {
                    // ... (Phần Select DTO không đổi)
                    BienTheId = bt.BienTheId,
                    GiaBan = bt.GiaBan,
                    Sku = bt.Sku,
                    SanPhamId = bt.SanPhamId,
                    TenSanPham = bt.SanPham.TenSanPham,
                    DanhMucId = bt.SanPham.DanhMucId,
                    ImageUrl = $"images/variants/{bt.BienTheId}.jpg"
                })
                .ToListAsync();

            return bienTheDtos;
        }
       
    }
}
