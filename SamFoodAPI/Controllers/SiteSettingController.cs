using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;

namespace SamFoodAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SiteSettingController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly SiteSettingRepo _siteSettingRepo;

    public SiteSettingController(SiteSettingRepo siteSettingRepo, IConfiguration configuration)
    {
        _siteSettingRepo = siteSettingRepo;
        _configuration = configuration;
    }

    // [Authorize]
    [HttpGet]
    public IActionResult GetAll(string? group = null)
    {
        try
        {
            var items = _siteSettingRepo.GetAll(s => !s.IsDeleted);
            if (!string.IsNullOrWhiteSpace(group))
            {
                items = items.Where(s => string.Equals(s.Group, group, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var ordered = items
                .OrderBy(s => s.Group, StringComparer.OrdinalIgnoreCase)
                .ThenBy(s => s.SortOrder)
                .ThenBy(s => s.ID)
                .ToList();
            return Ok(ApiResponseFactory.Success(ordered));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        try
        {
            var items = _siteSettingRepo.GetAll(s => !s.IsDeleted && s.IsPublic);
            var ordered = items
                .OrderBy(s => s.Group, StringComparer.OrdinalIgnoreCase)
                .ThenBy(s => s.SortOrder)
                .ThenBy(s => s.ID)
                .ToList();
            return Ok(ApiResponseFactory.Success(ordered));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    // [Authorize]
    [HttpPut("bulk")]
    public async Task<IActionResult> BulkUpdate([FromBody] List<SiteSetting> items)
    {
        try
        {
            if (items == null || items.Count == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Danh sách cập nhật không được rỗng!"));
            }

            int count = 0;
            foreach (var item in items)
            {
                if (item.ID <= 0) continue;
                var existing = await _siteSettingRepo.GetByIDAsync(item.ID);
                if (existing == null || existing.ID <= 0) continue;

                existing.SettingValue = item.SettingValue;
                existing.UpdatedBy = existing.UpdatedBy;
                existing.UpdatedDate = DateTime.Now;

                var result = await _siteSettingRepo.UpdateAsync(existing);
                if (result > 0) count++;
            }

            if (count > 0)
            {
                return Ok(ApiResponseFactory.Success(null, $"Đã cập nhật {count} cấu hình!"));
            }
            return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }

    // [Authorize]
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var settingKey = form["SettingKey"].ToString();
            var file = form.Files.GetFile("file");

            if (string.IsNullOrWhiteSpace(settingKey))
            {
                return BadRequest(ApiResponseFactory.Fail(null, "SettingKey không được để trống!"));
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "File ảnh không được rỗng!"));
            }

            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "File vượt quá 10MB!"));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico" };
            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
            if (string.IsNullOrEmpty(ext) || Array.IndexOf(allowedExtensions, ext) < 0)
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Định dạng không hợp lệ (chỉ chấp nhận jpg, jpeg, png, gif, webp, svg, ico)!"));
            }

            var contentType = file.ContentType?.ToLowerInvariant() ?? "";
            if (!contentType.StartsWith("image/") && !contentType.Contains("icon"))
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Content-Type phải là image/*!"));
            }

            var pathServer = _configuration.GetValue<string>("ImagePath");
            if (string.IsNullOrWhiteSpace(pathServer))
            {
                return BadRequest(ApiResponseFactory.Fail(null, "Không tìm thấy cấu hình đường dẫn ảnh!"));
            }

            var safeKey = settingKey.Replace("..", "").Replace("/", "").Replace("\\", "");
            var pathPattern = $@"Site/{safeKey}";
            var pathUpload = Path.Combine(pathServer, pathPattern);

            var uploadResult = await FileHelper.UploadFileAsync(file, pathUpload);

            if (uploadResult.status == 1)
            {
                var fileName = Convert.ToString(uploadResult.data) ?? "";
                var relativePath = $"{pathPattern}/{fileName}".Replace("\\", "/");
                return Ok(ApiResponseFactory.Success(new { path = relativePath }, "Upload thành công!"));
            }

            return BadRequest(ApiResponseFactory.Fail(null, "Upload thất bại!"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        }
    }
}
