using System;
using System.Threading.Tasks;
using My.Messages;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "SimpleSaga";
        var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSaga");

        #region config

        endpointConfiguration.UsePersistence<LearningPersistence>();
        var transport = endpointConfiguration.UseTransport(new LearningTransport());
        //comment out and it works
        transport.RouteToEndpoint(typeof(MyMessage).Assembly, "Samples.SimpleSaga");
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine();
        Console.WriteLine("Storage locations:");
        Console.WriteLine($"Learning Persister: {LearningLocationHelper.SagaDirectory}");
        Console.WriteLine($"Learning Transport: {LearningLocationHelper.TransportDirectory}");

        Console.WriteLine();
        Console.WriteLine("Press 'Enter' to send a StartOrder message");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            Console.WriteLine();
            if (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                break;
            }
            var orderId = Guid.NewGuid();
            var startOrder = new StartOrder
            {
                OrderId = orderId
            };
            await endpointInstance.SendLocal(startOrder);
            Console.WriteLine($"Sent StartOrder with OrderId {orderId}.");
        }

        await endpointInstance.Stop();
    }
}