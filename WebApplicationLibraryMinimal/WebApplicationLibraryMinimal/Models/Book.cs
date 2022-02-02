using System;
using System.Collections.Generic;

namespace WebApplicationLibraryMinimal.Models
{
    public partial class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public long Isbn { get; set; }
        public DateTime PublishedDate { get; set; }
        public int AuthorId { get; set; }
        public short Price { get; set; }

        public virtual Author Author { get; set; } = null!;
    }
}
