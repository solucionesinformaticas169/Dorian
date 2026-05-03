namespace Dorian.Infrastructure.Persistence;

using Dorian.Modules.Training.Domain.Entities;

internal static class ExerciseCatalogSeed
{
    public static IReadOnlyCollection<ExerciseCatalogSeedItem> Items { get; } =
    [
        new("30000000-0000-0000-0000-000000000001", "Press de banca plano", "press-banca-plano", ExerciseMuscleGroup.Chest, ExerciseEquipment.Barbell, "Ejercicio base para desarrollar fuerza y masa en el pecho.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000002", "Press inclinado con mancuernas", "press-inclinado-mancuernas", ExerciseMuscleGroup.Chest, ExerciseEquipment.Dumbbells, "Activa la porcion superior del pecho con recorrido controlado.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000003", "Aperturas con mancuernas", "aperturas-mancuernas", ExerciseMuscleGroup.Chest, ExerciseEquipment.Dumbbells, "Aislamiento para estirar y contraer el pecho.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000004", "Fondos en paralelas", "fondos-paralelas", ExerciseMuscleGroup.Chest, ExerciseEquipment.Bodyweight, "Movimiento multiarticular para pecho y triceps.", ExerciseDifficulty.Advanced),
        new("30000000-0000-0000-0000-000000000005", "Jalon al pecho", "jalon-pecho", ExerciseMuscleGroup.Back, ExerciseEquipment.Cable, "Mejora la amplitud dorsal y la tecnica de traccion.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000006", "Remo con barra", "remo-barra", ExerciseMuscleGroup.Back, ExerciseEquipment.Barbell, "Desarrolla espalda media y control postural.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000007", "Remo sentado en polea", "remo-polea", ExerciseMuscleGroup.Back, ExerciseEquipment.Cable, "Fortalece la espalda con tension constante.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000008", "Peso muerto rumano", "peso-muerto-rumano", ExerciseMuscleGroup.Back, ExerciseEquipment.Barbell, "Trabaja cadena posterior y control lumbar.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000009", "Sentadilla libre", "sentadilla-libre", ExerciseMuscleGroup.Legs, ExerciseEquipment.Barbell, "Patron rey para fuerza y potencia del tren inferior.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000010", "Prensa de piernas", "prensa-piernas", ExerciseMuscleGroup.Legs, ExerciseEquipment.Machine, "Aumenta volumen de cuadriceps con buena estabilidad.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000011", "Zancadas caminando", "zancadas-caminando", ExerciseMuscleGroup.Legs, ExerciseEquipment.Dumbbells, "Ejercicio unilateral para piernas y estabilidad.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000012", "Curl femoral", "curl-femoral", ExerciseMuscleGroup.Legs, ExerciseEquipment.Machine, "Enfocado en isquiotibiales y control de rodilla.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000013", "Press militar", "press-militar", ExerciseMuscleGroup.Shoulders, ExerciseEquipment.Barbell, "Fortalece hombros y estabilidad del core.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000014", "Elevaciones laterales", "elevaciones-laterales", ExerciseMuscleGroup.Shoulders, ExerciseEquipment.Dumbbells, "Aisla el deltoide medio para amplitud visual.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000015", "Face pulls", "face-pulls", ExerciseMuscleGroup.Shoulders, ExerciseEquipment.Cable, "Mejora la salud escapular y deltoides posterior.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000016", "Arnold press", "arnold-press", ExerciseMuscleGroup.Shoulders, ExerciseEquipment.Dumbbells, "Combina empuje y rotacion para hombro completo.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000017", "Curl con barra", "curl-barra", ExerciseMuscleGroup.Biceps, ExerciseEquipment.Barbell, "Movimiento base para fuerza de biceps.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000018", "Curl martillo", "curl-martillo", ExerciseMuscleGroup.Biceps, ExerciseEquipment.Dumbbells, "Trabaja braquial y antebrazo.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000019", "Curl inclinado", "curl-inclinado", ExerciseMuscleGroup.Biceps, ExerciseEquipment.Dumbbells, "Mayor estiramiento para hipertrofia de biceps.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000020", "Curl en cable", "curl-cable", ExerciseMuscleGroup.Biceps, ExerciseEquipment.Cable, "Tension continua durante todo el recorrido.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000021", "Press frances", "press-frances", ExerciseMuscleGroup.Triceps, ExerciseEquipment.Barbell, "Aislamiento para la porcion larga del triceps.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000022", "Extensiones en polea", "extensiones-polea", ExerciseMuscleGroup.Triceps, ExerciseEquipment.Cable, "Movimiento controlado para congestion del triceps.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000023", "Fondos en banco", "fondos-banco", ExerciseMuscleGroup.Triceps, ExerciseEquipment.Bench, "Ejercicio accesible para iniciar trabajo de triceps.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000024", "Patada de triceps", "patada-triceps", ExerciseMuscleGroup.Triceps, ExerciseEquipment.Dumbbells, "Aislamiento ligero con enfoque tecnico.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000025", "Crunch en colchoneta", "crunch-colchoneta", ExerciseMuscleGroup.Abdomen, ExerciseEquipment.Mat, "Movimiento basico para abdomen superior.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000026", "Plancha frontal", "plancha-frontal", ExerciseMuscleGroup.Abdomen, ExerciseEquipment.Bodyweight, "Mejora estabilidad del core.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000027", "Elevaciones de piernas", "elevaciones-piernas", ExerciseMuscleGroup.Abdomen, ExerciseEquipment.Mat, "Trabaja abdomen inferior y control lumbo-pelvico.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000028", "Russian twist", "russian-twist", ExerciseMuscleGroup.Abdomen, ExerciseEquipment.Bodyweight, "Activa oblicuos y resistencia del core.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000029", "Hip thrust", "hip-thrust", ExerciseMuscleGroup.Glutes, ExerciseEquipment.Barbell, "Principal constructor de gluteo y potencia de cadera.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000030", "Puente de gluteos", "puente-gluteos", ExerciseMuscleGroup.Glutes, ExerciseEquipment.Bodyweight, "Activacion inicial y tecnica de extension de cadera.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000031", "Patada de gluteo en polea", "patada-gluteo-polea", ExerciseMuscleGroup.Glutes, ExerciseEquipment.Cable, "Aisla gluteo mayor con control.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000032", "Abduccion de cadera", "abduccion-cadera", ExerciseMuscleGroup.Glutes, ExerciseEquipment.Machine, "Enfasis en gluteo medio y estabilidad.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000033", "Caminata en cinta", "caminata-cinta", ExerciseMuscleGroup.Cardio, ExerciseEquipment.Treadmill, "Cardio de baja intensidad para gasto calorico sostenido.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000034", "Bicicleta estatica", "bicicleta-estatica", ExerciseMuscleGroup.Cardio, ExerciseEquipment.Bike, "Trabajo cardiovascular de impacto bajo.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000035", "Remo ergometro", "remo-ergometro", ExerciseMuscleGroup.Cardio, ExerciseEquipment.RowingMachine, "Cardio de cuerpo completo con enfoque tecnico.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000036", "Burpees", "burpees", ExerciseMuscleGroup.Cardio, ExerciseEquipment.Bodyweight, "Condicionamiento intenso de cuerpo completo.", ExerciseDifficulty.Advanced),
        new("30000000-0000-0000-0000-000000000037", "Flexiones", "flexiones", ExerciseMuscleGroup.FullBody, ExerciseEquipment.Bodyweight, "Ejercicio global de empuje y estabilidad.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000038", "Sentadilla goblet", "sentadilla-goblet", ExerciseMuscleGroup.FullBody, ExerciseEquipment.Dumbbells, "Movimiento funcional para tren inferior y core.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000039", "Kettlebell swing", "kettlebell-swing", ExerciseMuscleGroup.FullBody, ExerciseEquipment.Kettlebell, "Potencia, cardio y cadena posterior.", ExerciseDifficulty.Intermediate),
        new("30000000-0000-0000-0000-000000000040", "Mountain climbers", "mountain-climbers", ExerciseMuscleGroup.FullBody, ExerciseEquipment.Bodyweight, "Core dinamico con respuesta cardiovascular.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000041", "Step-up en banco", "step-up-banco", ExerciseMuscleGroup.Legs, ExerciseEquipment.Bench, "Trabajo unilateral y coordinacion.", ExerciseDifficulty.Beginner),
        new("30000000-0000-0000-0000-000000000042", "Pull over en polea", "pull-over-polea", ExerciseMuscleGroup.Back, ExerciseEquipment.Cable, "Aislamiento dorsal y control escapular.", ExerciseDifficulty.Intermediate)
    ];
}

internal sealed record ExerciseCatalogSeedItem(
    string Id,
    string Name,
    string Slug,
    ExerciseMuscleGroup MuscleGroup,
    ExerciseEquipment Equipment,
    string Description,
    ExerciseDifficulty Difficulty,
    string? VideoUrl = null,
    string? ImageUrl = null);
