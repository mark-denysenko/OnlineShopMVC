using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Entities;
using System.Linq;
using Domain.Abstract;
using Moq;
using WebUI.Controllers;
using System.Web.Mvc;
using WebUI.Models;

namespace UnitTests
{
    /// <summary>
    /// Summary description for CartTests
    /// </summary>
    [TestClass]
    public class CartTests
    {
        public CartTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Can_add_new_lines()
        {
            Book book1 = new Book() { BookId = 1, Name = "Book1" };
            Book book2 = new Book() { BookId = 2, Name = "Book2" };

            Cart cart = new Cart();

            cart.AddCart(book1, 1);
            cart.AddCart(book2, 1);

            List<CartLine> result = cart.Lines.ToList();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].Book, book1);
            Assert.AreEqual(result[1].Book, book2);
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void Can_add_quntity_for_existing_lines()
        {
            Book book1 = new Book() { BookId = 1, Name = "Book1" };
            Book book2 = new Book() { BookId = 2, Name = "Book2" };

            Cart cart = new Cart();

            cart.AddCart(book1, 1);
            cart.AddCart(book2, 1);
            cart.AddCart(book1, 5);

            List<CartLine> result = cart.Lines.OrderBy(l => l.Book.BookId).ToList();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].Quantity, 6);
            Assert.AreEqual(result[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_remove_lines()
        {
            Book book1 = new Book() { BookId = 1, Name = "Book1" };
            Book book2 = new Book() { BookId = 2, Name = "Book2" };
            Book book3 = new Book() { BookId = 3, Name = "Book3" };

            Cart cart = new Cart();

            cart.AddCart(book1, 1);
            cart.AddCart(book2, 1);
            cart.AddCart(book1, 5);
            cart.AddCart(book3, 2);
            cart.RemoveLine(book2);

            Assert.AreEqual(cart.Lines.Where(l => l.Book == book2).Count(), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_cart_total()
        {
            Book book1 = new Book() { BookId = 1, Name = "Book1", Price = 10 };
            Book book2 = new Book() { BookId = 2, Name = "Book2", Price = 5 };

            Cart cart = new Cart();

            cart.AddCart(book1, 1);
            cart.AddCart(book2, 1);
            cart.AddCart(book1, 5);
            decimal result = cart.ComputeTotalValue();

            Assert.AreEqual(result, 65);
        }

        [TestMethod]
        public void Can_clear_contents()
        {
            Book book1 = new Book() { BookId = 1, Name = "Book1", Price = 10 };
            Book book2 = new Book() { BookId = 2, Name = "Book2", Price = 5 };

            Cart cart = new Cart();

            cart.AddCart(book1, 1);
            cart.AddCart(book2, 1);
            cart.AddCart(book1, 5);
            cart.Clear();

            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_add_to_cart()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book { BookId = 1, Name = "Book1", Genre = "Genre1" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            controller.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToList()[0].Book.BookId, 1);
        }

        // redirect on page of cart
        [TestMethod]
        public void Adding_book_to_cart_goes_to_cart_screen()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book { BookId = 1, Name = "Book1", Genre = "Genre1" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            RedirectToRouteResult result = controller.AddToCart(cart, 1, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_view_cart_contents()
        {
            Cart cart = new Cart();
            CartController target = new CartController(null, null);

            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_checkout_empty_cart()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();

            CartController controller = new CartController(null, mock.Object);

            ViewResult result = controller.Checkout(cart, shippingDetails);

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_checkout_invalid_cart()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddCart(new Book(), 1);

            CartController controller = new CartController(null, mock.Object);
            controller.ModelState.AddModelError("errorr", "error");

            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_checkout_and_submit_order()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddCart(new Book(), 1);

            CartController controller = new CartController(null, mock.Object);

            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());

            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
