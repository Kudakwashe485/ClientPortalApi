using System.ComponentModel.DataAnnotations;

namespace Order_System.models.Login
{
    public class LoginModel
    {
		[Required]
		[DataType(DataType.EmailAddress)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
