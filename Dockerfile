FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /src
#Copia todos os projetos e restaura as dependencias/ferramentas
COPY *.csproj .
RUN dotnet restore 
#Copia o resto dos arquivos
COPY . .
WORKDIR /src
#Compila os projetos
RUN dotnet build -c Release -o /app/build

#Usa o projeto compilado
FROM build-env AS publish
RUN dotnet publish -c Release -o /app/publish

#Pega a DLL que foi gerada o do projeto para executar
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS deploy
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication3.dll"]