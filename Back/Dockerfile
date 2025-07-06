# Etapa 1: Build
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

# Publicar aplicação no modo autossuficiente
RUN dotnet publish "GestaoEscalaPermutas.Server/GestaoEscalaPermutas.Server.csproj" -c Release -o /app/publish /p:PublishTrimmed=false


RUN find /app/publish -name "*Microsoft.AspNetCore.Http.Abstractions*"

# ==================================================================
# PASSO DE DEPURAÇÃO 1: Listar o conteúdo da pasta de publicação
# Vamos verificar se 'Microsoft.AspNetCore.Http.Abstractions.dll' está aqui.
# ==================================================================
RUN echo "--- Conteúdo de /app/publish ---" && ls -la /app/publish


# Etapa 2: Imagem final
# Usando a imagem base correta para builds autossuficientes
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar os arquivos publicados da etapa de build
COPY --from=build /app/publish .

# Copiar o certificado e o script de entrada
COPY --from=build /app/GestaoEscalaPermutas.Server/certificado.pfx /app/certificado.pfx
COPY --from=build /app/docker-entrypoint.sh .

# Preparar o script de entrada
RUN sed -i 's/\r$//' ./docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh
RUN chmod 644 /app/certificado.pfx

# ==================================================================
# PASSO DE DEPURAÇÃO 2: Listar o conteúdo da pasta final da aplicação
# Vamos verificar se tudo foi copiado corretamente.
# ==================================================================
RUN echo "--- Conteúdo final de /app ---" && ls -la .

# Configurar e executar a aplicação
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["/app/docker-entrypoint.sh"]
