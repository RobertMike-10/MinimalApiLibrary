using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationLibraryMinimal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LibraryContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();


app.MapGet("/books", async (LibraryContext db) =>
 await db.Books.ToListAsync()
).Produces<List<Book>>(StatusCodes.Status200OK)
.WithName("GetAllBooks").WithTags("Getters");

app.MapGet("/books/{id}", async (LibraryContext db, int id) =>
await db.Books.SingleOrDefaultAsync(s => s.BookId == id) is Book mybook ? 
                                                            Results.Ok(mybook) : Results.NotFound()
)
.Produces<Book>(StatusCodes.Status200OK)
.WithName("GetBookbyID").WithTags("Getters");

app.MapGet("/books/search/{query}",
(string query, LibraryContext db) =>
{
    var _selectedBooks = db.Books.Where(x => x.Title.ToLower().Contains(query.ToLower())).ToList();
    return _selectedBooks.Count > 0 ? Results.Ok(_selectedBooks) :
                                      Results.NotFound(Array.Empty<Book>());
})
.Produces<List<Book>>(StatusCodes.Status200OK)
.WithName("Search").WithTags("Getters");


//POST
app.MapPost("/books",
async ([FromBody] Book addbook, [FromServices] LibraryContext db, HttpResponse response) =>
{
    db.Books.Add(addbook);
    await db.SaveChangesAsync();
    response.StatusCode = 200;
    response.Headers.Location = $"books/{ addbook.BookId}";
})
.Accepts<Book>("application/json")
.Produces<Book>(StatusCodes.Status201Created)
.WithName("AddNewBook").WithTags("Setters");


//PUT
app.MapPut("/books/{bookId}",
[AllowAnonymous] async (int bookId, [FromBody] Book book,  [FromServices] LibraryContext db, HttpResponse response) =>
{
    var mybook = db.Books.SingleOrDefault(s => s.BookId == bookId);
    if (mybook == null) return Results.NotFound();
    mybook.Title = book.Title;
    mybook.Isbn = book.Isbn;
    mybook.PublishedDate = book.PublishedDate;
    mybook.Year = book.Year;
    mybook.Price = book.Price;
    await db.SaveChangesAsync();
    return Results.Created("/books", mybook);
})
.Produces<Book>(StatusCodes.Status201Created).Produces(StatusCodes.Status404NotFound)
.WithName("UpdateBook").WithTags("Setters");

app.MapDelete("/books/{id}", async (LibraryContext db, int id,  HttpResponse response) =>
 {
     var mybook = await db.Books.SingleOrDefaultAsync(s => s.BookId == id);
     if (mybook is not null)
     {
         db.Books.Remove(mybook);
         await db.SaveChangesAsync();
         response.StatusCode = 200;
     }
     else Results.NotFound();
 }
)
.Produces<Book>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound)
.WithName("DeleteBook").WithTags("Setters");
app.Run();

