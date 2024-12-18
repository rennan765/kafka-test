using _4oito6.informacoes_cliente_cadastrado;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _4oito6.Kafka.Drivers;

public static class IoC
{
    static IoC()
    {
        ServiceCollection services = new();

        services
            .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build());

        services
            .AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                return new AdminClientConfig
                {
                    BootstrapServers = configuration["AdminClientConfig:BootstrapServers"],
                    Debug = configuration["AdminClientConfig:Debug"],
                };
            })
            .AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                return new SchemaRegistryConfig
                {
                    Url = configuration["SchemaRegistryConfig:Url"]
                };
            })
            .AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                return new AvroSerializerConfig
                {
                    BufferBytes = Convert.ToInt32(configuration["AvroSerializerConfig:BufferBytes"])
                };
            })
            .AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                return new ProducerConfig
                {
                    BootstrapServers = configuration["ProducerConfig:BootstrapServers"],
                    ClientId = configuration["ProducerConfig:ClientId"],
                    Acks = (Acks)Convert.ToInt32(configuration["ProducerConfig:Acks"]),
                    EnableIdempotence = Convert.ToBoolean(configuration["ProducerConfig:EnableIdempotence"]),
                    EnableSslCertificateVerification = Convert.ToBoolean(configuration["ProducerConfig:EnableSslCertificateVerification"])
                };
            })
            .AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                return new ConsumerConfig
                {
                    BootstrapServers = configuration["ConsumerConfig:BootstrapServers"],
                    ClientId = configuration["ConsumerConfig:ClientId"],
                    GroupId = configuration["ConsumerConfig:GroupId"],
                    AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration["ConsumerConfig:AutoOffsetReset"]!),
                    EnableAutoCommit = Convert.ToBoolean(configuration["ConsumerConfig:EnableAutoCommit"])
                };
            });

        services
            .AddScoped<ISchemaRegistryClient>
            (
                sp => new CachedSchemaRegistryClient
                (
                    config: sp.GetRequiredService<SchemaRegistryConfig>()
                )
            );

        services
            .AddScoped(sp =>
            {
                AvroSerializer<InformacoesClienteCadastrado> avroSerializer = new
                (
                    schemaRegistryClient: sp.GetRequiredService<ISchemaRegistryClient>(),
                    config: sp.GetRequiredService<AvroSerializerConfig>()
                );

                return new ProducerBuilder<string, InformacoesClienteCadastrado>(sp.GetRequiredService<ProducerConfig>())
                    .SetValueSerializer(avroSerializer.AsSyncOverAsync())
                    .Build();
            });

        services
            .AddScoped(sp =>
            {
                AvroDeserializer<InformacoesClienteCadastrado> avroDeserializer = new
                (
                    schemaRegistryClient: sp.GetRequiredService<ISchemaRegistryClient>()
                );

                return new ConsumerBuilder<string, InformacoesClienteCadastrado>(sp.GetRequiredService<ConsumerConfig>())
                    .SetValueDeserializer(avroDeserializer.AsSyncOverAsync())
                    .Build();
            });

        Provider = services.BuildServiceProvider();
    }

    public static IServiceProvider Provider { get; private set; }
}