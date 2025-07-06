#!/bin/sh
set -e

# 1. Pega o conteúdo do secret (que é uma string JSON) e o escreve em um arquivo.
echo "$GOOGLE_CREDENTIALS_JSON" > /app/gcp-credentials.json

# 2. Define a variável de ambiente que a biblioteca do Google procura.
export GOOGLE_APPLICATION_CREDENTIALS="/app/gcp-credentials.json"

# 3. Executa o comando original do seu Dockerfile para iniciar a aplicação .NET.
# O 'exec' garante que sua aplicação se torne o processo principal do container.
exec ./GestaoEscalaPermutas.Server