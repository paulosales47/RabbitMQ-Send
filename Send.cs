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
            
            channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

            for(int i = 0; i < 10; i++){
                string message = $"{i}º Hello";
                string severity = gererateSeverity();
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "direct_logs",
                    routingKey: severity,
                    basicProperties: null,
                    body: body
                );

                System.Console.WriteLine("[x] Sent '{0}': '{1}'", message, severity);
            }
        }

        Console.WriteLine("Press [enter] to exit");
        Console.ReadLine();
    }

    public static string gererateSeverity(){
        int typeMessage = new Random().Next(1, 4);

        if(typeMessage == 1)
            return "error";
        else if(typeMessage == 2)
            return "warning";
        
        return "info";
    }

}