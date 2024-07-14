using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order_System.models.Login;
using Order_System.models.Products;
using Order_System.models.Order;
using System.Linq;
using System.Threading.Tasks;

namespace Order_System.models
{
	public class Repository : IRepository
	{
		private readonly AppDbContext _appDbContext;

		public Repository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		// Generic methods for adding, deleting, and updating entities
		public void Add<T>(T entity) where T : class
		{
			_appDbContext.Set<T>().Add(entity);
		}

		public void Delete<T>(T entity) where T : class
		{
			_appDbContext.Set<T>().Remove(entity);
		}

		public void Update<T>(T entity) where T : class
		{
			_appDbContext.Set<T>().Update(entity);
		}

		public async Task<bool> SaveChangesAsync()
		{
			return (await _appDbContext.SaveChangesAsync()) > 0;
		}

		// User methods

		public async Task<UserModel[]> GetUsersAsync()
		{
			IQueryable<UserModel> query = _appDbContext.Users;
			return await query.ToArrayAsync();
		}

		public async Task<UserModel> ViewProfileAsync(int UserId)
		{
			IQueryable<UserModel> query = _appDbContext.Users.Where(u => u.Id == UserId);
			return await query.FirstOrDefaultAsync();
		}

		// Product methods
		public async Task<Product[]> GetProductsAsync()
		{
			IQueryable<Product> query = _appDbContext.Products.Include(p => p.Brand).Include(p => p.ProductType);

			return await query.ToArrayAsync();
		}

		public async Task<Product> GetProductAsync(int productId)
		{
			IQueryable<Product> query = _appDbContext.Products.Where(d => d.ProductId == productId);
			return await query.FirstOrDefaultAsync();
		}

		// Order methods
		public async Task<OrderItems[]> GetOrdersAsync()
		{
			IQueryable<OrderItems> query = _appDbContext.Orders;

			return await query.ToArrayAsync();
		}

		public async Task<OrderItems> GetOrderAsync(int productId)
		{
			IQueryable<OrderItems> query = _appDbContext.Orders.Where(d => d.OrderId == productId);
			return await query.FirstOrDefaultAsync();
		}

		public async Task<List<OrderItems>> GetOrdersByEmailAsync(string email)
		{
			return await _appDbContext.Orders
				.Where(o => o.Email == email)
				.ToListAsync();
		}

		public async Task<OrderItems> GetOrderByIdAsync(int orderId)
		{
			return await _appDbContext.Orders.FindAsync(orderId);
		}

		// Methods for brands and product types
		public async Task<Brand[]> GetBrandsAsync()
		{
			IQueryable<Brand> query = _appDbContext.Brands;

			return await query.ToArrayAsync();
		}

		public async Task<ProductType[]> GetProductTypesAsync()
		{
			IQueryable<ProductType> query = _appDbContext.ProductTypes;

			return await query.ToArrayAsync();
		}
	}



}
