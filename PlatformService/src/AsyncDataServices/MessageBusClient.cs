using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration configuration;
    private readonly IConnection? connection;
    private readonly IModel? channel;
    private const string EXCHANGE_NAME = "trigger";

    public MessageBusClient(IConfiguration configuration)
    {
        this.configuration = configuration;
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
          connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
          Console.WriteLine("--> Connected to RabbitMQ Message Bus");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not connnect to the Message Bus: {ex.Message}");
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> Connection to RabbitMQ Message Bus ShutDown");
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
      var message = JsonSerializer.Serialize(platformPublishedDto);
      if(connection != null && connection.IsOpen)
      {
        Console.WriteLine("--> RabbitMQ connection is open, sending message");
        SendMessage(message);
      }
      else
      {
        Console.WriteLine("--> RabbitMQ connection is closed, not sending message");
      }
    }

    private void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);
      channel.BasicPublish(exchange: EXCHANGE_NAME, 
                          routingKey: "",
                          basicProperties: null,
                          body: body);
      Console.WriteLine($"--> Message sent '{message}'");
    }

    public void Dispose()
    {
      Console.WriteLine("--> Message Bus connection disposing...");
      if (channel != null && channel.IsOpen)
      {
        channel.Close();
      }

      if (connection != null && connection.IsOpen)
      {
        connection.Close();
      }
      Console.WriteLine("--> Message Bus connection disposed");
    }
}