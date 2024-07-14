using Microsoft.AspNetCore.Mvc;
using Order_System.models;
using Order_System.models.Products;

namespace Order_System.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : Controller
	{
		private readonly IRepository _repository;
		public ProductController(IRepository repository)
		{
			_repository = repository;
		}

		//**************************************************************************** Product Listing *******************************************************************************
		[HttpGet]
		[Route("ProductListing")]
		public async Task<ActionResult> ProductListing()
		{
			try
			{
				// Retrieve products from repository
				var results = await _repository.GetProductsAsync();

				// Select specific properties to return
				dynamic products = results.Select(p => new
				{
					p.ProductId,
					p.Price,
					ProductTypeName = p.ProductType.Name,
					BrandName = p.Brand.Name,
					p.Name,
					p.Description,
					p.DateCreated,
					p.DateModified,
					p.IsActive,
					p.IsDeleted,
					p.Image
				});

				// Return the products
				return Ok(products);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}
		}

		//**************************************************************************** Add Product *******************************************************************************
		[HttpPost, DisableRequestSizeLimit]
		[Route("AddProduct")]
		public async Task<IActionResult> AddProduct([FromForm] IFormCollection formData)
		{
			try
			{
				var formCollection = await Request.ReadFormAsync();
				var file = formCollection.Files.First();

				if (file.Length > 0)
				{
					using (var ms = new MemoryStream())
					{
						// Convert file to base64 string
						file.CopyTo(ms);
						var fileBytes = ms.ToArray();
						string base64 = Convert.ToBase64String(fileBytes);

						// Convert price and create new product object
						string price = formData["price"];
						decimal num = decimal.Parse(price.Replace(".", ","));

						var product = new Product
						{
							Price = num,
							Name = formData["name"],
							Description = formData["description"],
							BrandId = Convert.ToInt32(formData["brand"]),
							ProductTypeId = Convert.ToInt32(formData["producttype"]),
							Image = base64,
							DateCreated = DateTime.Now
						};

						// Add product to repository and save changes
						_repository.Add(product);
						await _repository.SaveChangesAsync();
					}

					// Return success response
					return Ok();
				}
				else
				{
					// Return bad request if file is empty
					return BadRequest();
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex}");
			}
		}

		//**************************************************************************** Brand *******************************************************************************
		[HttpGet]
		[Route("Brands")]
		public async Task<ActionResult> Brands()
		{
			try
			{
				// Retrieve brands from repository
				var results = await _repository.GetBrandsAsync();

				// Return the brands
				return Ok(results);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}
		}

		//**************************************************************************** Product Type *******************************************************************************
		[HttpGet]
		[Route("ProductTypes")]
		public async Task<ActionResult> ProductTypes()
		{
			try
			{
				// Retrieve product types from repository
				var results = await _repository.GetProductTypesAsync();

				// Return the product types
				return Ok(results);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}
		}

		//**************************************************************************** Edit Product *******************************************************************************
		[HttpPut]
		[Route("EditProduct/{productId}")]
		public async Task<IActionResult> EditProduct(int productId, [FromForm] IFormCollection formData)
		{
			try
			{
				// Retrieve the existing product by id
				var existingProduct = await _repository.GetProductAsync(productId);

				if (existingProduct == null)
				{
					// Return not found if product does not exist
					return NotFound($"The product with ID {productId} does not exist");
				}

				// Update product properties
				existingProduct.Name = formData["name"];
				existingProduct.Description = formData["description"];
				string priceString = formData["price"];
				existingProduct.Price = decimal.Parse(priceString.Replace(".", ","));
				existingProduct.BrandId = Convert.ToInt32(formData["brand"]);
				existingProduct.ProductTypeId = Convert.ToInt32(formData["producttype"]);

				var file = formData.Files.FirstOrDefault();
				if (file != null && file.Length > 0)
				{
					// Update product image if new file is provided
					using (var ms = new MemoryStream())
					{
						file.CopyTo(ms);
						var fileBytes = ms.ToArray();
						string base64 = Convert.ToBase64String(fileBytes);
						existingProduct.Image = base64;
					}
				}

				// Update product in repository and save changes
				_repository.Update(existingProduct);
				await _repository.SaveChangesAsync();

				// Return the updated product
				return Ok(existingProduct);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex}");
			}
		}

		//**************************************************************************** Delete Product *******************************************************************************
		[HttpDelete]
		[Route("DeleteProduct/{productId}")]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			try
			{
				// Retrieve the existing product by id
				var existingProduct = await _repository.GetProductAsync(productId);

				if (existingProduct == null)
				{
					// Return not found if product does not exist
					return NotFound($"The product does not exist");
				}

				// Delete the product from repository
				_repository.Delete(existingProduct);

				// Save changes and return the deleted product if successful
				if (await _repository.SaveChangesAsync())
				{
					return Ok(existingProduct);
				}
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
			}

			// Return bad request if delete operation fails
			return BadRequest("Your request is invalid");
		}

		//**************************************************************************** Get product by id *******************************************************************************
		[HttpGet]
		[Route("GetProduct/{productId}")]
		public async Task<IActionResult> GetProductAsync(int productId)
		{
			try
			{
				// Retrieve the product by id
				var result = await _repository.GetProductAsync(productId);

				if (result == null)
				{
					// Return not found if product does not exist
					return NotFound("Product does not exist. You need to create a product first");
				}

				// Return the product
				return Ok(result);
			}
			catch (Exception)
			{
				return StatusCode(500, "Internal Server Error. Please contact support");
			}
		}

	}
}
