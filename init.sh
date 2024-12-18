#!/bin/bash

echo -e 'Inicializando containers'
docker-compose up -d

echo -e 'Aguardando inicialização dos containers'
sleep 20

chmod +x jq-files/jq-windows-amd64
chmod +x jq-files/jq-linux-amd64

JQ_BIN='./jq-files/jq-windows-amd64'

if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    JQ_BIN='./jq-files/jq-linux-amd64'
fi

SUBJECT="4oito6"

INFORMACOES_CLIENTE_CADASTRO_NOME_TOPICO='informacoes-cliente-cadastrado'
echo -e 'Criando topico informacoes-cliente-cadastrado'

docker exec -i kafka1 kafka-topics \
    --bootstrap-server localhost:9092 \
    --create --replication-factor 1 --partitions 1 \
    --topic $INFORMACOES_CLIENTE_CADASTRO_NOME_TOPICO \
    --if-not-exists

echo -e 'Topico informacoes-cliente-cadastrado criado'

echo -e 'Cadastrando topico informacoes-cliente-cadastrado no schema registry'
INFORMACOES_CLIENTE_CADASTRO_AVRO=$($JQ_BIN -c '.' <./schemas/_4oito6.informacoes_cliente_cadastrado.asvc)

curl -X POST http://localhost:8081/subjects/$SUBJECT/versions \
    -H "Content-Type: application/vnd.schemaregistry.v1+json" \
    --data "{\"schema\": \"$(echo "$INFORMACOES_CLIENTE_CADASTRO_AVRO" | sed 's/"/\\"/g')\"}"
    
echo -e 'Cadastro do topico informacoes-cliente-cadastrado no schema registry efetuado'

sleep 2