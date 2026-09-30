# 18. Deploy Cơ Bản Với Docker

> Tag image (`10.0`) phải khớp `TargetFramework` trong file `.csproj` (ví dụ `net10.0`).

**Dockerfile**

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MyApp.Api/MyApp.Api.csproj", "MyApp.Api/"]
COPY ["MyApp.Application/MyApp.Application.csproj", "MyApp.Application/"]
COPY ["MyApp.Domain/MyApp.Domain.csproj", "MyApp.Domain/"]
COPY ["MyApp.Infrastructure/MyApp.Infrastructure.csproj", "MyApp.Infrastructure/"]
RUN dotnet restore "MyApp.Api/MyApp.Api.csproj"
COPY . .
RUN dotnet publish "MyApp.Api/MyApp.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime (image nhẹ hơn nhiều)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyApp.Api.dll"]
```

```bash
# Build và chạy
docker build -t myapp .
docker run -p 8080:8080 myapp
```

---

[← 17. Clean Architecture](17-clean-architecture.md) · [Mục lục](README.md)
