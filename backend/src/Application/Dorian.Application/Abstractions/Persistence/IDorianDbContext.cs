namespace Dorian.Application.Abstractions.Persistence;

using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Auditing.Domain.Entities;
using Dorian.Modules.Branches.Domain.Entities;
using Dorian.Modules.Classes.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Entities;
using Dorian.Modules.Memberships.Domain.Entities;
using Dorian.Modules.Nutrition.Domain.Entities;
using Dorian.Modules.Promotions.Domain.Entities;
using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IDorianDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Membership> Memberships { get; }
    DbSet<Customer> Customers { get; }
    DbSet<CustomerFitnessProfile> CustomerFitnessProfiles { get; }
    DbSet<BodyMeasurement> BodyMeasurements { get; }
    DbSet<BodyProgressPhoto> BodyProgressPhotos { get; }
    DbSet<ClassSession> ClassSessions { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Promotion> Promotions { get; }
    DbSet<NutritionProfile> NutritionProfiles { get; }
    DbSet<MealPlan> MealPlans { get; }
    DbSet<MealItem> MealItems { get; }
    DbSet<TrainingPlan> TrainingPlans { get; }
    DbSet<TrainingPhase> TrainingPhases { get; }
    DbSet<TrainingWeek> TrainingWeeks { get; }
    DbSet<TrainingPlanDay> TrainingPlanDays { get; }
    DbSet<TrainingExercise> TrainingExercises { get; }
    DbSet<ExerciseCatalog> ExerciseCatalog { get; }
    DbSet<WorkoutActivity> WorkoutActivities { get; }
    DbSet<WorkoutExerciseLog> WorkoutExerciseLogs { get; }
    DbSet<AccessPass> AccessPasses { get; }
    DbSet<CheckIn> CheckIns { get; }
    DbSet<AuditLog> AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
