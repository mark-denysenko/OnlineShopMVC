using Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class BooksController : Controller
    {
        private IBookRepository repository;
        public int pageSize = 4;                    // количество товаров на одной странице

        public BooksController(IBookRepository repo)
        {
            repository = repo;
        }

        public ViewResult List(int page = 1)
        {
            BooksListViewModel model = new BooksListViewModel()
            {
                Books = repository.Books.
                OrderBy(b => b.BookId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize),
                PagingIngo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = repository.Books.Count()
                }
            };

            return View(model);
        }
    }
}