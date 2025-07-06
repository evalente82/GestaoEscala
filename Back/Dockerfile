# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solução e projetos individualmente para otimizar o cache do Docker
COPY GestaoEscalaPermutas.sln ./
COPY GestaoEscalaPermutas.Server/*.csproj ./GestaoEscalaPermutas.Server/
COPY GestaoEscalaPermutas.Dominio/*.csproj ./GestaoEscalaPermutas.Dominio/
COPY GestaoEscalaPermuta.Infra/*.csproj ./GestaoEscalaPermuta.Infra/
COPY GestaoEscalaPermutas.Repository/*.csproj ./GestaoEscalaPermutas.Repository/
COPY GestaoEscalaPermutas.Tests/*.csproj ./GestaoEscalaPermutas.Tests/

# Restaurar dependências
RUN dotnet restore

# Copiar todo o código fonte (incluindo o docker-entrypoint.sh da pasta Back)
COPY . .

# Publica a aplicação. Esta linha está correta e resolveu o erro anterior.
WORKDIR /app/GestaoEscalaPermutas.Server
RUN dotnet publish -c Release -o /app/publish


# Etapa 2: Imagem final mínima
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /app/certificado.pfx /app/certificado.pfx

# ==================================================================
# CORREÇÃO AQUI: Corrigindo o caminho de onde o script é copiado da etapa de build.
# ==================================================================
COPY --from=build /app/docker-entrypoint.sh .

# Garante o formato de linha correto (LF) e permissão de execução
RUN sed -i 's/\r$//' ./docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh

# Configurações extras
RUN chmod 644 /app/certificado.pfx
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:80

# Inicia o container usando o script
ENTRYPOINT ["/app/docker-entrypoint.sh"]
