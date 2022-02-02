using System;
using System.Collections.Generic;

namespace WebApplicationLibraryMinimal.Models
{
    public partial class Author
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }

        public int AuthorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }
}
