# Kafka Test

Kafka test é um repositório onde contém a infraestrutura para a utilização do Kafka em ambiente de desenvolvimento. 
Além disso, este repositório contém:
- Inicialização de um tópico e seu avro no Schena Registry
- Projeto SpecFlow em .NET 8 para testar o consumo e a produção de mensagens usando o avro e o Schema Registry
- Script em Python 3.12 para testar o consumo e a produção de mensagens, porém sem usar o avro e o Schema Registry

## Installation

Com o Docker instalado na máquina, o primeiro passo é rodar o
init.sh.

```bash
sh init.sh
```

## Usage

Para .NET: 
```bash
cd 4oito6
cd 4oito6.Kafka
dotnet restore
dotnet build 
dotnet test
```

Para Python (no Windows): 
```bash
cd pykafka
python -m venv venv
source venv/Scripts/activate
pip install -r requirements.txt 
python -m unittests test_kafka.py
```

## Contributing

Pull requests são bem vindos. para maiores alterações, favor abrir uma issue primeiro para discutirmos o que será alterado. 

Favor garantir que os testes sejam devidamente atualizados. 

## License

[MIT](https://choosealicense.com/licenses/mit/)
