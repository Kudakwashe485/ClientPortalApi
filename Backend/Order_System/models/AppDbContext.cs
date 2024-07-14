using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Order_System.models.Login;
using Order_System.models.Products;
using Order_System.models.Order;

namespace Order_System.models
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Brand> Brands { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<OrderItems> Orders { get; set; }
		public DbSet<ProductType> ProductTypes { get; set; }
		public DbSet<UserModel> Users { get; set; }
		public DbSet<Help> Helps { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)

		{
			modelBuilder.Entity<UserModel>().ToTable("Users");
			base.OnModelCreating(modelBuilder);

			//********************************************************************************************************* Create Seed Data For the  Brand Table: **********************************************************************************

			modelBuilder.Entity<Brand>().HasData(
				new Brand
				{
					BrandId = 1,
					Name = "Nike",
					Description = "Description for Brand A",
					DateCreated = DateTime.UtcNow,
					IsActive = true,
					IsDeleted = false
				},
				new Brand
				{
					BrandId = 2,
					Name = "Addidas",
					Description = "Description for Brand B",
					DateCreated = DateTime.UtcNow,
					IsActive = true,
					IsDeleted = false
				}
			);

			//********************************************************************************************************* Create Seed Data For the Product Type Table: **********************************************************************************
			modelBuilder.Entity<ProductType>().HasData(
				new ProductType
				{
					ProductTypeId = 1,
					Name = "Type A",
					Description = "Description for Type A",
					DateCreated = DateTime.UtcNow,
					IsActive = true,
					IsDeleted = false
				},
				new ProductType
				{
					ProductTypeId = 2,
					Name = "Type B",
					Description = "Description for Type B",
					DateCreated = DateTime.UtcNow,
					IsActive = true,
					IsDeleted = false
				}
			);

			//********************************************************************************************************* Create Seed Data For the  Product Table: **********************************************************************************

			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					ProductId = 1,
					Name = "Shoes",
					Description = "Description for Product A",
					Price = 100.0m,
					Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQdL0KNu2v9MiUhgZ_j49ySPGkYpwhzMvwGCA&s",
					BrandId = 1,
					ProductTypeId = 1,
					DateCreated = DateTime.UtcNow,
					IsActive = true,
					IsDeleted = false
				},
				new Product
				{
						ProductId = 2,
						Name = "Clothes",
						Description = "Description for Product A",
						Price = 100.0m,
						Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTpj4XP-Zgu2O4G0wi0WqRvldD2iXt0mRM4Eg&s",
						BrandId = 1,
						ProductTypeId = 1,
						DateCreated = DateTime.UtcNow,
						IsActive = true,
						IsDeleted = false
				},
				new Product
				{
					ProductId = 3,
					Name = "Shoes",
					Description = "Description for Product B",
					Price = 150.0m,
					Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSCRzvh3U0qZod7VYobVPJ7QXU3Q8JZ7KoFp9qaKG3cVM2kb1QUAvTuqMD_nKUtnHkujiM&usqp=CAU",
					BrandId = 2,
					ProductTypeId = 2,
					DateCreated = DateTime.UtcNow,
					IsActive = true,
					IsDeleted = false
				});
		}
	}
}
