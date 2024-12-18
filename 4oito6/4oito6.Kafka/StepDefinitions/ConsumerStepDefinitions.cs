using _4oito6.informacoes_cliente_cadastrado;
using _4oito6.Kafka.Drivers;
using _4oito6.Kafka.Support;
using Confluent.Kafka;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _4oito6.Kafka.StepDefinitions;

[Binding]
public class ConsumerStepDefinitions : IClassFixture<InformacoesClienteCadastradoFixture>
{
    private readonly InformacoesClienteCadastradoFixture _fixture;
    private readonly IServiceScope _scope;

    private readonly List<Message<string, InformacoesClienteCadastrado>> _mensagensOriginais = new();
    private readonly Dictionary<string, ConsumeResult<string, InformacoesClienteCadastrado>> _mensagensConsumidas = new();

    public ConsumerStepDefinitions(InformacoesClienteCadastradoFixture fixture)
    {
        _fixture = fixture;
        _scope = IoC.Provider.CreateScope();
    }

    [Given(@"a necessidade de validar se existem mensagens no tópico")]
    public async Task GivenANecessidadeDeValidarSeExistemMensagensNoTopico()
    {
        _fixture.MockarRegistros(quantidade: 5);

        _mensagensOriginais
            .AddRange(_fixture.Registos
                .Select(mensagem => new Message<string, InformacoesClienteCadastrado>
                {
                    Key = mensagem.data.documento,
                    Value = mensagem
                }));

        var configuration = _scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var producer = _scope.ServiceProvider
            .GetRequiredService<IProducer<string, InformacoesClienteCadastrado>>();

        await Task
            .WhenAll(_mensagensOriginais.Select(m => producer.ProduceAsync(configuration["TopicName"]!, m)))
            .ConfigureAwait(false);

        producer.Flush();
    }

    [When(@"eu consumo as mensagens")]
    public void WhenEuConsumoAsMensagens()
    {
        var configuration = _scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var consumer = _scope.ServiceProvider
            .GetRequiredService<IConsumer<string, InformacoesClienteCadastrado>>();

        var topicName = configuration["TopicName"];

        consumer.Subscribe(topicName);

        var watermark = consumer
            .QueryWatermarkOffsets(new TopicPartition(topicName, new Partition(0)), TimeSpan.FromSeconds(5));

        for (var i = 0; i < (int)watermark.High; i++)
        {
            var message = consumer.Consume();
            if (message is not null)
            {
                _mensagensConsumidas.Add(message.Message.Key, message);
            }
        }
    }

    [Then(@"as mensagens deverão ser consumidas com suecsso")]
    public void ThenAsMensagensDeveraoSerConsumidasComSuecsso()
    {
        _mensagensConsumidas.Count
            .Should().BeGreaterThanOrEqualTo(_mensagensOriginais.Count);

        CompareLogic comparison = new();

        foreach (var mensagemOriginal in _mensagensOriginais)
        {
            var isMensagemExiste = _mensagensConsumidas.TryGetValue(mensagemOriginal.Key, out var mensagemConsumida);
            isMensagemExiste.Should().BeTrue();

            comparison.Compare(mensagemOriginal.Value, mensagemConsumida!.Message.Value).AreEqual
                .Should().BeTrue();
        }
    }
}