namespace Order_System.models.Products
{
	public abstract class BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
		public bool IsActive { get; set; } = true;
		public bool IsDeleted { get; set; } = false;
	}
}
