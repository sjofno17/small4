// TODO: Implement the order service
const amqp = require("amqplib/callback_api");
const fs = require('fs');
const moment = require('moment');

const { Order, OrderItem } = require("./data/db");

const messageBrokerInfo = {
    exchanges: {
      order: "order_exchange"
    },
    queues: {
        orderQueue: "order_queue"
    },
    routingKeys: {
      createOrder: "create_order"
    }
  };

  const createMessageBrokerConnection = () => new Promise((resolve, reject) => {
    amqp.connect('amqp://localhost', (err, conn) => {
        if (err) { reject(err); }
        resolve(conn);
    });
});

const configureMessageBroker = channel => {
    const { order } = messageBrokerInfo.exchanges;
    const { orderQueue } = messageBrokerInfo.queues;
    const { createOrder } = messageBrokerInfo.routingKeys;

    channel.assertExchange(order, "direct", { durable: true });
    channel.assertQueue(orderQueue, { durable: true });
    channel.bindQueue(orderQueue, order, createOrder);
};

const createChannel = connection => new Promise((resolve, reject) => {
    connection.createChannel((err, channel) => {
        if (err) { reject(err); }
        resolve(channel);
    });
});

(async () => {

    const connection = await createMessageBrokerConnection();
    const channel = await createChannel(connection);

    const { order } = messageBrokerInfo.exchanges;
    const { orderQueue } = messageBrokerInfo.queues;

    channel.consume(orderQueue, data => {
       
        
    }, { noAck: true });
})().catch(e => console.error(e));