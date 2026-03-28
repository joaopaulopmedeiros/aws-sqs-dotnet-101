FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Order.Worker/Order.Worker.csproj Order.Worker/
COPY src/Order.Infrastructure/Order.Infrastructure.csproj Order.Infrastructure/
COPY src/Order.Core/Order.Core.csproj Order.Core/
RUN dotnet restore Order.Worker/Order.Worker.csproj

COPY src/Order.Worker/ Order.Worker/
COPY src/Order.Infrastructure/ Order.Infrastructure/
COPY src/Order.Core/ Order.Core/
RUN dotnet publish Order.Worker/Order.Worker.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Order.Worker.dll"]
