import unittest
from confluent_kafka import Producer, Consumer, KafkaError
from dotenv import load_dotenv
from faker import Faker
import os
import json

# Carregar as configurações do .env
load_dotenv()

# Configurações Kafka
BOOTSTRAP_SERVERS = os.getenv('BOOTSTRAP_SERVERS')
TOPIC_NAME = os.getenv('TOPIC_NAME')
CLIENT_ID = os.getenv('CLIENT_ID')
GROUP_ID = os.getenv('GROUP_ID')
AUTO_OFFSET_RESET = os.getenv('AUTO_OFFSET_RESET')


class KafkaTests(unittest.TestCase):
    fake: Faker = Faker()

    def __generate_mock_message(self):
        """Gera uma mensagem mock de acordo com o schema"""
        return {
            'data': {
                'documento': self.fake.ssn(),  # Documento único
                'nome': self.fake.name(),
                'dataNascimento': self.fake.date_of_birth().isoformat(),
                'email': self.fake.email(),
                'telefone': self.fake.phone_number(),
                'endereco': {
                    'logradouro': self.fake.street_address(),
                    'cidade': self.fake.city(),
                    'estado': self.fake.state_abbr(),
                    'cep': self.fake.postcode()
                }
            }
        }

    def setUp(self):
        """Configurações comuns para os testes"""
        self.producer_config = {
            'bootstrap.servers': BOOTSTRAP_SERVERS,
            'client.id': CLIENT_ID
        }
        self.consumer_config = {
            'bootstrap.servers': BOOTSTRAP_SERVERS,
            'group.id': GROUP_ID,
            'auto.offset.reset': AUTO_OFFSET_RESET
        }

    @staticmethod
    def _delivery_report(err, msg):
        '''Callback chamado após o envio da mensagem'''
        if err is not None:
            print(f'Erro ao produzir mensagem: {err}')
        else:
            print(f'Mensagem enviada para {msg.topic()} [{msg.partition()}] @offset {msg.offset()}')
            return msg

    __status = None

    def test_produce_message(self):
        '''Teste de produção de mensagens'''
        producer = Producer(self.producer_config)

        # Gerar mensagem mock
        mock_message = self.__generate_mock_message()
        message_key = mock_message['data']['documento']

        # Produzir mensagem
        def delivery_report(err, msg):
            self.__status = self._delivery_report(err, msg)

        producer.produce(
            TOPIC_NAME,
            key=message_key,
            value=json.dumps(mock_message),
            callback=delivery_report
        )
        producer.flush()

        # Verificar que o callback foi chamado com sucesso
        print(self.__status)
        self.assertIsNotNone(self.__status)

    def test_consume_messages(self):
        '''Teste de consumo de mensagens'''
        producer = Producer(self.producer_config)
        consumer = Consumer(self.consumer_config)

        # Gerar mensagens mock
        mock_messages = [self.__generate_mock_message() for _ in range(5)]

        # Produzir mensagens
        for message in mock_messages:
            producer.produce(
                TOPIC_NAME,
                key=message['data']['documento'],
                value=json.dumps(message)
            )
        producer.flush()

        # Consumir mensagens
        consumer.subscribe([TOPIC_NAME])
        consumed_messages = []
        while len(consumed_messages) < 5:
            msg = consumer.poll(timeout=1.0)
            if msg is None:
                continue
            if msg.error():
                if msg.error().code() != KafkaError._PARTITION_EOF:
                    self.fail(f'Erro no consumidor: {msg.error()}')
                continue
            # Verifica se a mensagem não está vazia antes de processar
            if msg.value() is not None:
                consumed_messages.append(json.loads(msg.value().decode('utf-8')))

        # Verificar se todas as mensagens mock foram consumidas
        self.assertEqual(len(consumed_messages), 5)
        for mock_message in mock_messages:
            self.assertIn(mock_message, consumed_messages)

        consumer.close()


if __name__ == '__main__':
    unittest.main()
