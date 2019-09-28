using System.Collections.Generic;

namespace Bookshelf.Data.Model
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<BookTag> BookTags { get; set; }
    }
}