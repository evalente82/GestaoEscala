# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências primeiro para aproveitar o cache do Docker
COPY ["GestaoEscalaPermutas.Server/GestaoEscalaPermutas.Server.csproj", "GestaoEscalaPermutas.Server/"]
COPY ["GestaoEscalaPermutas.Dominio/GestaoEscalaPermutas.Dominio.csproj", "GestaoEscalaPermutas.Dominio/"]
COPY ["GestaoEscalaPermutas.Repository/GestaoEscalaPermutas.Repository.csproj", "GestaoEscalaPermutas.Repository/"]
COPY ["GestaoEscalaPermutas.sln", "."]
RUN dotnet restore "GestaoEscalaPermutas.sln"

# Copiar o restante do código fonte
COPY . .
WORKDIR "/src/GestaoEscalaPermutas.Server"

# Publicar a aplicação
RUN dotnet publish "GestaoEscalaPermutas.Server.csproj" -c Release -o /app/publish --no-restore --runtime linux-x64 --self-contained false

# Etapa 2: Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar os arquivos publicados da etapa de build
COPY --from=build /app/publish .

# Copiar o script de entrada
# O certificado deve ser gerenciado por segredos, não copiado para a imagem.
# Se for realmente necessário, copie-o, mas a melhor prática é usar segredos.
COPY --from=build /src/docker-entrypoint.sh .
COPY --from=build /src/GestaoEscalaPermutas.Server/certificado.pfx .

RUN chmod +x ./docker-entrypoint.sh

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["./docker-entrypoint.sh"]