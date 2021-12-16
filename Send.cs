using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;

public class Send {

    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly string? _replayQueueName;
    private readonly EventingBasicConsumer? _consumer;
    private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties? _props;
    

    public Send()
    {
        var factory = new ConnectionFactory() {HostName = "localhost"};
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _replayQueueName = _channel.QueueDeclare().QueueName;
        _consumer = new EventingBasicConsumer(_channel);

        _props = _channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        _props.CorrelationId = correlationId;
        _props.ReplyTo = _replayQueueName;

        _consumer.Received += (model, ea) => {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            if(ea.BasicProperties.CorrelationId == correlationId){
                _respQueue.Add(response);
            }
        };

        _channel.BasicConsume(consumer: _consumer, queue: _replayQueueName, autoAck: true);


        Console.WriteLine("Press [enter] to exit");
        Console.ReadLine();
    }

    public string Call(string message){

        var messageBytes = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: _props, body: messageBytes);

        return _respQueue.Take();
    }

    public void Close() => _connection!.Close();

}

public class Rpc{

    public static void Main(string[] args){
        var send = new Send();

        foreach(string arg in args){
            Console.WriteLine("[x] Requesting fib({0})", arg);
            var response = send.Call(arg);
            Console.WriteLine("[.] Got {0}", response);
        }

        send.Close();

    }

}