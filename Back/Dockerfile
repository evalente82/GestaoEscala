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

# Copiar todo o código fonte (incluindo Dockerfile, certificado.pfx, e docker-entrypoint.sh)
COPY . .

# Publica a aplicação de forma mais explícita, especificando o projeto.
RUN dotnet publish "GestaoEscalaPermutas.Server/GestaoEscalaPermutas.Server.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained true


# Etapa 2: Imagem final mínima
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0
WORKDIR /app
COPY --from=build /app/publish .

# CORREÇÃO 1: Copiando o certificado do local correto na imagem de build (/app/).
COPY --from=build /app/GestaoEscalaPermutas.Server/certificado.pfx /app/certificado.pfx

# CORREÇÃO 2: Copiando o script do local correto na imagem de build (/app/).
COPY --from=build /app/docker-entrypoint.sh .

# Garante o formato de linha correto (LF) e permissão de execução
RUN sed -i 's/\r$//' ./docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh

# Configurações extras
RUN chmod 644 /app/certificado.pfx
EXPOSE 8080

# CORREÇÃO 3: Alinhando a porta da aplicação com a porta interna do fly.toml (8080).
ENV ASPNETCORE_URLS=http://+:8080

# Inicia o container usando o script
ENTRYPOINT ["/app/docker-entrypoint.sh"]
