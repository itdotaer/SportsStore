using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository _repository;

        public AdminController(IProductRepository repo)
        {
            _repository = repo;
        }

        public ViewResult Index()
        {
            return View(_repository.Products);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpGet]
        public ViewResult Edit(int productId)
        {
            var product = _repository.Products.FirstOrDefault(prod => prod.ProductID == productId);

            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _repository.SaveProduct(product);

                TempData["message"] = string.Format("{0} is added successed.", product.Name);

                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        [HttpPost]
        public RedirectToRouteResult Delete(int productId)
        {
            var returnProd = _repository.DeleteProduct(productId);
            if (returnProd != null)
            {
                TempData["message"] = string.Format("{0} was deleted", returnProd.Name);
            }

            return RedirectToAction("Index");
        }

    }
}