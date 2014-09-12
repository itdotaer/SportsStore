using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using Moq;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class Products
    {

        [TestMethod]
        public void CanSendPaginationViewModel()
        {
            //arrange
            var target = new Mock<IProductRepository>();

            target.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "p1"},
                new Product{ProductID = 2, Name = "p2"},
                new Product{ProductID = 3, Name = "p3"},
                new Product{ProductID = 4, Name = "p4"},
                new Product{ProductID = 5, Name = "p5"},
            }.AsQueryable());

            //arrange
            var productController = new ProductController(target.Object);
            productController.PageSize = 3;

            //act
            var result = (ProductsListViewModel) productController.List(null, 2).Model;

            //assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CanPaginate()
        {
            //arrange
            var target = new Mock<IProductRepository>();

            target.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "p1"},
                new Product{ProductID = 2, Name = "p2"},
                new Product{ProductID = 3, Name = "p3"},
                new Product{ProductID = 4, Name = "p4"},
                new Product{ProductID = 5, Name = "p5"},
            }.AsQueryable());

            //arrange
            var productController = new ProductController(target.Object);
            productController.PageSize = 3;

            //act
            var result = (ProductsListViewModel)productController.List(null, 2).Model;

            //assert
            var prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "p4");
            Assert.AreEqual(prodArray[1].Name, "p5");
        }

        [TestMethod]
        public void CanGeratePageLinks()
        {
            //arrange
            HtmlHelper myHelper = null;

            //arrange
            var pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            //arrange
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //act
            var result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a><a class=""selected"" href=""Page2"">2</a><a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void CanFilterProducts()
        {
            //arrange
            var target = new Mock<IProductRepository>();
            target.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "p1", Category = "cat1"},
                new Product{ProductID = 2, Name = "p2", Category = "cat2"},
                new Product{ProductID = 3, Name = "p3", Category = "cat1"},
                new Product{ProductID = 4, Name = "p4", Category = "cat2"},
                new Product{ProductID = 5, Name = "p5", Category = "cat3"},
            }.AsQueryable());

            //arrange
            var productController = new ProductController(target.Object);
            productController.PageSize = 3;
            
            //act
            var result = (ProductsListViewModel)productController.List("cat1", 1).Model;

            //assert
            var prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray[0].Category == "cat1" && prodArray[0].Name == "p1");
            Assert.IsTrue(prodArray[1].Category == "cat1" && prodArray[1].Name == "p3");
        }

        [TestMethod]
        public void CanCreateMenuByCategories()
        {
            //arrange
            var target = new Mock<IProductRepository>();
            target.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "p1", Category = "cat1"},
                new Product{ProductID = 2, Name = "p2", Category = "cat2"},
                new Product{ProductID = 3, Name = "p3", Category = "cat1"},
                new Product{ProductID = 4, Name = "p4", Category = "cat2"},
                new Product{ProductID = 5, Name = "p5", Category = "cat3"},
            }.AsQueryable());

            //arrange
            var navController = new NavController(target.Object);

            //act
            var result = (IEnumerable<string>)navController.Menu().Model;

            //assert
            var menuArray = result.ToArray();
            Assert.AreEqual(menuArray[0], "cat1");
            Assert.AreEqual(menuArray[1], "cat2");
            Assert.AreEqual(menuArray[2], "cat3");
        }

        [TestMethod]
        public void IndicatesSelectedCategory()
        {
            //arrange
            var target = new Mock<IProductRepository>();
            target.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "p1", Category = "Apples"},
                new Product {ProductID = 2, Name = "p2", Category = "Oranges"}
            }.AsQueryable());

            //arrange
            var navController = new NavController(target.Object);
            var categoryToSelect = "Apples";

            //act
            var result = navController.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void GenerateCategorySpecificProductCount()
        {
            //arrange
            var target = new Mock<IProductRepository>();
            target.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "p1", Category = "cat1"},
                new Product{ProductID = 2, Name = "p2", Category = "cat2"},
                new Product{ProductID = 3, Name = "p3", Category = "cat1"},
                new Product{ProductID = 4, Name = "p4", Category = "cat2"},
                new Product{ProductID = 5, Name = "p5", Category = "cat3"},
            }.AsQueryable());

            //arrange
            var productController = new ProductController(target.Object);

            //act
            var result = (ProductsListViewModel)productController.List("cat1").Model;
            var pagingInfo = result.PagingInfo;

            //assert
            Assert.AreEqual(2, pagingInfo.TotalItems);
        }

        [TestMethod]
        public void CanAddNewLies()
        {
            //arrange
            var prod1 = new Product { ProductID = 1, Name = "p1" };
            var prod2 = new Product { ProductID = 2, Name = "p2" };

            //arrange
            var target = new Cart();

            //act
            target.AddProduct(prod1, 1);
            target.AddProduct(prod2, 2);
            var results = target.Lines.OrderBy(l => l.Product.ProductID);

            //assert
            var linesArray = results.ToArray();
            Assert.AreEqual(2, linesArray.Length);
            Assert.AreEqual(1, linesArray[0].Product.ProductID);
            Assert.AreEqual(2, linesArray[1].Product.ProductID);
            Assert.AreEqual(1, linesArray[0].Quantity);
            Assert.AreEqual(2, linesArray[1].Quantity);

        }

        [TestMethod]
        public void CanAddQuantityForExistingLines()
        {
            //arrange
            var prod1 = new Product {ProductID = 1, Name = "p1"};
            var prod2 = new Product { ProductID = 2, Name = "p2" };
            var prod3 = new Product { ProductID = 3, Name = "p3" };

            //arrange
            var target = new Cart();

            //act
            target.AddProduct(prod1, 10);
            target.AddProduct(prod1, 1);
            target.AddProduct(prod2, 1);
            target.AddProduct(prod3, 2);
            var results = target.Lines.OrderBy(l => l.Product.ProductID);

            //assert
            var linesArray = results.ToArray();
            Assert.AreEqual(linesArray.Length, 3);
            Assert.AreEqual(linesArray[0].Product.ProductID, 1);
            Assert.AreEqual(linesArray[0].Quantity, 11);
            Assert.AreEqual(linesArray[1].Quantity, 1);
            Assert.AreEqual(linesArray[2].Quantity, 2);
        }

        [TestMethod]
        public void CanRemoveLine()
        {
            //arrange
            var prod1 = new Product { ProductID = 1, Name = "p1" };
            var prod2 = new Product { ProductID = 2, Name = "p2" };
            var prod3 = new Product { ProductID = 3, Name = "p3" };

            //array
            var target = new Cart();

            //arrange
            target.AddProduct(prod1, 1);
            target.AddProduct(prod2, 3);
            target.AddProduct(prod3, 5);
            target.AddProduct(prod2, 1);

            //act
            target.RemoveLine(prod2);

            //assert
            Assert.AreEqual(0, target.Lines.Count(l => l.Product.ProductID == prod2.ProductID));
            Assert.AreEqual(2, target.Lines.Count());
        }

        [TestMethod]
        public void CalculateCartTotal()
        {
            //arrange
            var prod1 = new Product { ProductID = 1, Name = "p1", Price = 100M};
            var prod2 = new Product { ProductID = 2, Name = "p2", Price = 50M};

            //arrange
            var target = new Cart();

            //act
            target.AddProduct(prod1, 2);
            target.AddProduct(prod1, 8);
            target.AddProduct(prod2, 1);
            var result = target.CoputeTotalValue();

            //assert
            Assert.AreEqual(1050M, result);
        }

        [TestMethod]
        public void CanClearContents()
        {
            //array
            var prod1 = new Product { ProductID = 1, Name = "p1", Price = 100M };
            var prod2 = new Product { ProductID = 2, Name = "p2", Price = 50M };

            //arrange
            var target = new Cart();
            target.AddProduct(prod1, 2);
            target.AddProduct(prod1, 8);
            target.AddProduct(prod2, 1);

            //act
            target.Clear();

            //assert
            Assert.AreEqual(0, target.Lines.Count());
        }

        [TestMethod]
        public void CanAddToCart()
        {
            //arrange
            var prod1 = new Product { ProductID = 1, Name = "p1", Price = 100M };
            var prod2 = new Product { ProductID = 2, Name = "p2", Price = 50M };
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {prod1, prod2}.AsQueryable());

            //arrange
            var cart = new Cart();
            var target = new CartController(mock.Object, null);

            //act
            target.AddToCart(cart, prod1.ProductID, null);

            //assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(1, cart.Lines.ElementAt(0).Product.ProductID);
        }

        [TestMethod]
        public void AddingProductToCartGoesToCartScreen()
        {
            //arrange
            var prod1 = new Product { ProductID = 1, Name = "p1", Price = 100M };
            var prod2 = new Product { ProductID = 2, Name = "p2", Price = 50M };
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] { prod1, prod2 }.AsQueryable());

            //arrange
            var cart = new Cart();
            var target = new CartController(mock.Object, null);

            //act
            var result = target.AddToCart(cart, prod1.ProductID, "myUrl");

            //assert
            Assert.AreEqual("myUrl", result.RouteValues["returnUrl"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void CanViewCartContents()
        {
            //arrange
            var cart = new Cart();

            //arrange
            var target = new CartController(null, null);

            //act
            var result = (CartIndexViewModel) target.Index(cart, "myUrl").ViewData.Model;

            //assert
            Assert.AreEqual(cart, result.Cart);
            Assert.AreEqual("myUrl", result.ReturnUrl);
        }
    }
}
