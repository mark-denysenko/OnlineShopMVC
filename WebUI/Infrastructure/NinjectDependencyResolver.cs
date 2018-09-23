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
            ///* имитированные данные (imitation of info)
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(m => m.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1", Author = "Author1", Price = 110, Genre = "Genre1", Description = "Info1"},
                new Book {BookId = 2, Name = "Book2", Author = "Author2", Price = 50, Genre = "Genre2", Description = "Info2"},
                new Book {BookId = 3, Name = "Book3", Author = "Author3", Price = 200, Genre = "Genre3", Description = "Info3"},
                new Book {BookId = 4, Name = "Book4", Author = "Author4", Price = 30.50m, Genre = "Genre1", Description = "Info4"},
                new Book {BookId = 5, Name = "Book5", Author = "Author5", Price = 25.40m, Genre = "Genre2", Description = "Info5"},
            });
            kernel.Bind<IBookRepository>().ToConstant(mock.Object);
            //*/

            // Taking info from Db (use in developing)
            //kernel.Bind<IBookRepository>().To<EFBookRepository>();
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