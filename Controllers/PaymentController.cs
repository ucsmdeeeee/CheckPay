using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using WebApplication10.Models;
using WebApplication10.Data;

namespace WebApplication10.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppDbContext _context;

        public PaymentController(IHttpClientFactory clientFactory, AppDbContext context)
        {
            _clientFactory = clientFactory;
            _context = context;
        }


        [Route("/check-payment")]
    [HttpGet("/check-payment")]
        public async Task<IActionResult> CheckPayment([FromQuery] string order)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJNQTYyMjk3NiIsImp0aSI6ImI1OTNkODRkLTk1MWYtNGIyZi05ZGViLTcxOWExNDM4NWVmZCJ9.si-87k3Aw5GN67orgJpoyTXC0C2OpWwRCKzLogRWawU");

                string qrId = null;
                var order2 = _context.Order_.FirstOrDefault(o => o.OrderNum == order);
                if (order2 != null)
                {
                    qrId = order2.QrId;
                }
                else
                {
                    return NotFound($"Order with order number {order} not found.");
                }

                if (qrId != null)
                {
                    var response = await client.GetAsync($"https://pay-test.raif.ru/api/sbp/v1/qr/{qrId}/payment-info");
                    if (response.IsSuccessStatusCode)
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();
                        var jsonDocument = await JsonDocument.ParseAsync(contentStream);

                        var result = jsonDocument.RootElement;
                        var status = result.GetProperty("paymentStatus").GetString();

                        Console.WriteLine($"Requested payment status for order: {order}. Response status: {status}");

                        var responseModel = new PaymentStatusModel
                        {
                            OrderId = order,
                            Status = status
                        };

                        return Ok(responseModel);
                    }
                    else
                    {
                        Console.WriteLine($"Error response received for order {order}: {response.StatusCode}");
                        return BadRequest("Failed to get payment status");
                    }
                }
                else
                {
                    return BadRequest("QR ID is not defined for the order.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the request: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [Route("/create-qr")]
        [HttpPost("/create-qr")]
        public async Task<IActionResult> CreateQR([FromBody] PaymentRequestModel request)
        {
            try
            {
                Console.WriteLine("Received payment request data:");
                Console.WriteLine($"OrderId: {request.order}");
                Console.WriteLine($"QrType: {request.qrType}");
                Console.WriteLine($"SbpMerchantId: {request.sbpMerchantId}");
                Console.WriteLine($"Amount: {request.amount}");

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PostAsync("https://pay-test.raif.ru/api/sbp/v2/qrs", content);

                Console.WriteLine("Request Headers:");
                foreach (var header in content.Headers)
                {
                    Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
                }
                Console.WriteLine("Request Content:");
                Console.WriteLine(await content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {
                    var qrData = await response.Content.ReadAsStringAsync();
                    var jsonContent = await response.Content.ReadAsStringAsync(); 
                    var qrDataJson = JsonDocument.Parse(qrData);
                    var qrId = qrDataJson.RootElement.GetProperty("qrId").GetString();
                    var order = new Order_
                    {
                        OrderNum = request.order, 
                        QrId = qrId 


                    };
                    _context.Order_.Add(order);

                    _context.SaveChanges();

                    return Ok(new { qrResponce = qrData });
                }
                else
                {
                    return BadRequest("Failed to create QR");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
 
    }
}
