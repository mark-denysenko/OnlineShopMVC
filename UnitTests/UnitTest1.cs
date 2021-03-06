﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using System.Collections.Generic;
using Domain.Entities;
using WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using WebUI.Models;
using WebUI.HtmlHelpers;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(m => m.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1" },
                new Book {BookId = 2, Name = "Book2" },
                new Book {BookId = 3, Name = "Book3" },
                new Book {BookId = 4, Name = "Book4" },
                new Book {BookId = 5, Name = "Book5" },
                new Book {BookId = 6, Name = "Book6" }
            });

            BooksController controller = new BooksController(mock.Object);
            controller.pageSize = 3;

            BooksListViewModel result = (BooksListViewModel)controller.List(null, 2).Model;

            List<Book> books = result.Books.ToList();
            Assert.IsTrue(books.Count == 3);
            Assert.AreEqual(books[0].Name, "Book4");
            Assert.AreEqual(books[1].Name, "Book5");
        }

        [TestMethod]
        public void Can_generate_page_links()
        {
            HtmlHelper myHelper = null;
            PagingInfo pagingIngo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingIngo, pageUrlDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                            result.ToString());
        }

        [TestMethod]
        public void Can_send_pagination_view_model()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1" },
                new Book {BookId = 2, Name = "Book2" },
                new Book {BookId = 3, Name = "Book3" },
                new Book {BookId = 4, Name = "Book4" },
                new Book {BookId = 5, Name = "Book5" }
            });

            BooksController controller = new BooksController(mock.Object);
            controller.pageSize = 3;

            BooksListViewModel result = (BooksListViewModel)controller.List(null, 2).Model;

            PagingInfo pagingInfo = result.PagingIngo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_filter_books()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1", Genre = "Genre1" },
                new Book {BookId = 2, Name = "Book2", Genre = "Genre2" },
                new Book {BookId = 3, Name = "Book3", Genre = "Genre1" },
                new Book {BookId = 4, Name = "Book4", Genre = "Genre3" },
                new Book {BookId = 5, Name = "Book5", Genre = "Genre2" }
            });

            BooksController controller = new BooksController(mock.Object);
            controller.pageSize = 5;

            List<Book> result = ((BooksListViewModel)controller.List("Genre2", 1).Model).Books.ToList();

            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Book2" && result[0].Genre == "Genre2");
            Assert.IsTrue(result[1].Name == "Book5" && result[1].Genre == "Genre2");
        }

        [TestMethod]
        public void Create_categories()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1", Genre = "Genre1" },
                new Book {BookId = 2, Name = "Book2", Genre = "Genre2" },
                new Book {BookId = 3, Name = "Book3", Genre = "Genre1" },
                new Book {BookId = 4, Name = "Book4", Genre = "Genre3" },
                new Book {BookId = 5, Name = "Book5", Genre = "Genre2" }
            });

            NavController controller = new NavController(mock.Object);

            List<string> result = ((IEnumerable<string>)controller.Menu().Model).ToList();

            Assert.AreEqual(result.Count(), 3);
            Assert.AreEqual(result[0], "Genre1");
            Assert.AreEqual(result[1], "Genre2");
            Assert.AreEqual(result[2], "Genre3");
        }

        [TestMethod]
        public void Indicates_selected_genre()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1", Genre = "Genre1" },
                new Book {BookId = 2, Name = "Book2", Genre = "Genre2" },
                new Book {BookId = 3, Name = "Book3", Genre = "Genre1" },
                new Book {BookId = 4, Name = "Book4", Genre = "Genre3" },
                new Book {BookId = 5, Name = "Book5", Genre = "Genre2" }
            });

            NavController controller = new NavController(mock.Object);

            string genreSelected = "Genre2";

            string result = controller.Menu(genreSelected).ViewBag.SelectedGenre;

            Assert.AreEqual(result, genreSelected);
        }

        [TestMethod]
        public void Count_specific_book_genre()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>();
            mock.Setup(b => b.Books).Returns(new List<Book>
            {
                new Book {BookId = 1, Name = "Book1", Genre = "Genre1" },
                new Book {BookId = 2, Name = "Book2", Genre = "Genre2" },
                new Book {BookId = 3, Name = "Book3", Genre = "Genre1" },
                new Book {BookId = 4, Name = "Book4", Genre = "Genre3" },
                new Book {BookId = 5, Name = "Book5", Genre = "Genre2" }
            });

            BooksController controller = new BooksController(mock.Object);

            int res1 = ((BooksListViewModel)controller.List("Genre1").Model).PagingIngo.TotalItems;
            int res2 = ((BooksListViewModel)controller.List("Genre2").Model).PagingIngo.TotalItems;
            int res3 = ((BooksListViewModel)controller.List("Genre3").Model).PagingIngo.TotalItems;
            int resAll = ((BooksListViewModel)controller.List(null).Model).PagingIngo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
