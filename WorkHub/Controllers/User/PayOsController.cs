using Azure;
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
using WorkHub.Models.Models;
using WorkHub.Utility;


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
        [HttpPost("create-payment")]
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

                 _unitOfWork.OrderRepository.Add(new Order
                {
                    OrderCode = orderCode,
                    UserId = userId,
                    Amount = req.TotalAmount,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.SaveAsync();


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

        [HttpGet("cancel-payment")]
        [Authorize]
        public async Task<IActionResult> CancelUrl(long orderCode)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.OrderCode == orderCode,
                includeProperties: SD.Join_User
            );

            if (order == null)
                return NotFound(ApiResponse<object>.NotFound("Order không tồn tại"));

            // prevent double update
            if (order.Status == SD.OrderStatus_Canceled)
                return Ok(ApiResponse<object>.Ok(null, "Order already canceled"));

            order.Status = SD.OrderStatus_Canceled;
            order.PaidAt = null; // ❗ canceled = not paid

            await _unitOfWork.SaveAsync();

            return Ok(ApiResponse<object>.Ok(null, "Payment canceled successfully"));
        }


        [HttpGet("return-payment")]
        [Authorize]
        public async Task<IActionResult> ReturnUrl(long orderCode)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.OrderCode == orderCode,
                includeProperties: SD.Join_User
            );

            if (order == null)
                return NotFound(ApiResponse<object>.NotFound("Order không tồn tại"));

            if (order.User == null)
                return NotFound(ApiResponse<object>.NotFound("User không tồn tại trong đơn hàng"));

            // 🔒 Prevent duplicate processing
            if (order.Status == SD.OrderStatus_Paid)
                return Ok(ApiResponse<object>.Ok(null, "Order already paid"));

            if (order.Status == SD.OrderStatus_Canceled)
                return BadRequest(ApiResponse<object>.BadRequest(null, "Order was canceled"));

            // ✅ Mark paid
            order.Status = SD.OrderStatus_Paid;
            order.PaidAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            var response = new
            {
                orderCode = order.OrderCode,
                amount = order.Amount,
                userEmail = order.User.Email
            };

            return Ok(ApiResponse<object>.Ok(response, "Payment Successful"));
        }




    }

}

