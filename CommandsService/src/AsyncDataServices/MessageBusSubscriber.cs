
using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration configuration;
    private readonly IEventProcessor eventProcessor;
    private IConnection? connection;
    private IModel? channel;
    private string? queueName;
    private const string EXCHANGE_NAME = "trigger";

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        this.configuration = configuration;
        this.eventProcessor = eventProcessor;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
      var factory = new ConnectionFactory()
      {
          HostName = configuration["RabbitMQHost"],
          Port = int.Parse(configuration["RabbitMQPort"] ?? "5672")
      };

      try
      {
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: ExchangeType.Fanout);
        queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: EXCHANGE_NAME, routingKey: "");
        
        connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        Console.WriteLine("--> Listenning to RabbitMQ Message Bus");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not listen to RabbitMQ Message Bus: {ex.Message}");
      }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();
      
      var consumer = new EventingBasicConsumer(channel);
      consumer.Received += (moduleHandle, basicDeliverEventArgs) =>
      {
        Console.WriteLine($"--> Event Received!");
        
        var body = basicDeliverEventArgs.Body;
        var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

        eventProcessor.ProcessEvent(notificationMessage);
      };

      channel!.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

      return Task.CompletedTask;
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"--> Shutdown listenning to RabbitMQ Message Bus");
    }

    public override void Dispose()
    {
      Console.WriteLine("--> Disposing listenning to RabbitMQ Message Bus...");
      if (channel != null && channel.IsOpen)
      {
        channel.Close();
      }

      if (connection != null && connection.IsOpen)
      {
        connection.Close();
      }

      base.Dispose();
      Console.WriteLine("--> Disposed listenning to RabbitMQ Message Bus...");
    }
}