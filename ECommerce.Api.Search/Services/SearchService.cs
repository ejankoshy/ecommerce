
using ECommerce.Api.Search.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductService productService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService, IProductService productService, ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productService = productService;
            this.customersService = customersService;
        }
        public async System.Threading.Tasks.Task<(bool IsSuccess, dynamic SearchResult)> SearchAsync(int customerId)
        {
            var customerResult = await customersService.GetCustomerAsync(customerId);
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productService.GetProductsAsync();
            if (ordersResult.IsSuccess)
            {
                foreach (var order in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        var productName = productsResult.IsSuccess ? productsResult.Products.FirstOrDefault(pro => pro.Id== item.ProductId).Name :
                            "Product information is not available";
                        item.ProductName = productName;
                    }
                }
                var result = new
                {
                    Customer = customerResult.IsSuccess ? customerResult.Customer : new { Name = "Customer information is not available." },
                    ordersResult.Orders
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
