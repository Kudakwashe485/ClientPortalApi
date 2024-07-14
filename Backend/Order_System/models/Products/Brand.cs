namespace Order_System.models.Products
{
	public class Brand: BaseEntity
	{
		public int BrandId { get; set; }
		public virtual ICollection<Product> Products { get; set; }
	}
}
