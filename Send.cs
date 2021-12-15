using System;
using RabbitMQ.Client;
using System.Text;

public class Send {
    public static void Main(string[] args)
    {
        if(args.Length < 1)
            return;

        var factory = new ConnectionFactory() {HostName = "localhost"};
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            
            channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

            for(int i = 0; i < 50; i++){
                string message = $"{i}º Hello";
                string topic = gererateSeverity(args[new Random().Next(0, args.Length)]);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "topic_logs",
                    routingKey: topic,
                    basicProperties: null,
                    body: body
                );

                System.Console.WriteLine("[x] Sent '{0}': '{1}'", message, topic);
            }
        }

        Console.WriteLine("Press [enter] to exit");
        Console.ReadLine();
    }

    public static string gererateSeverity(string facility){
        int typeMessage = new Random().Next(1, 9);

        if(typeMessage == 1)
            return $"{facility}.emerg";
        else if(typeMessage == 2)
            return $"{facility}.alert";
        else if(typeMessage == 3)
            return $"{facility}.crit";
        else if(typeMessage == 4)
            return $"{facility}.err";
        else if(typeMessage == 5)
            return $"{facility}.warn";
        else if(typeMessage == 6)
            return $"{facility}.notice";
        else if(typeMessage == 7)
            return $"{facility}.info";
        
        return $"{facility}.debug";
    }

}