using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;

namespace SamFoodAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BannerController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly BannerRepo _bannerRepo;
    private readonly BannerDetailRepo _bannerDetailRepo;

    public BannerController(BannerRepo bannerRepo, BannerDetailRepo bannerDetailRepo, IConfiguration configuration)
    {
        _bannerRepo = bannerRepo;
        _bannerDetailRepo = bannerDetailRepo;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAll(string? keyword)
    {
        try
        {
            var banners = _bannerRepo.GetAll(b => !b.IsDeleted);

            var activeBannerIDs = banners.Select(b => b.ID).ToList();
            var detailsByBanner = _bannerDetailRepo
                .GetAll(d => !d.IsDeleted && activeBannerIDs.Contains(d.BannerID))
                .GroupBy(d => d.BannerID)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.SortOrder).ThenBy(x => x.ID).ToList());

            var dtos = banners.Select(b =>
                ObjectMapper.ToBannerWithDetailsDto(
                    b,
                    detailsByBanner.TryGetValue(b.ID, out var ds) ? ds : new List<Model.Entities.BannerDetail>()
                )
            ).ToList();

            return Ok(ApiResponseFactory.Success(dtos));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(int id)
    {
        try
        {
            var banner = await _bannerRepo.GetByIDAsync(id);
            if (banner == null || banner.ID <= 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy banner!"));
            }

            var details = _bannerDetailRepo.GetAll(d => d.BannerID == id && !d.IsDeleted);
            var dto = ObjectMapper.ToBannerWithDetailsDto(banner, details);

            return Ok(ApiResponseFactory.Success(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpPost("save-data")]
    public async Task<IActionResult> SaveData([FromBody] Banner banner)
    {
        try
        {
            if (banner == null)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu banner không hợp lệ!"));
            }

            if (string.IsNullOrWhiteSpace(banner.BannerCode))
            {
                return BadRequest(ApiResponseFactory.Fail(null, "BannerCode không được để trống!"));
            }

            if (string.IsNullOrWhiteSpace(banner.BannerName))
            {
                return BadRequest(ApiResponseFactory.Fail(null, "BannerName không được để trống!"));
            }

            if (banner.SlideshowInterval <= 0)
            {
                banner.SlideshowInterval = 5;
            }

            int result = 0;
            int savedBannerID = banner.ID;
            if (banner.ID <= 0)
            {
                var existing = _bannerRepo.GetAll(b =>
                    b.BannerCode == banner.BannerCode && !b.IsDeleted).FirstOrDefault();
                if (existing != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,
                        $"BannerCode '{banner.BannerCode}' đã tồn tại!"));
                }

                banner.CreatedDate = DateTime.Now;
                banner.UpdatedDate = DateTime.Now;
                result = await _bannerRepo.CreateAsync(banner);

                var created = _bannerRepo.GetAll(b => b.BannerCode == banner.BannerCode && !b.IsDeleted)
                    .OrderByDescending(b => b.ID)
                    .FirstOrDefault();
                if (created != null) savedBannerID = created.ID;
            }
            else
            {
                var existing = await _bannerRepo.GetByIDAsync(banner.ID);
                if (existing == null || existing.ID <= 0)
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy banner cần cập nhật!"));
                }

                var duplicate = _bannerRepo.GetAll(b =>
                    b.BannerCode == banner.BannerCode && !b.IsDeleted && b.ID != banner.ID).FirstOrDefault();
                if (duplicate != null)
                {
                    return BadRequest(ApiResponseFactory.Fail(null,
                        $"BannerCode '{banner.BannerCode}' đã được sử dụng bởi banner khác!"));
                }

                banner.CreatedDate = existing.CreatedDate;
                result = await _bannerRepo.UpdateAsync(banner);
            }

            if (banner.IsActive && result > 0 && savedBannerID > 0)
            {
                await DeactivateOtherBannersAsync(savedBannerID);
            }

            if (result > 0) return Ok(ApiResponseFactory.Success(banner, "Cập nhật thành công!"));
            else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!", banner));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpPost("save-detail")]
    public async Task<IActionResult> SaveDetail([FromBody] BannerDetail detail)
    {
        try
        {
            int result = 0;
            if (detail.ID <= 0)
            {
                detail.CreatedDate = DateTime.Now;
                detail.UpdatedDate = DateTime.Now;
                result = await _bannerDetailRepo.CreateAsync(detail);
            }
            else if (detail.IsDeleted)
            {
                var existing = await _bannerDetailRepo.GetByIDAsync(detail.ID);
                if (existing != null)
                {
                    existing.IsDeleted = true;
                    result = await _bannerDetailRepo.UpdateAsync(existing);
                }
            }
            else
            {
                result = await _bannerDetailRepo.UpdateAsync(detail);
            }

            if (result > 0) return Ok(ApiResponseFactory.Success(detail, "Cập nhật thành công!"));
            else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!", detail));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpPost("save-details")]
    public async Task<IActionResult> SaveDetails([FromBody] List<BannerDetail> details)
    {
        try
        {
            int result = 0;
            foreach (var detail in details)
            {
                if (detail.ID <= 0)
                {
                    detail.CreatedDate = DateTime.Now;
                    detail.UpdatedDate = DateTime.Now;
                    result += await _bannerDetailRepo.CreateAsync(detail);
                }
                else if (detail.IsDeleted)
                {
                    var existing = await _bannerDetailRepo.GetByIDAsync(detail.ID);
                    if (existing != null)
                    {
                        existing.IsDeleted = true;
                        result += await _bannerDetailRepo.UpdateAsync(existing);
                    }
                }
                else
                {
                    result += await _bannerDetailRepo.UpdateAsync(detail);
                }
            }

            if (result > 0) return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
            else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var bannerID = Convert.ToInt32(form["BannerID"]);

            var files = Request.Form.Files;

            var pathServer = _configuration.GetValue<string>("ImagePath");
            if (string.IsNullOrWhiteSpace(pathServer))
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy cấu hình đường dẫn!"));
            }

            var banner = await _bannerRepo.GetByIDAsync(bannerID);
            if (banner == null || banner.ID <= 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy banner!"));
            }

            var pendingDetailsJson = form["details"].ToString();
            var pendingDetails = string.IsNullOrWhiteSpace(pendingDetailsJson)
                ? new List<BannerDetail>()
                : System.Text.Json.JsonSerializer.Deserialize<List<BannerDetail>>(pendingDetailsJson) ?? new List<BannerDetail>();
            //var pendingDetails = _bannerDetailRepo.GetAll(c=>c.BannerID == banner.ID);
            string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            long maxFileSize = 10 * 1024 * 1024;
            var uploadedFiles = new List<string>();
            var rejectedFiles = new List<string>();

            // Phase 1: Upload tất cả files, giữ nguyên details cũ trong DB
            for (int fileIdx = 0; fileIdx < files.Count; fileIdx++)
            {
                var file = files[fileIdx];
                if (file == null || file.Length == 0)
                {
                    rejectedFiles.Add($"{file?.FileName ?? "(unknown)"}: file rỗng");
                    continue;
                }

                if (file.Length > maxFileSize)
                {
                    rejectedFiles.Add($"{file.FileName}: vượt quá 10MB");
                    continue;
                }

                var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
                if (string.IsNullOrEmpty(ext) || Array.IndexOf(allowedExtensions, ext) < 0)
                {
                    rejectedFiles.Add($"{file.FileName}: định dạng không hợp lệ (chỉ chấp nhận jpg, jpeg, png, gif, webp)");
                    continue;
                }

                var contentType = file.ContentType?.ToLowerInvariant() ?? "";
                if (!contentType.StartsWith("image/"))
                {
                    rejectedFiles.Add($"{file.FileName}: Content-Type không phải image/*");
                    continue;
                }

                string pathPattern = $@"Banner/{banner.BannerCode}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                var result = await FileHelper.UploadFileAsync(file, pathUpload);

                if (result.status == 1)
                {
                    var filename = Convert.ToString(result.data) ?? "";
                    uploadedFiles.Add(filename);
                }
                else
                {
                    rejectedFiles.Add($"{file.FileName}: upload thất bại");
                }
            }

            // Phase 2: Insert/Update pendingDetails - KHÔNG xóa details cũ
            for (int i = 0; i < pendingDetails.Count; i++)
            {
                var pendingDetail = pendingDetails[i];
                if (pendingDetail == null) continue;

                pendingDetail.ImageName = i < uploadedFiles.Count ? uploadedFiles[i] : pendingDetail.ImageName;
                pendingDetail.BannerID = bannerID;
                pendingDetail.IsDeleted = false;
                pendingDetail.UpdatedDate = DateTime.Now;

                if (pendingDetail.ID <= 0)
                {
                    pendingDetail.CreatedDate = DateTime.Now;
                    await _bannerDetailRepo.CreateAsync(pendingDetail);
                }
                else
                {
                    await _bannerDetailRepo.UpdateAsync(pendingDetail);
                }
            }

            var responseData = new
            {
                files = uploadedFiles,
                rejected = rejectedFiles
            };

            if (uploadedFiles.Count == 0 && rejectedFiles.Count > 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null,
                    $"Upload thất bại. {string.Join("; ", rejectedFiles)}",
                    responseData));
            }

            var message = $"Đã upload {uploadedFiles.Count} file thành công!";
            if (rejectedFiles.Count > 0)
            {
                message += $" Bỏ qua {rejectedFiles.Count} file không hợp lệ.";
            }

            return Ok(ApiResponseFactory.Success(responseData, message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var banner = await _bannerRepo.GetByIDAsync(id);
            if (banner == null || banner.ID <= 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy banner!"));
            }

            banner.IsDeleted = true;
            var result = await _bannerRepo.UpdateAsync(banner);

            foreach (var detail in banner.Details ?? Enumerable.Empty<Model.Entities.BannerDetail>())
            {
                if (!detail.IsDeleted)
                {
                    detail.IsDeleted = true;
                    await _bannerDetailRepo.UpdateAsync(detail);
                }
            }

            var folderToDelete = Path.Combine(
                _configuration.GetValue<string>("ImagePath") ?? "",
                "Banner",
                banner.BannerCode ?? ""
            );

            try
            {
                if (Directory.Exists(folderToDelete))
                {
                    Directory.Delete(folderToDelete, recursive: true);
                }
            }
            catch (Exception fsEx)
            {
                Console.WriteLine($"[Banner.Delete] Không thể xóa folder '{folderToDelete}': {fsEx.Message}");
            }

            if (result > 0) return Ok(ApiResponseFactory.Success(null, "Xóa thành công!"));
            else return BadRequest(ApiResponseFactory.Fail(null, "Xóa thất bại. Vui lòng thử lại!"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    private async Task DeactivateOtherBannersAsync(int activeBannerID)
    {
        var others = _bannerRepo.GetAll(b => !b.IsDeleted && b.IsActive && b.ID != activeBannerID).ToList();
        foreach (var b in others)
        {
            b.IsActive = false;
            b.UpdatedDate = DateTime.Now;
            await _bannerRepo.UpdateAsync(b);
        }
    }
}
