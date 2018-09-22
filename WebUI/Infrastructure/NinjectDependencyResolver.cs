using Ninject.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Domain.Abstract;
using Moq;
using Domain.Entities;
using Domain.Concrete;

namespace WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver, IHaveKernel
    {
        private IKernel kernel;
        public IKernel Kernel
        {
            get
            {
                return kernel;
            }
        }

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        private void AddBindings()
        {
            // привязка данных
            /* имитированные данные
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(m => m.Books).Returns(new List<Book>
            {
                new Book {Name = "Book1", Author = "Author1", Price = 110},
                new Book {Name = "Book2", Author = "Author2", Price = 50},
                new Book {Name = "Book3", Author = "Author3", Price = 200},
            });
            kernel.Bind<IBookRepository>().ToConstant(mock.Object);
            */

            kernel.Bind<IBookRepository>().To<EFBookRepository>();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }
}