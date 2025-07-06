# Etapa 1: Build (NENHUMA MUDANÇA AQUI)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solução e projetos individualmente
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

# Publicar aplicação
WORKDIR /app/GestaoEscalaPermutas.Server
RUN dotnet publish -c Release -o /app/publish --runtime linux-x64 --self-contained false


# Etapa 2: Imagem final mínima (APENAS PEQUENOS AJUSTES AQUI)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /app/GestaoEscalaPermutas.Server/certificado.pfx /app/certificado.pfx


# ADIÇÃO 1: Copiar o script que criamos para dentro da imagem final.
COPY docker-entrypoint.sh .

# ==================================================================
# CORREÇÃO AQUI: Garante que o script tenha o formato de linha correto (LF)
RUN sed -i 's/\r$//' ./docker-entrypoint.sh
# ==================================================================

# Dá permissão de execução para o script
RUN chmod +x ./docker-entrypoint.sh

# Configurações extras
RUN chmod 644 /app/certificado.pfx
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:80

# Inicia o container usando o script
ENTRYPOINT ["/app/docker-entrypoint.sh"]
