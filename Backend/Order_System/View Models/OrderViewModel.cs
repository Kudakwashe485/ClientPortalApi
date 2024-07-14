using System.ComponentModel.DataAnnotations;

namespace Order_System.View_Models
{
	public class OrderViewModel
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[DataType(DataType.PhoneNumber)]
		[StringLength(10)]
		public string ContactNumber { get; set; }
		public decimal price { get; set; }
		public int producttype { get; set; }
		public int brand { get; set; }
		public string description { get; set; }
		public string name { get; set; }
	}
}
