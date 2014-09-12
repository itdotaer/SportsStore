using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        public int PageSize = 2;

        public ProductController(IProductRepository productRepository)
        {
            _repository = productRepository;
        }

        public ViewResult List(string category, int page = 1)
        {
            //ViewBag.CurrentPage = page;
            //return View(repository.Products.OrderBy(p => p.ProductID).Skip((page -1) * PageSize).Take(PageSize));

            var model = new ProductsListViewModel
            {
                Products = _repository.Products.Where(p => category == null || p.Category == category).OrderBy(p => p.ProductID).Skip((page - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? _repository.Products.Count():
                    _repository.Products.Count(e => e.Category == category)
                },
                CurrentCategory = category
            };

            return View(model);
        }
    }
}