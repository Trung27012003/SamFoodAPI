using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamFoodAPI.Attributes;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Model.Entities;
using SamFoodAPI.Repo;
using System.Runtime.CompilerServices;

namespace SamFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceRepo _invoiceRepo;
        private readonly InvoiceDetailRepo _detailRepo;

        public InvoiceController(InvoiceRepo invoiceRepo, InvoiceDetailRepo detailRepo)
        {
            _invoiceRepo = invoiceRepo;
            _detailRepo = detailRepo;
        }


        [HttpGet()]
        [RequiresPermission("N1")]
        public async Task<IActionResult> GetAll(string? keyword)
        {
            try
            {
                var data = _invoiceRepo.GetAll(x=>x.IsDeleted != true);
                //var data = await SqlDapper<object>.ProcedureToListAsync("spGetInvoice", new { keyword = keyword ?? "" });
                return Ok(ApiResponseFactory.Success(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpGet("{id}")]
        [RequiresPermission("N1")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var invoice = await _invoiceRepo.GetByIDAsync(id);
                var details = _detailRepo.GetAll(x => x.InvoiceID == invoice.ID && x.IsDeleted != true);
                
                return Ok(ApiResponseFactory.Success(new
                {
                    invoice,
                    details,
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }

        [HttpPost("save-data")]
        public async Task<IActionResult> SaveData([FromBody] InvoiceDTO invoice)
        {
            try
            {
                if (invoice == null)
                    return BadRequest(ApiResponseFactory.Fail(null, "Dữ liệu đơn hàng trống!"));

                int record = 0;
                if (invoice.ID <= 0)
                {
                    if (string.IsNullOrWhiteSpace(invoice.CustomerName))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập họ tên!"));

                    if (string.IsNullOrWhiteSpace(invoice.PhoneNumber))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập số điện thoại!"));

                    if (!System.Text.RegularExpressions.Regex.IsMatch(invoice.PhoneNumber, @"^(0[3|5|7|8|9])[0-9]{8}$"))
                        return BadRequest(ApiResponseFactory.Fail(null, "Số điện thoại không hợp lệ (10 số, đầu 03/05/07/08/09)!"));

                    if (string.IsNullOrWhiteSpace(invoice.Address))
                        return BadRequest(ApiResponseFactory.Fail(null, "Vui lòng nhập địa chỉ!"));

                    if (invoice.InvoiceDetails == null || invoice.InvoiceDetails.Count == 0)
                        return BadRequest(ApiResponseFactory.Fail(null, "Giỏ hàng trống, vui lòng thêm sản phẩm!"));

                    invoice.BillDate = DateTime.Now;
                    invoice.BillCode = _invoiceRepo.GetBillCode();
                    invoice.TotalAmount = invoice.InvoiceDetails?
                        .Sum(d => (d.Quantity ?? 0) * (d.UnitPrice ?? 0)) ?? 0;
                    record = await _invoiceRepo.CreateAsync(invoice);

                    foreach (var item in invoice.InvoiceDetails)
                    {
                        if (item.ID <= 0)
                        {
                            item.InvoiceID = invoice.ID;
                            await _detailRepo.CreateAsync(item);
                        }
                        else await _detailRepo.UpdateAsync(item);
                    }
                }
                else
                {
                    record = await _invoiceRepo.UpdateAsync(invoice);
                }

                if (record > 0) return Ok(ApiResponseFactory.Success(invoice, "Cập nhật thành công!"));
                else
                {
                    return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật đơn hàng thất bại. Vui lòng thử lại!", invoice));
                    //else if (recordIngre <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật nguyên liệu thất bại. Vui lòng thử lại!", product));
                    //else if (recordProcess <= 0) return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật công thức thất bại. Vui lòng thử lại!", product));
                    //else return BadRequest(ApiResponseFactory.Fail(null, "Cập nhật thất bại. Vui lòng thử lại!", product));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseFactory.Fail(ex, ex.Message));
            }
        }
    }
}
