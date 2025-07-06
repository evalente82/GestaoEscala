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

# Copiar todo o código fonte
COPY . .

# ==================================================================
# CORREÇÃO AQUI: Simplificamos o comando publish para garantir que todas as DLLs sejam incluídas.
# ==================================================================
WORKDIR /app/GestaoEscalaPermutas.Server
RUN dotnet publish -c Release -o /app/publish


# Etapa 2: Imagem final mínima
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /app/GestaoEscalaPermutas.Server/certificado.pfx /app/certificado.pfx

# Copia o script de entrada
COPY --from=build /app/GestaoEscalaPermutas.Server/docker-entrypoint.sh .

# Garante o formato de linha correto (LF) e permissão de execução
RUN sed -i 's/\r$//' ./docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh

# Configurações extras
RUN chmod 644 /app/certificado.pfx
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:80

# Inicia o container usando o script
ENTRYPOINT ["/app/docker-entrypoint.sh"]
