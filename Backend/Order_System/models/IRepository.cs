using Order_System.models.Login;
using Order_System.models.Order;
using Order_System.models.Products;

namespace Order_System.models
{
	public interface IRepository
	{
		// Generic methods for add, delete, update and save
		void Add<T>(T entity) where T : class;
		void Delete<T>(T entity) where T : class;
		void Update<T>(T entity) where T : class;
		Task<bool> SaveChangesAsync();

		// Product methods
		Task<Product[]> GetProductsAsync();
		Task<Product> GetProductAsync(int ProductId);

		// Order methods
		Task<OrderItems[]> GetOrdersAsync();
		Task<OrderItems> GetOrderAsync(int OrderId);
		Task<List<OrderItems>> GetOrdersByEmailAsync(string email);
		Task<OrderItems> GetOrderByIdAsync(int orderId);

		Task<ProductType[]> GetProductTypesAsync();
		Task<Brand[]> GetBrandsAsync();

		// User methods
		Task<UserModel> ViewProfileAsync(int UserId);
		Task<UserModel[]> GetUsersAsync();
	}
}
