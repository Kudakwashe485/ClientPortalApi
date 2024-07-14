using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_System.models;
using Order_System.models.Order;
using Order_System.models.Products;
using System.Security.Claims;

namespace Order_System.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : Controller
	{
		private readonly IRepository _repository;
		public OrderController(IRepository repository)
		{
			_repository = repository;
		}

		//**************************************************************************** Make Order *******************************************************************************
		[HttpPost, DisableRequestSizeLimit]
		[Route("AddOrder")]
		public async Task<IActionResult> AddOrder([FromForm] IFormCollection formData)
		{
			try
			{
				// Convert StringValues to string before calling Replace
				string priceStr = formData["price"].ToString();

				// Validate and parse the price
				if (!decimal.TryParse(priceStr.Replace(".", ","), out var price))
				{
					return BadRequest("Invalid price format.");
				}

				// Parse other form data
				if (!int.TryParse(formData["total"], out var total) ||
					!int.TryParse(formData["quantity"], out var quantity))
				{
					return BadRequest("Invalid format for total or quantity.");
				}

				// Generate a 6-digit order number
				var random = new Random();
				var orderNumber = random.Next(100000, 999999).ToString();

				// Create a new OrderItems object with the provided form data
				var order = new OrderItems
				{
					OrderNumber = orderNumber,
					Price = price,
					productname = formData["productname"],
					Address = formData["address"],
					FirstName = formData["firstname"],
					LastName = formData["lastname"],
					Email = formData["email"],
					ContactNumber = formData["contactNumber"],
					Quantity = quantity,
					Total = total,
					OrderStatus = "Pending"
				};

				// Add the new order to the repository and save changes
				_repository.Add(order);
				await _repository.SaveChangesAsync();

				// Return a success response
				return Ok(new { OrderNumber = orderNumber });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		//**************************************************************************** Get order by email *******************************************************************************
		[HttpGet("GetOrderInfor/{email}")]
		public async Task<IActionResult> GetOrderInfor(string email)
		{
			try
			{
				// Get the orders through the method with the user's email
				var result = await _repository.GetOrdersByEmailAsync(email);

				// If no orders found, return a not found response
				if (result == null || result.Count == 0)
					return NotFound("Order does not exist. You need to create a booking first");

				var mappedResult = result.Select(o => new
				{
					o.OrderId,
					o.OrderNumber,
					o.FirstName,
					o.LastName,
					o.Email,
					o.ContactNumber,
					o.Price,
					o.OrderStatus,
					o.Address,
					o.Quantity,
					o.productname,
					o.Total
				});

				// Return the orders
				return Ok(mappedResult);
			}
			catch (Exception)
			{
				return StatusCode(500, "Internal Server Error. Please contact support");
			}
		}

		//**************************************************************************** Order Listing *******************************************************************************
		[HttpGet]
		[Route("OrderListing")]
		public async Task<ActionResult> OrderListing()
		{
			try
			{
				// Retrieve orders from repository
				var results = await _repository.GetOrdersAsync();

				dynamic products = results.Select(o => new
				{
					o.OrderId,
					o.OrderNumber,
					o.FirstName,
					o.LastName,
					o.Email,
					o.ContactNumber,
					o.Price,
					o.OrderStatus,
					o.Address,
					o.Quantity,
					o.productname,
					o.Total
				});

				// Return the orders
				return Ok(products);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}
		}

		//**************************************************************************** Update Status *******************************************************************************
		[Authorize(Roles = "Vendor")]
		[HttpPut("UpdateOrderStatus/{orderId}")]
		public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string status)
		{
			try
			{
				// Get the order by ID
				var order = await _repository.GetOrderByIdAsync(orderId);

				// If order not found, return a not found response
				if (order == null)
				{
					return NotFound("Order not found.");
				}

				// Update the order status
				order.OrderStatus = status;

				// Save the changes to the repository
				_repository.Update(order);
				await _repository.SaveChangesAsync();

				// Return a success response
				return Ok("Order status updated.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

	}
}
