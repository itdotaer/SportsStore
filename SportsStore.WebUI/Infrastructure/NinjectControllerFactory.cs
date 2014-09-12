using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory:DefaultControllerFactory
    {
        public IKernel NinjectKernel;

        public NinjectControllerFactory()
        {
            NinjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController) NinjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            //Mock data
            ////put bindings here
            //var mock = new Mock<IProductRepository>();
            //mock.Setup(m => m.Products).Returns(new List<Product>
            //{
            //    new Product{Name = "Football", Price = 25},
            //    new Product{Name = "Surf board", Price = 179},
            //    new Product{Name = "Running shoes", Price = 95},
            //}.AsQueryable());

            ////What's the function?
            //ninjectKernel.Bind<IProductRepository>().ToConstant(mock.Object);

            NinjectKernel.Bind<IProductRepository>().To<EFProductRepository>();

            NinjectKernel.Bind<IAuthProvider>().To<AuthProvider>();

            var emailSettings = new EmailSettings { WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")};

            NinjectKernel.Bind<IOrderProcessor>()
                .To<OrderProcessor>()
                .WithConstructorArgument("settings", emailSettings);
        }
    }
}