FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/OrbitOS.Api/OrbitOS.Api.csproj src/OrbitOS.Api/
COPY src/OrbitOS.Application/OrbitOS.Application.csproj src/OrbitOS.Application/
COPY src/OrbitOS.Infrastructure/OrbitOS.Infrastructure.csproj src/OrbitOS.Infrastructure/
COPY src/OrbitOS.Domain/OrbitOS.Domain.csproj src/OrbitOS.Domain/
COPY src/OrbitOS.Contracts/OrbitOS.Contracts.csproj src/OrbitOS.Contracts/

RUN dotnet restore src/OrbitOS.Api/OrbitOS.Api.csproj

COPY src/ src/
RUN dotnet publish src/OrbitOS.Api/OrbitOS.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 5000
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrbitOS.Api.dll"]
