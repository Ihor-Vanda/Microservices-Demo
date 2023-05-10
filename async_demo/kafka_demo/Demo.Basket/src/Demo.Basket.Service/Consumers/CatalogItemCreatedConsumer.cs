using System.Text.Json;
using Confluent.Kafka;
using Demo.Bascket.Service.Entities;
using Demo.Catalog.Contracts;
using Demo.Common;

namespace Demo.Bascket
{
    public class CatalogItemCreatedConsumer : BackgroundService
    {
        private readonly string topic = "topic-create";
        private readonly string groupId = "demo";
        private readonly string bootstrapServers = "localhost:9092";
        public readonly IRepository<CatalogItem> repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => Start(stoppingToken));
            return Task.CompletedTask;
        }

        private async void Start(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
                consumerBuilder.Subscribe(topic);
                var cancelToken = new CancellationTokenSource();

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumer = consumerBuilder.Consume(cancelToken.Token);
                        var message = JsonSerializer.Deserialize<CatalogItemCreated>(consumer.Message.Value);
                        if (message != null)
                        {
                            var item = await repository.GetAsync(message.ItemId);

                            if (item != null)
                            {
                                return;
                            }

                            item = new CatalogItem
                            {
                                Id = message.ItemId,
                                Name = message.name,
                                Description = message.Description
                            };

                            await repository.CreateAsync(item);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumerBuilder.Close();
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        // public async Task StartAsync(CancellationToken cancellationToken)
        // {
        //     var config = new ConsumerConfig
        //     {
        //         GroupId = groupId,
        //         BootstrapServers = bootstrapServers,
        //         AutoOffsetReset = AutoOffsetReset.Earliest
        //     };

        //     try
        //     {
        //         var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
        //         consumerBuilder.Subscribe(topic);
        //         var cancelToken = new CancellationTokenSource();

        //         try
        //         {
        //             while (true)
        //             {
        //                 var consumer = consumerBuilder.Consume(cancelToken.Token);
        //                 var message = JsonSerializer.Deserialize<CatalogItemCreated>(consumer.Message.Value);
        //                 if (message != null)
        //                 {
        //                     var item = await repository.GetAsync(message.ItemId);

        //                     if (item != null)
        //                     {
        //                         return;
        //                     }

        //                     item = new CatalogItem
        //                     {
        //                         Id = message.ItemId,
        //                         Name = message.name,
        //                         Description = message.Description
        //                     };

        //                     await repository.CreateAsync(item);
        //                 }
        //             }
        //         }
        //         catch (OperationCanceledException)
        //         {
        //             consumerBuilder.Close();
        //         }

        //     }
        //     catch (Exception e)
        //     {
        //         System.Diagnostics.Debug.WriteLine(e.Message);
        //     }
        // }

        // public Task StopAsync(CancellationToken cancellationToken)
        // {
        //     return Task.CompletedTask;
        // }
    }
}