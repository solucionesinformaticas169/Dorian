namespace Dorian.Application.GroupClasses;

public sealed record GroupClassCatalogItem(
    string Slug,
    string Name,
    string Emoji,
    string Summary,
    string Type,
    string Subtitle,
    string Description,
    string Tagline,
    IReadOnlyCollection<string> Benefits);
