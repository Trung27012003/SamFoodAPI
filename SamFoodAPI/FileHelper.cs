using SamFoodAPI.Model.Common;
using static SamFoodAPI.Model.Common.ApiResponseFactory;

namespace SamFoodAPI
{
    public static class FileHelper
    {
        public static async Task<APIResponse> UploadFileAsync(IFormFile file, string path, string newFileName = "")
        {
            try
            {
                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (file.Length > 0)
                {
                    // Tạo tên file unique
                    string fileExtension = Path.GetExtension(file.FileName);
                    string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string uniqueFileName = $"{originalFileName}_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}{fileExtension}";

                    if (!string.IsNullOrWhiteSpace(newFileName)) uniqueFileName = newFileName;
                    var fullPath = Path.Combine(path, uniqueFileName);

                    // Lưu file
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return ApiResponseFactory.Success(uniqueFileName, $"Upload thành công!");
                }
                return ApiResponseFactory.Fail(null, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
