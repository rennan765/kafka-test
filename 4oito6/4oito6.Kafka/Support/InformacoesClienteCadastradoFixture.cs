using _4oito6.informacoes_cliente_cadastrado;
using AutoBogus;

namespace _4oito6.Kafka.Support;

public class InformacoesClienteCadastradoFixture
{
    private readonly AutoFaker<Cliente> _autoFaker = new();

    public InformacoesClienteCadastradoFixture()
    {
        MockarRegistros();
    }

    public List<InformacoesClienteCadastrado> Registos { get; init; } = new();

    public void MockarRegistros(int quantidade = 1)
    {
        Registos.Clear();

        Registos
            .AddRange(_autoFaker
                .RuleFor(x => x.documento, f => f.Random.Long(10000000000, 99999000000).ToString())
                .RuleFor(x => x.endereco, f => new()
                {
                    bairro = f.Random.String2(15),
                    cep = f.Random.Long(10000000, 30000000).ToString(),
                    cidade = f.Random.String2(10),
                    estado = f.Random.String2(2),
                    logradouro = f.Random.String2(30),
                    numero = f.Random.Bool() ? f.Random.Int(1, 9999).ToString() : null!
                })
                .RuleFor(x => x.telefone, f => new()
                {
                    ddd = f.Random.Int(10, 99).ToString(),
                    numero = f.Random.Long(20000000, 999999999).ToString()
                })
                .Generate(quantidade)
                .Select(data => new InformacoesClienteCadastrado
                {
                    data = data
                }));
    }
}