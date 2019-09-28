using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshelf.ViewModel
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsEbook { get; set; }
        public bool Available { get; set; }
        public List<string> Tags { get; set; }
        public string Borrower { get; set; }
    }
}
