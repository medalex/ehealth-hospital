FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/EHealth.Hospital.Api/EHealth.Hospital.Api.csproj EHealth.Hospital.Api/
RUN dotnet restore EHealth.Hospital.Api/EHealth.Hospital.Api.csproj

COPY src/EHealth.Hospital.Api/ EHealth.Hospital.Api/
RUN dotnet publish EHealth.Hospital.Api/EHealth.Hospital.Api.csproj \
    -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out ./
EXPOSE 3003
ENV ASPNETCORE_URLS=http://+:3003
ENTRYPOINT ["dotnet", "EHealth.Hospital.Api.dll"]
