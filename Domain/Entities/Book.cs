using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Book
    {
        public int BookId { set; get; }
        public string Name { set; get; }
        public string Author { set; get; }
        public string Description { set; get; }
        public string Genre { set; get; }
        public decimal Price { set; get; }
    }
}
