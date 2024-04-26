using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecipeApplication.DTO;

public class RecipeDTO
{
    [Required, StringLength(100)]
    public required string Name { get; set; }

    [Range(0, 23), DisplayName("Time to cook (hrs)")]
    public int TimeToCookHrs { get; set; }

    [Range(0, 59), DisplayName("Time to cook (mins)")]
    public int TimeToCookMins { get; set; }

    [Required]
    public required string Method { get; set; }

    [DisplayName("Vegetarian?")]
    public bool IsVegetarian { get; set; }

    [DisplayName("Vegan?")]
    public bool IsVegan { get; set; }
}