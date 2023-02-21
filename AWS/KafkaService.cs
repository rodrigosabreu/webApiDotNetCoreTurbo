using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.AWS
{
    public class KafkaService : IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;

        public KafkaService(ConsumerConfig config)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public async Task<List<string>> ConsumerAsync(string topic, int count)
        {
            var result = new List<string>();

            _consumer.Subscribe(topic);

            try
            {
                while (true)
                {
                    var cr = _consumer.Consume();                    
                    result.Add(cr.Value);
                    break;
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Ocorreu um erro: {e.Error.Reason}");                
            }
            return result;
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}