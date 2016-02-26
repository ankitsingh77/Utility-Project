// -----------------------------------------------------------------------
// <copyright file="RabbitMQHelper.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace FileSystemInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RabbitMQHelper
    {
        private string queueName;
        ConnectionFactory factory;
        IConnection connection;
        IModel channel;
        IBasicProperties properties;
        public RabbitMQHelper(string driveName)
        {
            this.queueName = driveName +"_Queue";
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queueName, true, false, true, null);
            properties = channel.CreateBasicProperties();
            properties.SetPersistent(true);
        }

        public void pushMessage(FileSystemEventArgs file, bool isRenamed)
        {
            string message = file.FullPath +"?" +file.Name + "?" +file.ChangeType +"?"+ (isRenamed ? ((RenamedEventArgs)file).OldFullPath : "##");
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", queueName, properties, body);
        }
    }
}
