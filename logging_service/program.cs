/*
LOGGING SERVICE (40%)
It should be implemented as a console application in .NET Core using RabbitMQ
1. (10%) It should consume the order created event using the queue “logging_queue”
2. (30%) It should store the data within the logging event prefixed with “Log: “ within a file called log.txt
*/
using System;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.IO;
using System.Text;

namespace logging_service
{
    class Program
    {
        static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel()) {

                var orderExchange = "order_exchange";
                var loggingQueue = "logging_queue";
                var createOrder = "create_order";

                channel.ExchangeDeclare(
                        exchange: orderExchange, 
                        type: "direct", 
                        durable: true
                );

                channel.QueueDeclare(
                        queue: loggingQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                );

                channel.QueueBind(  	
                    queue: loggingQueue, 
                    exchange: orderExchange, 
                    routingKey: createOrder
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {
                    var data = Encoding.UTF8.GetString(ea.Body);
                    using (StreamWriter write = File.AppendText("log.txt")) {
                        write.WriteLine("Log: " + data);
                    };
                };

                channel.BasicConsume(
                    queue: loggingQueue, 
                    autoAck: true, 
                    consumer: consumer
                );               
                Console.ReadLine();
            };
        }
    };
};