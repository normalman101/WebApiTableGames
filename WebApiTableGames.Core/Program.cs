/*
Задание.
Сделать WebAPI для получения, добавления, удаления и изменения списка настольных игр.
Модель данных должна содержать название игры, издатель и год выпуска.
При удалении данные не должны удаляться, а только помечаются на удаление и потом не показываются при запросе всех игр.
Данные должны храниться в json-файле.
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApiTableGames.Core;


const string version = "v1";
const string filePath = "tableGames.json";

var json = File.ReadAllText(filePath);
var tableGames = JsonConvert.DeserializeObject<List<TableGame>>(json);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/openapi/{version}.json", version); });
}

void UpdateFile()
{
    File.WriteAllText(filePath, JsonConvert.SerializeObject(tableGames, Formatting.Indented));
}

app.MapGet("/tableGames/list", () => tableGames);

app.MapPost("/tableGames/add", (TableGame tableGame) =>
{
    if (tableGame.Name.Length == 0 || tableGame.Publisher.Length == 0 || tableGame.ReleaseYear.Length == 0)
    {
        Results.NoContent();
        return;
    }

    tableGames.Add(tableGame with
    {
        Id = Guid.NewGuid(),
        IsDeleted = false
    });
    UpdateFile();
}).Produces(StatusCodes.Status204NoContent);

app.MapDelete("/tableGames/delete/id/{id}", (string id) =>
{
    var foundTableGame = tableGames.FirstOrDefault(game => game.Id.ToString() == id);

    if (foundTableGame == null) Results.NoContent();
    
    tableGames.RemoveAll(game => game.Id.ToString() == id);
    tableGames.Add(foundTableGame with { IsDeleted = true });
    UpdateFile();
}).Produces(StatusCodes.Status404NotFound);

app.MapPut("/tableGames/update/id/{id}", (string id, TableGame tableGame) =>
{
    if (tableGame.Name.Length == 0 && tableGame.Publisher.Length == 0 && tableGame.ReleaseYear.Length == 0)
    {
        Results.NoContent();
        return;
    }

    var foundTableGame = tableGames.FirstOrDefault(game => game.Id.ToString() == id);
    
    if (foundTableGame == null) Results.NoContent();
    
    tableGames.RemoveAll(game => game.Id.ToString() == id);
    tableGames.Add(foundTableGame with
    {
        Name = tableGame.Name,
        Publisher = tableGame.Publisher,
        ReleaseYear = tableGame.ReleaseYear
    });
    UpdateFile();
});

app.UseHttpsRedirection();

app.Run();