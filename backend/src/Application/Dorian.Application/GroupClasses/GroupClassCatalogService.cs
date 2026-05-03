namespace Dorian.Application.GroupClasses;

using Dorian.Application.Abstractions.Errors;

public sealed class GroupClassCatalogService : IGroupClassCatalogService
{
    private static readonly IReadOnlyCollection<GroupClassCatalogItem> Catalog =
    [
        new(
            "boxfit",
            "Boxfit",
            "🥊",
            "Golpea el estres y mejora tu condicion fisica.",
            "CLASE GRUPAL DE BOXFIT",
            "DEFENSA PERSONAL",
            "Golpea fuerte, entrena con pasion. El box te da fuerza, agilidad y enfoque.",
            "¡Descubre tu mejor version con cada round!",
            ["MEJORA TUS REFLEJOS", "FORTALECE TUS MUSCULOS", "DISCIPLINA MENTAL"]),
        new(
            "crossfit",
            "Crossfit",
            "💪",
            "Desafia tus limites con fuerza e intensidad.",
            "CLASE GRUPAL DE CROSSFIT",
            "AUMENTA TU FUERZA",
            "Supera tus limites, entrena sin excusas. El CrossFit te reta con cada repeticion.",
            "¡Fuerza, resistencia y resultados reales!",
            ["FUERZA FUNCIONAL", "ALTA INTENSIDAD", "RESISTENCIA FISICA"]),
        new(
            "bailoterapia",
            "Bailoterapia",
            "💃",
            "¡Baila, sonrie y quema calorias!",
            "CLASE GRUPAL DE BAILOTERAPIA",
            "ACTIVA TU RITMO",
            "Baila, sonrie y transforma tu energia. Con bailoterapia ejercitas cuerpo y mente.",
            "¡Disfruta cada ritmo mientras te pones en forma!",
            ["SALUD CARDIOVASCULAR", "QUEMA CALORIAS", "BIENESTAR EMOCIONAL"]),
        new(
            "spinning",
            "Spinning",
            "🚴‍♀️",
            "Pedalea al ritmo de tu mejor version.",
            "CLASE GRUPAL DE SPINNING",
            "MEJOR RESISTENCIA",
            "Pedalea con fuerza, alcanza nuevas metas. El spinning te da energia y resistencia.",
            "¡Avanza, quema calorias y disfruta del viaje!",
            ["CARDIO INTENSO", "QUEMA GRASA", "PIERNAS FUERTES"])
    ];

    public Task<IReadOnlyCollection<GroupClassCatalogItem>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult(Catalog);

    public Task<GroupClassCatalogItem> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var item = Catalog.SingleOrDefault(x => x.Slug.Equals(slug.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? throw new NotFoundException("Group class not found.");
        return Task.FromResult(item);
    }
}
