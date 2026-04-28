FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/Directory.Build.props backend/
COPY backend/src/BuildingBlocks/Dorian.SharedKernel/Dorian.SharedKernel.csproj backend/src/BuildingBlocks/Dorian.SharedKernel/
COPY backend/src/Modules/Identity/Dorian.Modules.Identity.Domain/Dorian.Modules.Identity.Domain.csproj backend/src/Modules/Identity/Dorian.Modules.Identity.Domain/
COPY backend/src/Modules/Branches/Dorian.Modules.Branches.Domain/Dorian.Modules.Branches.Domain.csproj backend/src/Modules/Branches/Dorian.Modules.Branches.Domain/
COPY backend/src/Modules/Memberships/Dorian.Modules.Memberships.Domain/Dorian.Modules.Memberships.Domain.csproj backend/src/Modules/Memberships/Dorian.Modules.Memberships.Domain/
COPY backend/src/Modules/Classes/Dorian.Modules.Classes.Domain/Dorian.Modules.Classes.Domain.csproj backend/src/Modules/Classes/Dorian.Modules.Classes.Domain/
COPY backend/src/Modules/Promotions/Dorian.Modules.Promotions.Domain/Dorian.Modules.Promotions.Domain.csproj backend/src/Modules/Promotions/Dorian.Modules.Promotions.Domain/
COPY backend/src/Modules/Training/Dorian.Modules.Training.Domain/Dorian.Modules.Training.Domain.csproj backend/src/Modules/Training/Dorian.Modules.Training.Domain/
COPY backend/src/Modules/Nutrition/Dorian.Modules.Nutrition.Domain/Dorian.Modules.Nutrition.Domain.csproj backend/src/Modules/Nutrition/Dorian.Modules.Nutrition.Domain/
COPY backend/src/Modules/Payments/Dorian.Modules.Payments.Domain/Dorian.Modules.Payments.Domain.csproj backend/src/Modules/Payments/Dorian.Modules.Payments.Domain/
COPY backend/src/Modules/Auditing/Dorian.Modules.Auditing.Domain/Dorian.Modules.Auditing.Domain.csproj backend/src/Modules/Auditing/Dorian.Modules.Auditing.Domain/
COPY backend/src/Api/Dorian.Api/Dorian.Api.csproj backend/src/Api/Dorian.Api/
RUN dotnet restore backend/src/Api/Dorian.Api/Dorian.Api.csproj

COPY backend/ backend/
RUN dotnet publish backend/src/Api/Dorian.Api/Dorian.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Dorian.Api.dll"]
