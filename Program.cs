using Microsoft.EntityFrameworkCore;
using RecipeApplication.DTO;
using RecipeApplication.Models;
using RecipeApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Recipe App", Version = "v1" }));

/* 
    Connection String template in secrets/appsettings.json for MSSQL

    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=<DbName>;User Id=<Username>;Password=<Password>;TrustServerCertificate=True;"
    }

 */
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<RecipeService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var recipeGroup = app.MapGroup("").WithOpenApi().WithTags("Recipes");

recipeGroup.MapPost("/", async (CreateRecipeDTO createRecipe, RecipeService recipeService) =>
{
    var id = await recipeService.CreateRecipe(createRecipe);

    return Results.CreatedAtRoute("view-recipe", new { id });
})
.WithSummary("Create recipe")
.Produces(StatusCodes.Status201Created);

recipeGroup.MapGet("/", async (RecipeService recipeService) =>
{
    return await recipeService.GetRecipes();
})
.WithSummary("List recipes");

recipeGroup.MapGet("/{id}", async (int id, RecipeService recipeService) =>
{
    var recipe = await recipeService.GetRecipeDetail(id);

    return recipe is null ? Results.Problem(statusCode: 404) : Results.Ok(recipe);
})
.WithName("view-recipe")
.WithSummary("Get recipe")
.ProducesProblem(404)
.Produces<RecipeDetailViewModel>();

recipeGroup.MapPut("/{id}", async (int id, UpdateRecipeDTO UpdateRecipeDTO, RecipeService recipeService) =>
{
    if (await recipeService.IsAvailableForUpdate(id))
    {
        await recipeService.UpdateRecipe(UpdateRecipeDTO);
        return Results.NoContent();
    }

    return Results.Problem(statusCode: 404);
})
.WithSummary("Update recipe")
.ProducesProblem(404)
.Produces(204);

recipeGroup.MapDelete("/{id}", async (int id, RecipeService recipeService) =>
{
    await recipeService.DeleteRecipe(id);
    return Results.NoContent();
})
.WithSummary("Delete recipe")
.Produces(201);

app.Run();


