using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly IOrderProcessor _orderProcessor;

        public CartController(IProductRepository repo, IOrderProcessor proc)
        {
            _repository = repo;
            _orderProcessor = proc;
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel { Cart = cart, ReturnUrl = returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        [HttpGet]
        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (!cart.Lines.Any())
            {
                ModelState.AddModelError("", "Sorry, your cart is empty.");
            }
            if (ModelState.IsValid)
            {
                _orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            var product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                cart.AddProduct(product, 1);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            var product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                cart.RemoveLine(product);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        //private Cart GetCart()
        //{
        //    var cart = (Cart) Session["Cart"];
        //    if (cart == null)
        //    {
        //        cart = new Cart();
        //        Session["Cart"] = cart;
        //    }

        //    return cart;
        //}

        #region Debug for cart session
        //public ViewResult Index(string returnUrl)
        //{
        //    return View(new CartIndexViewModel { Cart = GetCart(), ReturnUrl = returnUrl });
        //}

        //public PartialViewResult Summary()
        //{
        //    return PartialView(GetCart());
        //}

        //[HttpGet]
        //public ViewResult Checkout()
        //{
        //    return View(new ShippingDetails());
        //}

        //[HttpPost]
        //public ViewResult Checkout(ShippingDetails shippingDetails)
        //{
        //    var cart = GetCart();

        //    if (!cart.Lines.Any())
        //    {
        //        ModelState.AddModelError("", "Sorry, your cart is empty.");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        _orderProcessor.ProcessOrder(cart, shippingDetails);
        //        cart.Clear();
        //        return View("Completed");
        //    }
        //    else
        //    {
        //        return View(shippingDetails);
        //    }
        //}

        //public RedirectToRouteResult AddToCart(int productId, string returnUrl)
        //{
        //    var product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);
        //    var cart = GetCart();

        //    if (product != null)
        //    {
        //        cart.AddProduct(product, 1);
        //    }

        //    return RedirectToAction("Index", new { returnUrl });
        //}

        //public RedirectToRouteResult RemoveFromCart(int productId, string returnUrl)
        //{
        //    var product = _repository.Products.FirstOrDefault(p => p.ProductID == productId);
        //    var cart = GetCart();

        //    if (product != null)
        //    {
        //        cart.RemoveLine(product);
        //    }

        //    return RedirectToAction("Index", new { returnUrl });
        //}

        //private Cart GetCart()
        //{
        //    var cart = (Cart)Session["Cart"];
        //    if (cart == null)
        //    {
        //        cart = new Cart();
        //        Session["Cart"] = cart;
        //    }

        //    return cart;
        //} 
        #endregion

    }
}