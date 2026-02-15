using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.V2.PaymentRequests.Invoices;
using System.Security.Claims;
using WorkHub.DataAccess.Repository.IRepository;
using WorkHub.Models.DTOs;
using WorkHub.Models.DTOs.ModelDTOs.PaymentDTOs;


namespace WorkHub.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly PayOSClient _client;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
     


        public PayOsController([FromKeyedServices("OrderClient")] PayOSClient client
            , IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _client = client;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDTO req)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);

            var payosConfig = _configuration.GetSection("PayOS");

       

            if (user == null)
                return BadRequest(ApiResponse<object>.BadRequest(null, "User not found"));

            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = req.TotalAmount,

                BuyerName = user.FullName,
                BuyerCompanyName = user.FullName,
                BuyerEmail = user.Email,
                BuyerPhone = user.Phone,

                Description = "Thanh toán đơn hàng",
                CancelUrl = payosConfig["CancelUrl"],
                ReturnUrl = payosConfig["ReturnUrl"],

                ExpiredAt = req.ExpiredAt?.ToUnixTimeSeconds(),

                Items = req.Items.Select(i => new PaymentLinkItem
                {
                    Name = i.Name ?? "",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            try
            {
                var paymentLink = await _client.PaymentRequests.CreateAsync(paymentRequest);


                var response = new
                {
                    checkoutUrl = paymentLink, // ❗ you forgot .checkoutUrl
                    orderCode = orderCode
                };

                return Ok(ApiResponse<object>.Ok(response, "Payment Created"));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Error creating payment link", ex.Message));
            }
        
        }

        //// Trang hủy
        //[HttpGet]
        //public IActionResult CancelUrl(int orderCode)
        //{
        //    var order = _context.OrderTables.FirstOrDefault(o => o.OrderID == orderCode);

        //    if (order == null)
        //    {
        //        return NotFound(new { message = "Order không tồn tại" });
        //    }

        //    // Cập nhật trạng thái đơn hàng thành "Canceled"
        //    order.Status = OrderStatus.Cancelled;
        //    order.UpdatedAt = DateTime.UtcNow;
        //    _context.SaveChanges();

        //    return View();
        //}

        //[HttpGet]
        //public async Task<IActionResult> ReturnUrl(int orderCode)
        //{
        //    // Tìm đơn hàng trong database với User
        //    var order = await _context.OrderTables
        //        .Include(o => o.User) // Ensure User is loaded
        //        .FirstOrDefaultAsync(o => o.OrderID == 2);

        //    if (order == null)
        //    {
        //        return NotFound(new { message = "Order không tồn tại" });
        //    }

        //    if (order.User == null)
        //    {
        //        return NotFound(new { message = "User không tồn tại trong đơn hàng" });
        //    }



        //    // Cập nhật trạng thái đơn hàng thành "Completed"
        //    order.Status = OrderStatus.Completed;
        //    order.UpdatedAt = DateTime.UtcNow;

        //    await _context.SaveChangesAsync(); // Ensure async save



        //    // Gửi QR code qua email
        //    await GenerateTicket(order);

        //    return View();
        //}

    }
}
