using RecipeApplication.Models;

namespace RecipeApplication.DTO;

public class CreateRecipeDTO : RecipeDTO
{
    public IList<CreateIngredientDTO> Ingredients { get; set; } = new List<CreateIngredientDTO>();

    public Recipe ToRecipe()
    {
        return new Recipe
        {
            Name = Name,
            TimeToCook = new TimeSpan(TimeToCookHrs, TimeToCookMins, 0),
            Method = Method,
            IsVegetarian = IsVegetarian,
            IsVegan = IsVegan,
            Ingredients = Ingredients.Select(ingredient => ingredient.ToIngredient()).ToList()
        };
    }
}