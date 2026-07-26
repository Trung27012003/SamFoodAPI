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
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ProductRepo _productRepo;
        private readonly ProductIngredientRepo _productIngreRepo;
        private readonly ProductProcessingRecipeRepo _productProcessRepo;
        private readonly ProductImageRepo _imageRepo;

        public ProductController(IConfiguration configuration, ProductRepo productRepo, ProductIngredientRepo productIngreRepo, ProductProcessingRecipeRepo productProcessRepo, ProductImageRepo imageRepo)
        {
            _configuration = configuration;
            _productRepo = productRepo;
            _productIngreRepo = productIngreRepo;
            _productProcessRepo = productProcessRepo;
            _imageRepo = imageRepo;
        }


        [HttpGet("cart-items")]
        public async Task<IActionResult> GetCartItems(string ids)
        {
            try
            {
                var idList = ids.Split(',').Select(int.Parse).ToList();
                var products =  _productRepo.GetAll(x => idList.Contains(x.ID) && x.IsDeleted != true);
                var result = new List<object>();

                foreach (var product in products)
                {
                    var images = _imageRepo.GetAll(x => x.ProductID == product.ID && x.IsDeleted != true)
                        .Select(x => new
                        {
                            FileName = x.FileName,
                            ProductCode = product.ProductCode,
                            IsPrimary = x.IsPrimary
                        }).ToList();

                    result.Add(new
                    {
                        ProductID = product.ID,
                        ProductCode = product.ProductCode,
                        ProductName = product.ProductName,
                        UnitPrice = product.UnitPrice,
                        Images = images
                    });
                }

                return Ok(ApiResponseFactory.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll(string? keyword)
        {
            try
            {
                //var data = _productRepo.GetAll(x=>x.IsDeleted != true);
                var data = await SqlDapper<object>.ProcedureToListAsync("spGetProduct", new { keyword = keyword ?? "" });
                return Ok(ApiResponseFactory.Success(data));
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
                var product = await _productRepo.GetByIDAsync(id);

                var productIngres = _productIngreRepo.GetAll(x => x.ProductID == product.ID && x.IsDeleted != true);
                var productProcess = _productProcessRepo.GetAll(x => x.ProductID == product.ID && x.IsDeleted != true);
                var productImages = _imageRepo.GetAll(x => x.ProductID == product.ID && x.IsDeleted != true).
                                    Select(x => new
                                    {
                                        ID = x.ID,
                                        FileName = x.FileName,
                                        ProductCode = product.ProductCode,
                                        IsPrimary = x.IsPrimary
                                    }).ToList();
                return Ok(ApiResponseFactory.Success(new
                {
                    product,
                    productIngres,
                    productProcess,
                    productImages
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] ProductDTO product)
        {
            try
            {
                int record = 0;
                if (product.ID <= 0) record = await _productRepo.CreateAsync(product);
                else record = await _productRepo.UpdateAsync(product);

                int recordIngre = await _productIngreRepo.Create(product);
                int recordProcess = await _productProcessRepo.Create(product);

                if (record > 0 && recordIngre > 0 && recordProcess > 0) return Ok(ApiResponseFactory.Success(product, "Cập nhật thành công!"));
                else
                {
                    if (record <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật sản phẩm thất bại. Vui lòng thử lại!", product));
                    else if (recordIngre <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật nguyên liệu thất bại. Vui lòng thử lại!", product));
                    else if (recordProcess <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật công thức thất bại. Vui lòng thử lại!", product));
                    else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!", product));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        //[HttpGet("images")]
        //public async Task<IActionResult> GetImages(int productID)
        //{
        //    try
        //    {
        //        var product = await _productRepo.GetByIDAsync(productID);
        //        var data = _imageRepo.GetAll(x => x.ProductID == productID && x.IsDeleted != true).
        //                            Select(x => new
        //                            {
        //                                ID = x.ID,
        //                                FileName = x.FileName,
        //                                ProductCode = product.ProductCode
        //                            }).ToList();

        //        return Ok(ApiResponseFactory.Success(data));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
        //    }
        //}

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                //_currentUser = HttpContext.Session.GetObject<CurrentUser>(_configuration.GetValue<string>("SessionKey") ?? "");

                //var claims = User.Claims.ToDictionary(x => x.Type, x => x.Value);
                //_currentUser = ObjectMapper.GetCurrentUser(claims);

                var form = await Request.ReadFormAsync();
                var productID = Convert.ToInt32(form["ProductID"]);
                var primaryImageID = form["PrimaryImageID"].ToString();
                var primaryImageIdInt = 0;
                if (!string.IsNullOrEmpty(primaryImageID) && int.TryParse(primaryImageID, out var parsedId))
                {
                    primaryImageIdInt = parsedId;
                }

                var imageRemoves = JsonConvert.DeserializeObject<List<ProductImage>>(form["ProductImageRemoves"]) ?? new List<ProductImage>();
                //var images = _imageRepo.GetAll(x => x.ProductID == productID && x.IsDeleted != true);
                foreach (var file in imageRemoves)
                {
                    file.IsDeleted = true;
                    if (file.ID > 0) await _imageRepo.UpdateAsync(file);
                }

                var files = Request.Form.Files;

                // Lấy đường dẫn từ ConfigSystem
                //var pathServer = _configSystemRepo.GetUploadPathByKey("PathPaymentOrder");
                //var pathServer = @"F:\\Angular\\Image";
                var pathServer = _configuration.GetValue<string>("ImagePath");
                if (string.IsNullOrWhiteSpace(pathServer))
                {
                    return BadRequest(ApiResponseFactory.Fail(null, $"Không tìm thấy cấu hình đường dẫn cho key: PathPaymentOrder"));
                }

                var product = await _productRepo.GetByIDAsync(productID);

                //if (_currentUser.EmployeeID != order.EmployeeID)
                //{
                //    return BadRequest(ApiResponseFactory.Fail(null, "Bạn không thể bổ sung file vào đề nghị của người khác!"));
                //}

                string pathPattern = $@"Product/{product.ProductCode}";
                string pathUpload = Path.Combine(pathServer, pathPattern);

                int firstImageID = 0;
                foreach (var file in files)
                {
                    var result = await FileHelper.UploadFileAsync(file, pathUpload);

                    if (result.status == 1)
                    {
                        var productImage = new ProductImage();
                        productImage.ProductID = product.ID;
                        //orderFile.FileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(file.FileName)}";
                        productImage.FileName = Convert.ToString(result.data);
                        productImage.IsPrimary = false;
                        if (firstImageID == 0)
                        {
                            firstImageID = await _imageRepo.CreateAsync(productImage);
                        }
                        else
                        {
                            await _imageRepo.CreateAsync(productImage);
                        }
                    }
                }

                // Cập nhật IsPrimary cho ảnh được chọn
                if (primaryImageIdInt > 0)
                {
                    await _imageRepo.UpdateIsPrimaryAsync(productID, primaryImageIdInt);
                }
                else if (firstImageID > 0)
                {
                    // Nếu không có ảnh nào được chọn làm ảnh chính, đặt ảnh đầu tiên làm ảnh chính
                    await _imageRepo.UpdateIsPrimaryAsync(productID, firstImageID);
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
