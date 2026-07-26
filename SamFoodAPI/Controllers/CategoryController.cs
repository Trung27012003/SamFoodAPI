using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;
using System.Threading.Tasks;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly CategoryRepo _categoryRepo;
        public CategoryController(CategoryRepo categoryRepo, IConfiguration configuration)
        {
            _categoryRepo = categoryRepo;
            _configuration = configuration;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            try
            {
                var categorys = _categoryRepo.GetAll();
                return Ok(ApiResponseFactory.Success(categorys));
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
                var category = await _categoryRepo.GetByIDAsync(id);
                return Ok(ApiResponseFactory.Success(category));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] Category category)
        {
            try
            {
                int result = 0;
                if (category.ID <= 0) result = await _categoryRepo.CreateAsync(category);
                else result = await _categoryRepo.UpdateAsync(category);

                if(result > 0) return Ok(ApiResponseFactory.Success(category,"Cập nhật thành công!"));
                else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!",category));
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
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //_currentUser = ObjectMapper.GetCurrentUser(claims);

                var form = await Request.ReadFormAsync();
                var categoryID = Convert.ToInt32(form["CategoryID"]);

                //var imageRemoves = JsonConvert.DeserializeObject<List<Category>>(form["CategoryImageRemoves"]) ?? new List<Category>();
                //var images = _imageRepo.GetAll(x => x.ProductID == productID && x.IsDeleted != true);
                //foreach (var file in imageRemoves)
                //{
                //    file.ImageName = "";
                //    if (file.ID > 0) await _categoryRepo.UpdateAsync(file);
                //}
                
                var files = Request.Form.Files;

                var pathServer = _configuration.GetValue<string>("ImagePath");
                if (string.IsNullOrWhiteSpace(pathServer))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: PathPaymentOrder"));
                }

                var category = await _categoryRepo.GetByIDAsync(categoryID);

                //if (_currentUser.EmployeeID != order.EmployeeID)
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể bổ sung file vào đề nghị của người khác!"));
                //}

                string pathPattern = $@"Category/{category.CategoryCode}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                foreach (var file in files)
                {
                    var result = await FileHelper.UploadFileAsync(file, pathUpload);

                    if (result.status == 1)
                    {
                        //var productImage = new ProductImage();
                        //productImage.ProductID = product.ID;
                        //orderFile.FileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(file.FileName)}";
                        category.ImageName = Convert.ToString(result.data);

                        await _categoryRepo.UpdateAsync(category);
                    }
                }

                //Process.Start(pathUpload);

                return Ok(ApiResponseFactory.Success(null, "Cập nhật thành công!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
