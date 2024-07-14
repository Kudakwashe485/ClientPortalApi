using System.ComponentModel.DataAnnotations;

namespace Order_System.models.Login
{
	public class RegisterModel
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string userName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[DataType(DataType.Password)]
		public string Confirmpassword { get; set; }
	}
}
