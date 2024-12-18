using _4oito6.informacoes_cliente_cadastrado;
using _4oito6.Kafka.Drivers;
using _4oito6.Kafka.Support;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _4oito6.Kafka.StepDefinitions;

[Binding]
public class ProducerStepDefinitions : IClassFixture<InformacoesClienteCadastradoFixture>
{
    private readonly InformacoesClienteCadastradoFixture _fixture;
    private readonly IServiceScope _scope;

    private Message<string, InformacoesClienteCadastrado> _mensagem = null!;
    private DeliveryResult<string, InformacoesClienteCadastrado> _resultado = null!;

    public ProducerStepDefinitions(InformacoesClienteCadastradoFixture fixture)
    {
        _fixture = fixture;
        _scope = IoC.Provider.CreateScope();
    }

    [Given(@"a necessidade de validar se as mensagens estão sendo produzidas")]
    public void GivenANecessidadeDeValidarSeAsMensagensEstaoSendoProduzidas()
    {
        var mensagem = _fixture.Registos.FirstOrDefault()!;

        _mensagem = new()
        {
            Key = mensagem.data.documento,
            Value = mensagem
        };
    }

    [When(@"eu produzo as mensagens")]
    public async Task WhenEuProduzoAsMensagens()
    {
        var configuration = _scope.ServiceProvider
            .GetRequiredService<IConfiguration>();

        var producer = _scope.ServiceProvider
            .GetRequiredService<IProducer<string, InformacoesClienteCadastrado>>();

        _resultado = await producer
            .ProduceAsync(configuration["TopicName"]!, _mensagem)
            .ConfigureAwait(false);

        producer.Flush();
    }

    [Then(@"as mensagens deverão ser produzidas com sucesso")]
    public void ThenAsMensagensDeveraoSerProduzidasComSucesso()
    {
        _resultado.Status
            .Should().Be(PersistenceStatus.Persisted);
    }
}