namespace WebApiTableGames.Core;

public record TableGame
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Publisher { get; init; }
    public string ReleaseYear { get; init; }
    public bool IsDeleted { get; init; }
}