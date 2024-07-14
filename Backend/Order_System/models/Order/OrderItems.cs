using Order_System.models.Products;
using System.ComponentModel.DataAnnotations;

namespace Order_System.models.Order
{
	public class OrderItems 
	{
		[Key]
		public int OrderId { get; set; }
		public required string OrderNumber { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
		public string? Address { get; set; }
		public string? productname { get; set; }

		[DataType(DataType.PhoneNumber)]
		[StringLength(10)]
		public string? ContactNumber { get; set; }

		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public int Total { get; set; }
		public string OrderStatus { get; set; } = "Pending";

	}
}
