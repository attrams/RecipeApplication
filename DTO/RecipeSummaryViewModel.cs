using RecipeApplication.Models;

namespace RecipeApplication.DTO;

public class RecipeSummaryViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string TimeToCook { get; set; }
    public int NumberOfIngredients { get; set; }

    public static RecipeSummaryViewModel FromRecipe(Recipe recipe)
    {
        return new RecipeSummaryViewModel
        {
            Id = recipe.RecipeId,
            Name = recipe.Name,
            TimeToCook = $"{recipe.TimeToCook.Hours}hrs {recipe.TimeToCook.Minutes} mins"
        };
    }
}