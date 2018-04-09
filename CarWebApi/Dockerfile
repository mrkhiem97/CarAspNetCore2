FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

# FROM microsoft/aspnetcore-build:2.0 AS build
# Copy csproj and restore dll dependency as distinct layers
# COPY *.csproj ./

#WORKDIR /src
#COPY CarWebApi.sln ./
#COPY CarWebApi/CarWebApi.csproj CarWebApi/
#RUN dotnet restore -nowarn:msb3202,nu1503
#COPY . .
#WORKDIR /src/CarWebApi
#RUN dotnet build -c Release -o /app

#FROM build AS publish
#RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
#COPY --from=publish /app .
COPY ./CarWebApi/docker/app/ .
ENTRYPOINT ["dotnet", "CarWebApi.dll"]
