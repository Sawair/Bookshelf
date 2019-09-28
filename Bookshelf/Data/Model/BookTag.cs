namespace Bookshelf.Data.Model
{
    public class BookTag
    {
        public int Id { get; set; }
        public Tag Tag { get; set; }
        public Book Book { get; set; }
    }
}