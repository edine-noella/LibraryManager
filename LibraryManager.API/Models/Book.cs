using System.ComponentModel.DataAnnotations;

namespace LibraryManager.API.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Author is required")]
    [StringLength(50, ErrorMessage = "Author name cannot exceed 50 characters")]
    public string Author { get; set; }

    [Required(ErrorMessage = "ISBN is required")]
    public string ISBN { get; set; }

    public bool IsAvailable { get; set; } = true;
}