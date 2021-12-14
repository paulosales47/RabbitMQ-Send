using System;
using RabbitMQ.Client;
using System.Text;

public class Send {
    public static void Main()
    {
        var factory = new ConnectionFactory() {HostName = "localhost"};
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            for(int i = 0; i < 10; i++){
                string dots = new string('.', new Random().Next(1, 5));
                string message = $"{i}º Hello{dots}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: "task_queue",
                    basicProperties: null,
                    body: body
                );

                System.Console.WriteLine("[x] Sent {0}", message);
            }
        }

        Console.WriteLine("Press [enter] to exit");
        Console.ReadLine();
    }

}