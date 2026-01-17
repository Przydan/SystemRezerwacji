FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for better caching
COPY ["SystemRezerwacji.sln", "./"]
COPY ["src/Presentation/Web/Web.csproj", "src/Presentation/Web/"]
COPY ["src/Core/Application/Application.csproj", "src/Core/Application/"]
COPY ["src/Core/Domain/Domain.csproj", "src/Core/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/Shared.csproj"]

# Restore dependencies
RUN dotnet restore "src/Presentation/Web/Web.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/Presentation/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Web.dll"]
