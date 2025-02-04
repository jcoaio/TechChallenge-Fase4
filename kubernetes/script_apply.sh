#!/bin/bash

# Aplica o namespace primeiro
kubectl apply -f namespace.yaml

# Função para aplicar todos os arquivos YAML em um diretório
apply_yaml_files() {
    for file in "$1"/*.yaml; do
        if [[ -f $file ]]; then
            echo "Aplicando $file..."
            kubectl apply -f "$file"
        fi
    done
}

# Diretórios a serem processados
directories=("Api" "Bus" "Consumer" "Database" "Grafana" "Prometheus" "Secrets")

# Aplica os arquivos YAML de cada diretório
for dir in "${directories[@]}"; do
    apply_yaml_files "$dir"
done

echo "Todos os arquivos YAML foram aplicados."

