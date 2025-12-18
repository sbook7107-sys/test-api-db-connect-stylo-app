using Test_Api_DB_Connect_Stylo_App.Data;
using Test_Api_DB_Connect_Stylo_App.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Test_Api_DB_Connect_Stylo_App.Services
{
    public class ProductService
    {
        //private readonly FashionShopContext _context;

        //public ProductService(FashionShopContext context)
        //{
        //    _context = context;
        //}

        //public async Task<HomeScreenResponseDto> GetHomeScreenDataAsync()
        //{
        //    // Thiết lập thời gian chờ tối đa là 120 giây (2 phút) cho tất cả các lệnh SQL trong luồng này.
        //    _context.Database.SetCommandTimeout(120);

        //    // 1. Tải toàn bộ PhanLoai (Không thay đổi)
        //    var phanLoaiDtos = await _context.PhanLoais
        //        .AsNoTracking()
        //        .OrderBy(pl => pl.PhanLoaiId)
        //        .Select(pl => new PhanLoaiDto
        //        {
        //            Id = pl.PhanLoaiId,
        //            Ten = pl.Ten
        //        }).ToListAsync();

        //    // Sửa từ pageSize (20) thành 100 theo yêu cầu màn hình Home
        //    const int topCount = 50;

        //    // 2. Tải 100 Biến Thể đầu tiên (Mới nhất theo ID)
        //    var bienTheDtos = await _context.SanPhamBienThes
        //        .AsNoTracking()
        //        // Eager Loading SanPham cha (để lấy Tên, DanhMucId)
        //        .Include(bt => bt.SanPham)

        //        // Sắp xếp theo ID Biến Thể giảm dần (biến thể mới nhất)
        //        .OrderByDescending(bt => bt.BienTheId)

        //        .Take(topCount) // Lấy 100
        //        .Select(bt => new SanPhamBienTheHomeDto
        //        {
        //            BienTheId = bt.BienTheId,
        //            GiaBan = bt.GiaBan,
        //            Sku = bt.Sku,

        //            // Lấy thông tin từ SanPham cha
        //            SanPhamId = bt.SanPhamId,
        //            TenSanPham = bt.SanPham.TenSanPham,
        //            // Giả định Navigation Property: SanPham.DanhMucId
        //            DanhMucId = bt.SanPham.DanhMucId,

        //            ImageUrl = $"images/variants/{bt.BienTheId}.jpg"
        //        })
        //        .ToListAsync();

        //    // 3. Trả về DTO tổng hợp
        //    return new HomeScreenResponseDto
        //    {
        //        PhanLoaiList = phanLoaiDtos,
        //        InitialVariants = bienTheDtos,
        //        TotalVariantCount = 0 // Đã bỏ CountAsync
        //    };
        //}

        //// PHƯƠNG THỨC MỚI: Tải 20 Biến Thể theo PhanLoaiID
        //public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetTop20VariantsByPhanLoaiAsync(int phanLoaiId)
        //{
        //    _context.Database.SetCommandTimeout(60);

        //    var bienTheDtos = await _context.SanPhamBienThes
        //        .AsNoTracking()
        //        // Chỉ Include SanPham, không cần ThenInclude DanhMuc/PhanLoai
        //        .Include(bt => bt.SanPham)

        //        // SỬA ĐỔI ĐIỀU KIỆN WHERE: Sử dụng Subquery để lọc
        //        .Where(bt => bt.SanPham.DanhMucId.HasValue &&
        //                     _context.DanhMucs
        //                             .Where(dm => dm.PhanLoaiId == phanLoaiId)
        //                             // Kiểm tra xem DanhMucID của Sản phẩm có nằm trong list DanhMuc thuộc PhanLoai đó không
        //                             .Select(dm => dm.DanhMucId)
        //                             .Contains(bt.SanPham.DanhMucId.Value)
        //        )

        //        .OrderByDescending(bt => bt.BienTheId)
        //        .Take(20)
        //        .Select(bt => new SanPhamBienTheHomeDto
        //        {
        //            // ... (Phần Select DTO không đổi)
        //            BienTheId = bt.BienTheId,
        //            GiaBan = bt.GiaBan,
        //            Sku = bt.Sku,
        //            SanPhamId = bt.SanPhamId,
        //            TenSanPham = bt.SanPham.TenSanPham,
        //            DanhMucId = bt.SanPham.DanhMucId,
        //            ImageUrl = $"images/variants/{bt.BienTheId}.jpg"
        //        })
        //        .ToListAsync();

        //    return bienTheDtos;
        //}
        private readonly FashionShopContext _context;

        public ProductService(FashionShopContext context)
        {
            _context = context;
        }

        public async Task<HomeScreenResponseDto> GetHomeScreenDataAsync()
        {
            _context.Database.SetCommandTimeout(120);

            // 1. Lấy danh sách Phân loại
            var phanLoaiDtos = await _context.PhanLoais
                .AsNoTracking()
                .OrderBy(pl => pl.PhanLoaiId)
                .Select(pl => new PhanLoaiDto
                {
                    Id = pl.PhanLoaiId,
                    Ten = pl.Ten
                }).ToListAsync();

            // 2. Lấy 50 sản phẩm mới nhất (với giá thấp nhất) cho trang Home
            // Chúng ta bắt đầu truy vấn từ SanPham để đảm bảo tính duy nhất của Sản phẩm
            var products = await GetProductDataQuery(null, 50);

            return new HomeScreenResponseDto
            {
                PhanLoaiList = phanLoaiDtos,
                InitialVariants = products, // Dù tên là Variants nhưng dữ liệu là Product đại diện
                TotalVariantCount = 0
            };
        }

        public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetTop50VariantsByPhanLoaiAsync(int phanLoaiId)
        {
            _context.Database.SetCommandTimeout(60);
            return await GetProductDataQuery(phanLoaiId, 50);
        }

        // HÀM DÙNG CHUNG ĐỂ XỬ LÝ LOGIC LẤY GIÁ MIN VÀ ẢNH
        private async Task<List<SanPhamBienTheHomeDto>> GetProductDataQuery(int? phanLoaiId, int takeCount)
        {
            var query = _context.SanPhams.AsNoTracking();

            // Nếu có lọc theo phân loại
            if (phanLoaiId.HasValue)
            {
                var danhMucIds = _context.DanhMucs
                    .Where(dm => dm.PhanLoaiId == phanLoaiId.Value)
                    .Select(dm => dm.DanhMucId);

                query = query.Where(sp => sp.DanhMucId.HasValue && danhMucIds.Contains(sp.DanhMucId.Value));
            }

            return await query
                .OrderByDescending(sp => sp.SanPhamId)
                .Take(takeCount)
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    DanhMucId = sp.DanhMucId,

                    // LẤY GIÁ BÁN THẤP NHẤT TỪ BẢNG BIẾN THỂ
                    GiaBan = sp.SanPhamBienThes.Any()
                             ? sp.SanPhamBienThes.Min(bt => bt.GiaBan)
                             : 0,

                    // LẤY BIENTHEID CỦA THẰNG CÓ GIÁ THẤP NHẤT ĐÓ
                    BienTheId = sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.BienTheId).FirstOrDefault(),

                    // LẤY SKU ĐẠI DIỆN
                    Sku = sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.Sku).FirstOrDefault(),

                    // LẤY ẢNH: Ưu tiên ảnh của Biến thể giá thấp nhất, nếu không có thì lấy ảnh của Sản phẩm, 
                    // nếu vẫn không có lấy ảnh đầu tiên bất kỳ.
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary) // Ưu tiên ảnh chính
                                .ThenBy(a => a.BienTheId == sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.BienTheId).FirstOrDefault() ? 0 : 1)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "default-product.jpg"
                })
                .ToListAsync();
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(int sanPhamId)
        {
            _context.Database.SetCommandTimeout(60);

            return await _context.SanPhams
                .AsNoTracking()
                .Where(sp => sp.SanPhamId == sanPhamId)
                .Select(sp => new ProductDetailDto
                {
                    SanPhamId = sp.SanPhamId,
                    Name = sp.TenSanPham,
                    Description = sp.MoTa ?? "",
                    // Giá thấp nhất để hiển thị ban đầu
                    BasePrice = sp.SanPhamBienThes.Min(bt => bt.GiaBan),
                    // Ảnh chính của sản phẩm
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "default-product.jpg",

                    // Lấy danh sách màu sắc độc nhất từ các biến thể
                    AvailableColors = sp.SanPhamBienThes
                        .Select(bt => bt.Mau) // Giả định Navigation Property là 'Mau'
                        .Distinct()
                        .Select(m => new MauSacDto { Id = m.MauId, Ten = m.Ten, MaHex = m.MaHex ?? "" })
                        .ToList(),

                    // Lấy danh sách size độc nhất từ các biến thể
                    AvailableSizes = sp.SanPhamBienThes
                        .Select(bt => bt.Size) // Giả định Navigation Property là 'Size'
                        .Distinct()
                        .OrderBy(s => s.ThuTu)
                        .Select(s => new SizeDto { Id = s.SizeId, KyHieu = s.KyHieu })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<decimal?> GetPriceByVariantAsync(int sanPhamId, int mauId, int sizeId)
        {
            // API này dùng khi người dùng đã chọn đủ cả Màu và Size
            return await _context.SanPhamBienThes
                .AsNoTracking()
                .Where(bt => bt.SanPhamId == sanPhamId && bt.MauId == mauId && bt.SizeId == sizeId)
                .Select(bt => bt.GiaBan)
                .FirstOrDefaultAsync();
        }

        // 1. Lấy toàn bộ Phân loại
        public async Task<IEnumerable<PhanLoaiDto>> GetAllPhanLoaiAsync()
        {
            return await _context.PhanLoais
                .AsNoTracking()
                .OrderBy(pl => pl.Ten)
                .Select(pl => new PhanLoaiDto
                {
                    Id = pl.PhanLoaiId,
                    Ten = pl.Ten
                }).ToListAsync();
        }

        // 2. Lấy toàn bộ Danh mục theo Phân loại ID
        public async Task<IEnumerable<DanhMucDto>> GetDanhMucsByPhanLoaiAsync(int phanLoaiId)
        {
            return await _context.DanhMucs
                .AsNoTracking()
                .Where(dm => dm.PhanLoaiId == phanLoaiId)
                .OrderBy(dm => dm.Ten)
                .Select(dm => new DanhMucDto
                {
                    Id = dm.DanhMucId,
                    Ten = dm.Ten
                }).ToListAsync();
        }

        // 3. Lấy 50 sản phẩm theo Danh mục (Lấy kèm giá min và ảnh đại diện)
        public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetProductsByDanhMucAsync(int danhMucId)
        {
            // Thiết lập timeout để xử lý dữ liệu lớn (45.000+ bản ghi)
            _context.Database.SetCommandTimeout(60);

            return await _context.SanPhams
                .AsNoTracking()
                .Where(sp => sp.DanhMucId == danhMucId)
                .OrderByDescending(sp => sp.SanPhamId)
                .Take(50)
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    // Lấy thông tin cơ bản từ SanPham
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    DanhMucId = sp.DanhMucId,

                    // KỸ THUẬT: Lấy biến thể có giá thấp nhất của sản phẩm này
                    // Sắp xếp các biến thể theo GiaBan tăng dần, lấy thằng đầu tiên
                    BienTheId = sp.SanPhamBienThes
                                .OrderBy(bt => bt.GiaBan)
                                .Select(bt => bt.BienTheId)
                                .FirstOrDefault(),

                    GiaBan = sp.SanPhamBienThes
                                .OrderBy(bt => bt.GiaBan)
                                .Select(bt => bt.GiaBan)
                                .FirstOrDefault(),

                    Sku = sp.SanPhamBienThes
                                .OrderBy(bt => bt.GiaBan)
                                .Select(bt => bt.Sku)
                                .FirstOrDefault(),

                    // Lấy ảnh: Ưu tiên ảnh chính (IsPrimary), nếu không có lấy ảnh bất kỳ của SP đó
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "https://via.placeholder.com/600x800?text=No+Image"
                })
                .ToListAsync();
        }
    }
}
