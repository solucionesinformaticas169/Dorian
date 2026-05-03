namespace Dorian.Infrastructure.Persistence;

using Dorian.Application.Abstractions.Auth;
using Dorian.Modules.Access.Domain.Entities;
using Dorian.Modules.Classes.Domain.Entities;
using Dorian.Modules.Customers.Domain.Entities;
using Dorian.Modules.Identity.Domain.Constants;
using Dorian.Modules.Identity.Domain.Entities;
using Dorian.Modules.Memberships.Domain.Entities;
using Dorian.Modules.Nutrition.Domain.Entities;
using Dorian.Modules.Promotions.Domain.Entities;
using Dorian.Modules.Training.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DevelopmentDemoDataSeeder
{
    private static readonly Guid MainMembershipId = Guid.Parse("18181818-1818-1818-1818-181818181818");
    private static readonly Guid GonzalesMembershipId = Guid.Parse("19191919-1919-1919-1919-191919191919");
    private static readonly Guid ParqueMembershipId = Guid.Parse("3a3a3a3a-3a3a-3a3a-3a3a-3a3a3a3a3a3a");
    private static readonly Guid ElTiempoMembershipId = Guid.Parse("4a4a4a4a-4a4a-4a4a-4a4a-4a4a4a4a4a4a");
    private static readonly Guid AzoguesMembershipId = Guid.Parse("5a5a5a5a-5a5a-5a5a-5a5a-5a5a5a5a5a5a");

    private static readonly Guid SuperAdminUserId = Guid.Parse("60606060-6060-6060-6060-606060606060");
    private static readonly Guid BranchAdminUserId = Guid.Parse("61616161-6161-6161-6161-616161616161");
    private static readonly Guid ReceptionUserId = Guid.Parse("62626262-6262-6262-6262-626262626262");
    private static readonly Guid TrainerUserId = Guid.Parse("13131313-1313-1313-1313-131313131313");
    private static readonly Guid MainCustomerUserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
    private static readonly Guid OnboardingPendingUserId = Guid.Parse("63636363-6363-6363-6363-636363636363");
    private static readonly Guid ExpiredCustomerUserId = Guid.Parse("25252525-2525-2525-2525-252525252525");
    private static readonly Guid PendingCustomerUserId = Guid.Parse("64646464-6464-6464-6464-646464646464");
    private static readonly Guid SecondaryCustomerUserId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    private static readonly Guid MainCustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid OnboardingPendingCustomerId = Guid.Parse("65656565-6565-6565-6565-656565656565");
    private static readonly Guid ExpiredCustomerId = Guid.Parse("21212121-2121-2121-2121-212121212121");
    private static readonly Guid PendingCustomerId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private static readonly Guid SecondaryCustomerId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    private static readonly Guid MainFitnessProfileId = Guid.Parse("67676767-6767-6767-6767-676767676767");
    private static readonly Guid SecondaryFitnessProfileId = Guid.Parse("68686868-6868-6868-6868-686868686868");

    private static readonly Guid MainAccessPassId = Guid.Parse("28282828-2828-2828-2828-282828282828");
    private static readonly Guid SecondaryAccessPassId = Guid.Parse("31313131-3131-3131-3131-313131313131");
    private static readonly Guid PendingAccessPassId = Guid.Parse("69696969-6969-6969-6969-696969696969");
    private static readonly Guid ExpiredAccessPassId = Guid.Parse("29292929-2929-2929-2929-292929292929");

    private static readonly Guid BoxfitClassId = Guid.Parse("6a6a6a6a-6a6a-6a6a-6a6a-6a6a6a6a6a6a");
    private static readonly Guid CrossfitClassId = Guid.Parse("6b6b6b6b-6b6b-6b6b-6b6b-6b6b6b6b6b6b");
    private static readonly Guid BailoterapiaClassId = Guid.Parse("6c6c6c6c-6c6c-6c6c-6c6c-6c6c6c6c6c6c");
    private static readonly Guid SpinningClassId = Guid.Parse("6d6d6d6d-6d6d-6d6d-6d6d-6d6d6d6d6d6d");

    private static readonly Guid ActivePromotionId = Guid.Parse("15151515-1515-1515-1515-151515151515");
    private static readonly Guid BranchPromotionId = Guid.Parse("16161616-1616-1616-1616-161616161616");
    private static readonly Guid StarterPromotionId = Guid.Parse("6e6e6e6e-6e6e-6e6e-6e6e-6e6e6e6e6e6e");

    private static readonly Guid MainBodyMeasurementLatestId = Guid.Parse("6f6f6f6f-6f6f-6f6f-6f6f-6f6f6f6f6f6f");
    private static readonly Guid MainBodyPhotoFrontId = Guid.Parse("70707070-7070-7070-7070-707070707070");
    private static readonly Guid MainBodyPhotoSideId = Guid.Parse("71717171-7171-7171-7171-717171717171");

    private static readonly Guid MainTrainingPlanId = Guid.Parse("72727272-7272-7272-7272-727272727272");
    private static readonly Guid MainWorkoutActivityOneId = Guid.Parse("73737373-7373-7373-7373-737373737373");
    private static readonly Guid MainWorkoutActivityTwoId = Guid.Parse("74747474-7474-7474-7474-747474747474");
    private static readonly Guid MainWorkoutActivityThreeId = Guid.Parse("75757575-7575-7575-7575-757575757575");

    private static readonly Guid MainNutritionProfileId = Guid.Parse("76767676-7676-7676-7676-767676767676");

    public static async Task SeedAsync(AppDbContext dbContext, IAppPasswordHasher passwordHasher, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        await EnsureBranchesAsync(dbContext, cancellationToken);
        await EnsureMembershipsAsync(dbContext, cancellationToken);
        await EnsureUsersAsync(dbContext, passwordHasher, cancellationToken);
        await EnsureCustomersAsync(dbContext, now, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await EnsureAccessPassesAsync(dbContext, now, cancellationToken);
        await EnsureClassesAsync(dbContext, now, cancellationToken);
        await EnsureBookingsAsync(dbContext, cancellationToken);
        await EnsureCheckInsAsync(dbContext, now, cancellationToken);
        await EnsurePromotionsAsync(dbContext, now, cancellationToken);
        await EnsureFitnessProfilesAsync(dbContext, cancellationToken);
        await EnsureBodyTrackingAsync(dbContext, now, cancellationToken);
        await EnsureTrainingPlanAsync(dbContext, now, cancellationToken);
        await EnsureWorkoutActivitiesAsync(dbContext, now, cancellationToken);
        await EnsureNutritionAsync(dbContext, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureBranchesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await EnsureBranchAsync(dbContext, SeedData.ElCebollarBranchId, "CEBOLLAR", "El Cebollar", "Cuenca", "Av. Abelardo J Andrade y Reinaldo Chico Penaherrera", "0990001001", "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", SeedData.BranchMaps.ElCebollar, cancellationToken);
        await EnsureBranchAsync(dbContext, SeedData.GonzalesSuarezBranchId, "GONSUAREZ", "Gonzales Suarez", "Cuenca", "Av. Gonzales Suarez y Jijon de Caamano", "0990001002", "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", SeedData.BranchMaps.GonzalesSuarez, cancellationToken);
        await EnsureBranchAsync(dbContext, SeedData.ParqueIndustrialBranchId, "PARQUEIND", "Parque Industrial", "Cuenca", "Octavio Chacon Moscoso y Cornelio Vintimilla", "0990001003", "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", SeedData.BranchMaps.ParqueIndustrial, cancellationToken);
        await EnsureBranchAsync(dbContext, SeedData.ElTiempoBranchId, "ELTIEMPO", "El Tiempo", "Cuenca", "Av. Loja y Rodrigo de Triana", "0990001004", "Lun a Vie 06:00 - 22:00 | Sab 08:00 - 14:00", SeedData.BranchMaps.ElTiempo, cancellationToken);
        await EnsureBranchAsync(dbContext, SeedData.AzoguesBranchId, "AZOGUES", "Azogues", "Azogues", "Calle Simon Bolivar", "0990001005", "Lun a Vie 06:00 - 21:00 | Sab 08:00 - 13:00", SeedData.BranchMaps.Azogues, cancellationToken);
    }

    private static async Task EnsureBranchAsync(AppDbContext dbContext, Guid id, string code, string name, string city, string address, string phoneNumber, string openingHours, string mapUrl, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (branch is null)
        {
            return;
        }

        branch.Update(code, name, city, address, phoneNumber, openingHours, mapUrl, null, null, true);
    }

    private static async Task EnsureMembershipsAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await EnsureMembershipAsync(dbContext, MainMembershipId, SeedData.ElCebollarBranchId, "Mensual Central", 30, 35m, cancellationToken);
        await EnsureMembershipAsync(dbContext, GonzalesMembershipId, SeedData.GonzalesSuarezBranchId, "Mensual Gonzales", 30, 33m, cancellationToken);
        await EnsureMembershipAsync(dbContext, ParqueMembershipId, SeedData.ParqueIndustrialBranchId, "Plan Industrial", 30, 34m, cancellationToken);
        await EnsureMembershipAsync(dbContext, ElTiempoMembershipId, SeedData.ElTiempoBranchId, "Plan Express", 30, 29m, cancellationToken);
        await EnsureMembershipAsync(dbContext, AzoguesMembershipId, SeedData.AzoguesBranchId, "Azogues Full", 30, 27m, cancellationToken);
    }

    private static async Task EnsureMembershipAsync(AppDbContext dbContext, Guid id, Guid branchId, string name, int durationInDays, decimal price, CancellationToken cancellationToken)
    {
        var membership = await dbContext.Memberships.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (membership is null)
        {
            dbContext.Memberships.Add(new Membership(id, branchId, name, durationInDays, price, "USD", true));
            return;
        }

        membership.Update(branchId, name, durationInDays, price, "USD", true);
    }

    private static async Task EnsureUsersAsync(AppDbContext dbContext, IAppPasswordHasher passwordHasher, CancellationToken cancellationToken)
    {
        var passwordHash = passwordHasher.Hash("Pass1234!");

        await EnsureUserAsync(dbContext, SuperAdminUserId, "superadmin@dorian.test", "Super Admin Dorian", passwordHash, null, [SeedData.SuperAdminRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, BranchAdminUserId, "branchadmin@dorian.test", "Branch Admin Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.BranchAdminRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, ReceptionUserId, "reception@dorian.test", "Recepcion Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.ReceptionRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, TrainerUserId, "trainer@dorian.test", "Coach Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.TrainerRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, MainCustomerUserId, "customer@dorian.test", "Jane Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.CustomerRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, OnboardingPendingUserId, "pendingonboarding@dorian.test", "Leo Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.CustomerRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, ExpiredCustomerUserId, "expiredmembership@dorian.test", "Carla Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.CustomerRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, PendingCustomerUserId, "pendingcustomer@dorian.test", "Mateo Dorian", passwordHash, SeedData.ElCebollarBranchId, [SeedData.CustomerRoleId], true, cancellationToken);
        await EnsureUserAsync(dbContext, SecondaryCustomerUserId, "othercustomer@dorian.test", "Ana Dorian", passwordHash, SeedData.GonzalesSuarezBranchId, [SeedData.CustomerRoleId], true, cancellationToken);
    }

    private static async Task EnsureUserAsync(AppDbContext dbContext, Guid id, string email, string fullName, string passwordHash, Guid? branchId, IReadOnlyCollection<Guid> roleIds, bool isActive, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.Include(x => x.UserRoles).FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            user = new User(id, email, fullName, passwordHash);
            dbContext.Users.Add(user);
        }

        user.UpdateProfile(fullName, null);
        user.SetPasswordHash(passwordHash);
        user.AssignToBranch(branchId);
        user.SetRoles(roleIds);
        user.SetActive(isActive);
    }

    private static async Task EnsureCustomersAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        await EnsureCustomerAsync(dbContext, MainCustomerId, MainCustomerUserId, SeedData.ElCebollarBranchId, "Jane", "Dorian", "ID-001", "0991111111", new DateOnly(1995, 1, 1), Gender.Female, "Martha Dorian", "0992222222", MainMembershipId, now.AddDays(-18), now.AddDays(12), CustomerStatus.Active, cancellationToken);
        await EnsureCustomerAsync(dbContext, OnboardingPendingCustomerId, OnboardingPendingUserId, SeedData.ElCebollarBranchId, "Leo", "Dorian", "ID-002", "0991112222", new DateOnly(1998, 2, 2), Gender.Male, "Paola Dorian", "0993334444", MainMembershipId, now.AddDays(-10), now.AddDays(20), CustomerStatus.Active, cancellationToken);
        await EnsureCustomerAsync(dbContext, ExpiredCustomerId, ExpiredCustomerUserId, SeedData.ElCebollarBranchId, "Carla", "Vencida", "ID-003", "0993331111", new DateOnly(1991, 3, 4), Gender.Female, "David Vencida", "0995551111", MainMembershipId, now.AddDays(-40), now.AddDays(-1), CustomerStatus.Active, cancellationToken);
        await EnsureCustomerAsync(dbContext, PendingCustomerId, PendingCustomerUserId, SeedData.ElCebollarBranchId, "Mateo", "Pendiente", "ID-004", "0998880000", new DateOnly(1997, 8, 12), Gender.Male, "Lucia Pendiente", "0998881111", MainMembershipId, now.AddDays(1), now.AddDays(31), CustomerStatus.Active, cancellationToken);
        await EnsureCustomerAsync(dbContext, SecondaryCustomerId, SecondaryCustomerUserId, SeedData.GonzalesSuarezBranchId, "Ana", "Dorian", "ID-005", "0987777777", new DateOnly(1993, 9, 14), Gender.Female, "Luis Dorian", "0986666666", GonzalesMembershipId, now.AddDays(-8), now.AddDays(3), CustomerStatus.Active, cancellationToken);
    }

    private static async Task EnsureCustomerAsync(AppDbContext dbContext, Guid id, Guid userId, Guid branchId, string firstName, string lastName, string identificationNumber, string phone, DateOnly birthDate, Gender gender, string emergencyContactName, string emergencyContactPhone, Guid? membershipId, DateTimeOffset? membershipStartsAtUtc, DateTimeOffset? membershipEndsAtUtc, CustomerStatus status, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (customer is null)
        {
            dbContext.Customers.Add(new Customer(id, userId, branchId, firstName, lastName, identificationNumber, phone, birthDate, gender, emergencyContactName, emergencyContactPhone, membershipId, membershipStartsAtUtc, membershipEndsAtUtc, status));
            return;
        }

        customer.Update(branchId, firstName, lastName, identificationNumber, phone, birthDate, gender, emergencyContactName, emergencyContactPhone, membershipId, membershipStartsAtUtc, membershipEndsAtUtc, status);
    }

    private static async Task EnsureAccessPassesAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        await EnsureAccessPassAsync(dbContext, MainAccessPassId, MainCustomerId, "DORIAN-MAIN-QR", now.AddHours(8), cancellationToken);
        await EnsureAccessPassAsync(dbContext, SecondaryAccessPassId, SecondaryCustomerId, "DORIAN-SECONDARY-QR", now.AddHours(8), cancellationToken);
        await EnsureAccessPassAsync(dbContext, PendingAccessPassId, PendingCustomerId, "DORIAN-PENDING-QR", now.AddHours(8), cancellationToken);
        await EnsureAccessPassAsync(dbContext, ExpiredAccessPassId, ExpiredCustomerId, "DORIAN-EXPIRED-QR", now.AddHours(-2), cancellationToken);
    }

    private static async Task EnsureAccessPassAsync(AppDbContext dbContext, Guid id, Guid customerId, string qrCodeValue, DateTimeOffset expiresAt, CancellationToken cancellationToken)
    {
        var pass = await dbContext.AccessPasses.FirstOrDefaultAsync(x => x.CustomerId == customerId || x.Id == id, cancellationToken);
        if (pass is null)
        {
            dbContext.AccessPasses.Add(new AccessPass(id, customerId, qrCodeValue, expiresAt));
            return;
        }

        pass.Regenerate(qrCodeValue, expiresAt);
    }

    private static async Task EnsureClassesAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var startOfToday = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);
        await EnsureClassAsync(dbContext, BoxfitClassId, SeedData.ElCebollarBranchId, "Boxfit", "Golpea el estres y trabaja cardio con tecnica basica.", startOfToday.AddHours(13), startOfToday.AddHours(14), 18, cancellationToken);
        await EnsureClassAsync(dbContext, CrossfitClassId, SeedData.ElCebollarBranchId, "Crossfit", "Trabajo funcional para fuerza y potencia.", startOfToday.AddHours(16), startOfToday.AddHours(17), 16, cancellationToken);
        await EnsureClassAsync(dbContext, BailoterapiaClassId, SeedData.GonzalesSuarezBranchId, "Bailoterapia", "Cardio grupal con foco en energia y adherencia.", startOfToday.AddHours(18), startOfToday.AddHours(19), 24, cancellationToken);
        await EnsureClassAsync(dbContext, SpinningClassId, SeedData.ParqueIndustrialBranchId, "Spinning", "Sesiones de resistencia y quema calorica guiada.", startOfToday.AddHours(20), startOfToday.AddHours(21), 20, cancellationToken);
    }

    private static async Task EnsureClassAsync(AppDbContext dbContext, Guid id, Guid branchId, string name, string description, DateTimeOffset startTime, DateTimeOffset endTime, int capacity, CancellationToken cancellationToken)
    {
        var classSession = await dbContext.ClassSessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (classSession is null)
        {
            dbContext.ClassSessions.Add(new ClassSession(id, branchId, TrainerUserId, name, description, startTime, endTime, capacity, ClassSessionStatus.Scheduled));
            return;
        }

        classSession.Update(branchId, TrainerUserId, name, description, startTime, endTime, capacity, ClassSessionStatus.Scheduled);
    }

    private static async Task EnsureBookingsAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await EnsureBookingAsync(dbContext, Guid.Parse("78787878-7878-7878-7878-787878787878"), MainCustomerId, BoxfitClassId, BookingStatus.Reserved, cancellationToken);
        await EnsureBookingAsync(dbContext, Guid.Parse("79797979-7979-7979-7979-797979797979"), MainCustomerId, CrossfitClassId, BookingStatus.Attended, cancellationToken);
        await EnsureBookingAsync(dbContext, Guid.Parse("7a7a7a7a-7a7a-7a7a-7a7a-7a7a7a7a7a7a"), SecondaryCustomerId, BailoterapiaClassId, BookingStatus.Reserved, cancellationToken);
        await EnsureBookingAsync(dbContext, Guid.Parse("7b7b7b7b-7b7b-7b7b-7b7b-7b7b7b7b7b7b"), PendingCustomerId, SpinningClassId, BookingStatus.Reserved, cancellationToken);
    }

    private static async Task EnsureBookingAsync(AppDbContext dbContext, Guid id, Guid customerId, Guid classSessionId, BookingStatus status, CancellationToken cancellationToken)
    {
        var booking = await dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (booking is null)
        {
            booking = new Booking(id, customerId, classSessionId);
            dbContext.Bookings.Add(booking);
        }

        if (status == BookingStatus.Cancelled && booking.Status != BookingStatus.Cancelled)
        {
            booking.Cancel();
        }
        else if (status == BookingStatus.Attended && booking.Status != BookingStatus.Attended)
        {
            booking.MarkAttended();
        }
    }

    private static async Task EnsureCheckInsAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        await EnsureCheckInAsync(dbContext, Guid.Parse("7c7c7c7c-7c7c-7c7c-7c7c-7c7c7c7c7c7c"), MainCustomerId, SeedData.ElCebollarBranchId, now.AddHours(-2), ReceptionUserId, CheckInSource.QrScan, CheckInStatus.Accepted, null, cancellationToken);
        await EnsureCheckInAsync(dbContext, Guid.Parse("7d7d7d7d-7d7d-7d7d-7d7d-7d7d7d7d7d7d"), SecondaryCustomerId, SeedData.GonzalesSuarezBranchId, now.AddHours(-3), ReceptionUserId, CheckInSource.Manual, CheckInStatus.Accepted, null, cancellationToken);
        await EnsureCheckInAsync(dbContext, Guid.Parse("7e7e7e7e-7e7e-7e7e-7e7e-7e7e7e7e7e7e"), ExpiredCustomerId, SeedData.ElCebollarBranchId, now.AddHours(-1), ReceptionUserId, CheckInSource.QrScan, CheckInStatus.Rejected, "Membresia vencida", cancellationToken);
    }

    private static async Task EnsureCheckInAsync(AppDbContext dbContext, Guid id, Guid customerId, Guid branchId, DateTimeOffset checkedInAt, Guid? checkedInByUserId, CheckInSource source, CheckInStatus status, string? rejectionReason, CancellationToken cancellationToken)
    {
        if (await dbContext.CheckIns.AnyAsync(x => x.Id == id, cancellationToken))
        {
            return;
        }

        dbContext.CheckIns.Add(new CheckIn(id, customerId, branchId, checkedInAt, checkedInByUserId, source, status, rejectionReason));
    }

    private static async Task EnsurePromotionsAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        await EnsurePromotionAsync(dbContext, ActivePromotionId, null, "Mes Dorian", "Activa tu primer mes con onboarding, plan y acceso QR incluidos.", PromotionDiscountType.Percentage, 20m, now.AddDays(-2), now.AddDays(10), PromotionStatus.Active, cancellationToken);
        await EnsurePromotionAsync(dbContext, BranchPromotionId, SeedData.ElCebollarBranchId, "Boxfit Weekend", "Reserva tu clase de Boxfit del fin de semana con cupos limitados.", PromotionDiscountType.Informational, null, now.AddDays(-1), now.AddDays(7), PromotionStatus.Active, cancellationToken);
        await EnsurePromotionAsync(dbContext, StarterPromotionId, SeedData.GonzalesSuarezBranchId, "Semana Starter", "Ideal para clientes pendientes o por vencer que quieren retomar ritmo.", PromotionDiscountType.FixedAmount, 5m, now.AddDays(-1), now.AddDays(5), PromotionStatus.Active, cancellationToken);
    }

    private static async Task EnsurePromotionAsync(AppDbContext dbContext, Guid id, Guid? branchId, string title, string description, PromotionDiscountType discountType, decimal? discountValue, DateTimeOffset startsAt, DateTimeOffset endsAt, PromotionStatus status, CancellationToken cancellationToken)
    {
        var promotion = await dbContext.Promotions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (promotion is null)
        {
            dbContext.Promotions.Add(new Promotion(id, branchId, title, description, null, discountType, discountValue, startsAt, endsAt, status));
            return;
        }

        promotion.Update(branchId, title, description, null, discountType, discountValue, startsAt, endsAt, status);
    }

    private static async Task EnsureFitnessProfilesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await EnsureFitnessProfileAsync(dbContext, MainFitnessProfileId, MainCustomerId, FitnessGoal.Hypertrophy, FocusMuscleGroup.Chest, FitnessExperienceLevel.Intermediate, GymType.Advanced, true, [TrainingDay.Monday, TrainingDay.Wednesday, TrainingDay.Friday, TrainingDay.Saturday], "18:30", FitnessProfileGender.Female, new DateOnly(1995, 1, 1), 72.4m, 168m, 69m, true, NotificationIntensity.Moderate, true, cancellationToken);
        await EnsureFitnessProfileAsync(dbContext, SecondaryFitnessProfileId, SecondaryCustomerId, FitnessGoal.Definition, FocusMuscleGroup.Glutes, FitnessExperienceLevel.Advanced, GymType.Advanced, true, [TrainingDay.Tuesday, TrainingDay.Thursday, TrainingDay.Saturday], "07:00", FitnessProfileGender.Female, new DateOnly(1993, 9, 14), 61.8m, 164m, 59m, true, NotificationIntensity.High, true, cancellationToken);
    }

    private static async Task EnsureFitnessProfileAsync(AppDbContext dbContext, Guid id, Guid customerId, FitnessGoal goal, FocusMuscleGroup focusMuscleGroup, FitnessExperienceLevel experienceLevel, GymType gymType, bool includeCardio, IReadOnlyCollection<TrainingDay> trainingDays, string preferredTrainingTime, FitnessProfileGender gender, DateOnly birthDate, decimal weightKg, decimal heightCm, decimal targetWeightKg, bool notificationsEnabled, NotificationIntensity notificationIntensity, bool onboardingCompleted, CancellationToken cancellationToken)
    {
        var profile = await dbContext.CustomerFitnessProfiles.FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        if (profile is null)
        {
            dbContext.CustomerFitnessProfiles.Add(new CustomerFitnessProfile(id, customerId, goal, focusMuscleGroup, experienceLevel, gymType, includeCardio, trainingDays, preferredTrainingTime, gender, birthDate, weightKg, heightCm, targetWeightKg, notificationsEnabled, notificationIntensity, onboardingCompleted));
            return;
        }

        profile.Update(goal, focusMuscleGroup, experienceLevel, gymType, includeCardio, trainingDays, preferredTrainingTime, gender, birthDate, weightKg, heightCm, targetWeightKg, notificationsEnabled, notificationIntensity, onboardingCompleted);
    }

    private static async Task EnsureBodyTrackingAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        await EnsureBodyMeasurementAsync(dbContext, Guid.Parse("80808080-8080-8080-8080-808080808080"), MainCustomerId, now.AddDays(-28), 74.2m, 168m, 27.1m, 31.4m, 2.8m, 18.2m, 81m, 93m, 99m, 104m, 31m, 31.2m, 55m, 55.5m, 36.5m, 36.8m, 33m, "Inicio del bloque de hipertrofia.", cancellationToken);
        await EnsureBodyMeasurementAsync(dbContext, Guid.Parse("81818181-8181-8181-8181-818181818181"), MainCustomerId, now.AddDays(-14), 73.1m, 168m, 25.8m, 32.0m, 2.8m, 17.8m, 79.5m, 94m, 98m, 105m, 31.5m, 31.6m, 55.4m, 55.7m, 36.8m, 37.0m, 32.5m, "Se nota mejor adherencia al plan.", cancellationToken);
        await EnsureBodyMeasurementAsync(dbContext, MainBodyMeasurementLatestId, MainCustomerId, now.AddDays(-3), 72.4m, 168m, 24.9m, 32.6m, 2.8m, 17.0m, 77.8m, 95.2m, 97.3m, 105.5m, 31.8m, 31.9m, 55.8m, 56.0m, 37.0m, 37.3m, 32.2m, "Semana fuerte de fuerza y mejor descanso.", cancellationToken);
        await EnsureBodyPhotoAsync(dbContext, MainBodyPhotoFrontId, MainCustomerId, "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?auto=format&fit=crop&w=1200&q=80", now.AddDays(-21), BodyProgressPhotoType.Front, "Comparativa frontal del primer mes.", cancellationToken);
        await EnsureBodyPhotoAsync(dbContext, MainBodyPhotoSideId, MainCustomerId, "https://images.unsplash.com/photo-1549570652-97324981a6fd?auto=format&fit=crop&w=1200&q=80", now.AddDays(-7), BodyProgressPhotoType.Side, "Mejor postura y cintura mas definida.", cancellationToken);
    }

    private static async Task EnsureBodyMeasurementAsync(AppDbContext dbContext, Guid id, Guid customerId, DateTimeOffset measuredAt, decimal weightKg, decimal heightCm, decimal? bodyFatPercentage, decimal? muscleMassKg, decimal? boneMassKg, decimal? residualMassKg, decimal? waistCm, decimal? chestCm, decimal? hipCm, decimal? shouldersCm, decimal? leftArmCm, decimal? rightArmCm, decimal? leftLegCm, decimal? rightLegCm, decimal? leftCalfCm, decimal? rightCalfCm, decimal? neckCm, string? notes, CancellationToken cancellationToken)
    {
        var measurement = await dbContext.BodyMeasurements.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (measurement is null)
        {
            dbContext.BodyMeasurements.Add(new BodyMeasurement(id, customerId, measuredAt, weightKg, heightCm, bodyFatPercentage, muscleMassKg, boneMassKg, residualMassKg, waistCm, chestCm, hipCm, shouldersCm, leftArmCm, rightArmCm, leftLegCm, rightLegCm, leftCalfCm, rightCalfCm, neckCm, notes));
            return;
        }

        measurement.Update(measuredAt, weightKg, heightCm, bodyFatPercentage, muscleMassKg, boneMassKg, residualMassKg, waistCm, chestCm, hipCm, shouldersCm, leftArmCm, rightArmCm, leftLegCm, rightLegCm, leftCalfCm, rightCalfCm, neckCm, notes);
    }

    private static async Task EnsureBodyPhotoAsync(AppDbContext dbContext, Guid id, Guid customerId, string photoUrl, DateTimeOffset takenAt, BodyProgressPhotoType type, string? notes, CancellationToken cancellationToken)
    {
        var photo = await dbContext.BodyProgressPhotos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (photo is null)
        {
            dbContext.BodyProgressPhotos.Add(new BodyProgressPhoto(id, customerId, photoUrl, takenAt, type, notes));
            return;
        }

        photo.Update(photoUrl, takenAt, type, notes);
    }

    private static async Task EnsureTrainingPlanAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (await dbContext.TrainingPlans.AnyAsync(x => x.CustomerId == MainCustomerId, cancellationToken))
        {
            return;
        }

        dbContext.TrainingPlans.Add(new TrainingPlan(MainTrainingPlanId, MainCustomerId, FitnessGoal.Hypertrophy, FitnessExperienceLevel.Intermediate, FocusMuscleGroup.Chest, DateOnly.FromDateTime(now.UtcDateTime)));
        dbContext.TrainingPhases.AddRange(
            new TrainingPhase(Guid.Parse("82828282-8282-8282-8282-828282828282"), MainTrainingPlanId, TrainingPhaseName.Resistencia, "Base de tecnica, movilidad y adherencia.", 1, 1),
            new TrainingPhase(Guid.Parse("83838383-8383-8383-8383-838383838383"), MainTrainingPlanId, TrainingPhaseName.Fuerza, "Bloque para consolidar patrones y subir carga.", 2, 1),
            new TrainingPhase(Guid.Parse("84848484-8484-8484-8484-848484848484"), MainTrainingPlanId, TrainingPhaseName.Hipertrofia, "Trabajo principal para ganar masa muscular con foco en pecho.", 3, 2));

        dbContext.TrainingWeeks.AddRange(
            new TrainingWeek(Guid.Parse("85858585-8585-8585-8585-858585858585"), Guid.Parse("82828282-8282-8282-8282-828282828282"), 1, "Semana 1", "Adaptacion, tecnica y cardio ligero."),
            new TrainingWeek(Guid.Parse("86868686-8686-8686-8686-868686868686"), Guid.Parse("83838383-8383-8383-8383-838383838383"), 2, "Semana 2", "Fuerza basica en tren superior e inferior."),
            new TrainingWeek(Guid.Parse("87878787-8787-8787-8787-878787878787"), Guid.Parse("84848484-8484-8484-8484-848484848484"), 3, "Semana 3", "Hipertrofia con mayor volumen."));

        dbContext.TrainingPlanDays.AddRange(
            new TrainingPlanDay(Guid.Parse("88888888-8888-8888-8888-888888888888"), Guid.Parse("85858585-8585-8585-8585-858585858585"), TrainingDay.Monday, "Torso tecnico + cardio", 55, TrainingDayIntensity.Medium),
            new TrainingPlanDay(Guid.Parse("89898989-8989-8989-8989-898989898989"), Guid.Parse("85858585-8585-8585-8585-858585858585"), TrainingDay.Wednesday, "Pierna base y core", 60, TrainingDayIntensity.Medium),
            new TrainingPlanDay(Guid.Parse("8a8a8a8a-8a8a-8a8a-8a8a-8a8a8a8a8a8a"), Guid.Parse("86868686-8686-8686-8686-868686868686"), TrainingDay.Friday, "Fuerza de empuje", 65, TrainingDayIntensity.High),
            new TrainingPlanDay(Guid.Parse("8b8b8b8b-8b8b-8b8b-8b8b-8b8b8b8b8b8b"), Guid.Parse("87878787-8787-8787-8787-878787878787"), TrainingDay.Saturday, "Hipertrofia full body", 70, TrainingDayIntensity.High));

        dbContext.TrainingExercises.AddRange(
            new TrainingExercise(Guid.Parse("8c8c8c8c-8c8c-8c8c-8c8c-8c8c8c8c8c8c"), Guid.Parse("88888888-8888-8888-8888-888888888888"), null, "Press de banca con mancuernas", ExerciseMuscleGroup.Chest, 4, "10-12", 75, 12m, "Controla la bajada.", 1),
            new TrainingExercise(Guid.Parse("8d8d8d8d-8d8d-8d8d-8d8d-8d8d8d8d8d8d"), Guid.Parse("88888888-8888-8888-8888-888888888888"), null, "Remo con cable", ExerciseMuscleGroup.Back, 4, "10-12", 60, 18m, "Sostiene un segundo al final.", 2),
            new TrainingExercise(Guid.Parse("8e8e8e8e-8e8e-8e8e-8e8e-8e8e8e8e8e8e"), Guid.Parse("88888888-8888-8888-8888-888888888888"), null, "Bicicleta estatica", ExerciseMuscleGroup.Cardio, 1, "15 min", 0, null, "Ritmo moderado.", 3),
            new TrainingExercise(Guid.Parse("8f8f8f8f-8f8f-8f8f-8f8f-8f8f8f8f8f8f"), Guid.Parse("89898989-8989-8989-8989-898989898989"), null, "Sentadilla goblet", ExerciseMuscleGroup.Legs, 4, "12", 75, 22m, "Prioriza profundidad.", 1),
            new TrainingExercise(Guid.Parse("90909090-9090-9090-9090-909090909090"), Guid.Parse("89898989-8989-8989-8989-898989898989"), null, "Hip thrust", ExerciseMuscleGroup.Glutes, 4, "10", 90, 40m, "Pausa arriba 1 segundo.", 2),
            new TrainingExercise(Guid.Parse("91919191-9191-9191-9191-919191919191"), Guid.Parse("8a8a8a8a-8a8a-8a8a-8a8a-8a8a8a8a8a8a"), null, "Press inclinado", ExerciseMuscleGroup.Chest, 5, "8-10", 90, 16m, "Sube de carga si mantienes tecnica.", 1),
            new TrainingExercise(Guid.Parse("92929292-9292-9292-9292-929292929292"), Guid.Parse("8a8a8a8a-8a8a-8a8a-8a8a-8a8a8a8a8a8a"), null, "Fondos asistidos", ExerciseMuscleGroup.Triceps, 4, "10-12", 75, null, "Enfatiza triceps.", 2),
            new TrainingExercise(Guid.Parse("93939393-9393-9393-9393-939393939393"), Guid.Parse("8b8b8b8b-8b8b-8b8b-8b8b-8b8b8b8b8b8b"), null, "Peso muerto rumano", ExerciseMuscleGroup.Legs, 4, "8-10", 90, 36m, "Espalda neutra.", 1),
            new TrainingExercise(Guid.Parse("94949494-9494-9494-9494-949494949494"), Guid.Parse("8b8b8b8b-8b8b-8b8b-8b8b-8b8b8b8b8b8b"), null, "Press militar sentado", ExerciseMuscleGroup.Shoulders, 4, "10", 75, 12m, "Controla el core.", 2),
            new TrainingExercise(Guid.Parse("95959595-9595-9595-9595-959595959595"), Guid.Parse("8b8b8b8b-8b8b-8b8b-8b8b-8b8b8b8b8b8b"), null, "Crunch con cable", ExerciseMuscleGroup.Abdomen, 3, "15", 45, null, "Exhala fuerte arriba.", 3));
    }

    private static async Task EnsureWorkoutActivitiesAsync(AppDbContext dbContext, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var planDays = await dbContext.TrainingPlanDays.Where(x => x.TrainingWeek.TrainingPhase.TrainingPlan.CustomerId == MainCustomerId).Include(x => x.Exercises).OrderBy(x => x.TrainingWeek.WeekNumber).ThenBy(x => x.DayOfWeek).ToListAsync(cancellationToken);
        if (!planDays.Any())
        {
            return;
        }

        planDays[0].MarkCompleted(now.AddDays(-5));
        if (planDays.Count > 1) planDays[1].MarkCompleted(now.AddDays(-3));
        if (planDays.Count > 2) planDays[2].MarkCompleted(now.AddDays(-1));

        await EnsureWorkoutActivityAsync(dbContext, MainWorkoutActivityOneId, MainCustomerId, planDays[0], now.AddDays(-5), 3300, 410, "Sesion de tecnica y cardio completada.", cancellationToken);
        if (planDays.Count > 1) await EnsureWorkoutActivityAsync(dbContext, MainWorkoutActivityTwoId, MainCustomerId, planDays[1], now.AddDays(-3), 3600, 455, "Buen trabajo de gluteos y pierna.", cancellationToken);
        if (planDays.Count > 2) await EnsureWorkoutActivityAsync(dbContext, MainWorkoutActivityThreeId, MainCustomerId, planDays[2], now.AddDays(-1), 3900, 520, "Bloque fuerte de empuje completado.", cancellationToken);
    }

    private static async Task EnsureWorkoutActivityAsync(AppDbContext dbContext, Guid id, Guid customerId, TrainingPlanDay trainingDay, DateTimeOffset completedAt, int durationSeconds, int caloriesEstimated, string notes, CancellationToken cancellationToken)
    {
        var activity = await dbContext.WorkoutActivities.Include(x => x.ExerciseLogs).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (activity is null)
        {
            activity = new WorkoutActivity(id, customerId, trainingDay.Id, completedAt, durationSeconds, caloriesEstimated, notes);
            dbContext.WorkoutActivities.Add(activity);
        }
        else
        {
            activity.Update(completedAt, durationSeconds, caloriesEstimated, notes);
        }

        if (activity.ExerciseLogs.Count == 0)
        {
            foreach (var exercise in trainingDay.Exercises.OrderBy(x => x.Order))
            {
                dbContext.WorkoutExerciseLogs.Add(new WorkoutExerciseLog(Guid.NewGuid(), activity.Id, exercise.Name, exercise.MuscleGroup, exercise.Sets, exercise.Reps, exercise.WeightKg, true));
            }
        }
    }

    private static async Task EnsureNutritionAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var profile = await dbContext.NutritionProfiles.FirstOrDefaultAsync(x => x.CustomerId == MainCustomerId, cancellationToken);
        if (profile is null)
        {
            dbContext.NutritionProfiles.Add(new NutritionProfile(MainNutritionProfileId, MainCustomerId, FitnessGoal.Hypertrophy, 2380, 150, 255, 68, 4, 2.9m, "Evitar exceso de lactosa antes de entrenar."));
        }
        else
        {
            profile.Update(FitnessGoal.Hypertrophy, 2380, 150, 255, 68, 4, 2.9m, "Evitar exceso de lactosa antes de entrenar.");
        }

        if (await dbContext.MealPlans.AnyAsync(x => x.CustomerId == MainCustomerId, cancellationToken))
        {
            return;
        }

        var mondayPlan = new MealPlan(Guid.Parse("96969696-9696-9696-9696-969696969696"), MainCustomerId, "Lunes de fuerza", "Dia de mayor carga y foco en energia sostenida.", TrainingDay.Monday);
        var fridayPlan = new MealPlan(Guid.Parse("97979797-9797-9797-9797-979797979797"), MainCustomerId, "Viernes de empuje", "Comidas orientadas a rendimiento y recuperacion.", TrainingDay.Friday);

        dbContext.MealPlans.AddRange(mondayPlan, fridayPlan);
        dbContext.MealItems.AddRange(
            new MealItem(Guid.Parse("98989898-9898-9898-9898-989898989898"), mondayPlan.Id, MealType.Breakfast, "Avena con yogurt y frutos rojos", "Desayuno alto en carbohidratos complejos y proteina ligera.", 520, 28, 72, 12),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999998"), mondayPlan.Id, MealType.Lunch, "Pollo con arroz y vegetales", "Plato principal para sostener la sesion de la tarde.", 710, 46, 88, 16),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999997"), mondayPlan.Id, MealType.Dinner, "Salmon con pure y ensalada", "Cena de recuperacion con grasas saludables.", 650, 38, 55, 24),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999996"), mondayPlan.Id, MealType.Snack, "Batido de banano y whey", "Snack post entrenamiento rapido.", 340, 32, 40, 6),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999995"), fridayPlan.Id, MealType.Breakfast, "Huevos, tostadas y fruta", "Desayuno completo para el bloque de fuerza.", 500, 30, 48, 18),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999994"), fridayPlan.Id, MealType.Lunch, "Carne magra con quinoa", "Almuerzo balanceado alto en proteina.", 690, 48, 70, 18),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999993"), fridayPlan.Id, MealType.Dinner, "Wrap integral de pavo", "Cena ligera para cerrar el dia sin pesadez.", 470, 32, 42, 14),
            new MealItem(Guid.Parse("99999999-9999-9999-9999-999999999992"), fridayPlan.Id, MealType.Snack, "Yogurt griego con granola", "Refuerzo de proteina entre reuniones y entrenamiento.", 290, 18, 25, 9));
    }
}
