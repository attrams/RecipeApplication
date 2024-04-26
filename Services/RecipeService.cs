using Microsoft.EntityFrameworkCore;
using RecipeApplication.DTO;
using RecipeApplication.Models;

namespace RecipeApplication.Services;

public class RecipeService
{
    readonly AppDbContext _context;
    readonly ILogger<RecipeService> _logger;

    public RecipeService(AppDbContext context, ILogger<RecipeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateRecipe(CreateRecipeDTO createRecipe)
    {
        var recipe = createRecipe.ToRecipe();
        _context.Add(recipe);
        await _context.SaveChangesAsync();

        return recipe.RecipeId;
    }

    public async Task<List<RecipeSummaryViewModel>> GetRecipes()
    {
        var recipes = await _context.Recipes.Where(recipe => !recipe.IsDeleted).Select(recipe => new RecipeSummaryViewModel
        {
            Id = recipe.RecipeId,
            Name = recipe.Name,
            TimeToCook = $"{recipe.TimeToCook.Hours}hrs {recipe.TimeToCook.Minutes}mins",
            NumberOfIngredients = recipe.Ingredients.Count()
        }).ToListAsync();

        return recipes;
    }

    public async Task<RecipeDetailViewModel?> GetRecipeDetail(int id)
    {
        var recipe = await _context.Recipes.Where(recipe => recipe.RecipeId == id).Where(recipe => !recipe.IsDeleted).Select(
            recipe => new RecipeDetailViewModel
            {
                Id = recipe.RecipeId,
                Name = recipe.Name,
                Method = recipe.Method,
                Ingredients = recipe.Ingredients.Select(ingredient => new RecipeDetailViewModel.Item
                {
                    Name = ingredient.Name,
                    Quantity = $"{ingredient.Quantity} {ingredient.Unit}"
                }),

            }).SingleOrDefaultAsync();

        return recipe;
    }

    public async Task<bool> IsAvailableForUpdate(int recipeId)
    {
        return await _context.Recipes.Where(recipe => recipe.RecipeId == recipeId).Where(recipe => !recipe.IsDeleted).AnyAsync();
    }

    public async Task UpdateRecipe(UpdateRecipeDTO updateRecipeDTO)
    {
        var recipe = await _context.Recipes.FindAsync(updateRecipeDTO.Id) ?? throw new Exception("Unable to find the recipe");
        if (recipe.IsDeleted)
        {
            throw new Exception("Unable to update a deleted recipe");
        }
        updateRecipeDTO.UpdateRecipe(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRecipe(int recipeId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);

        if (recipe is not null)
        {
            recipe.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}