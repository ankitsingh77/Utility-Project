// -----------------------------------------------------------------------
// <copyright file="Drive.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace FileSystemInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Security.Permissions;
    using System.Data.SqlClient;
    using System.Data;
    using System.Security.Cryptography;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Drive
    {
        string driveName;
        FileSystemWatcher watcher;
        public Boolean tableCreated = false;
        public RabbitMQHelper rabbitMQhelper;
        public Drive(string driveName,bool isInsertRequest)
        {
            this.driveName = driveName;
            this.Create(driveName + "_DirectoryDetails", "Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123");
            this.Create(driveName + "_FileDetails", "Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123");
            if(!isInsertRequest)
            rabbitMQhelper = new RabbitMQHelper(driveName);
        }

        
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run(string Path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;
            // Add event handlers.
            // watcher.Changed += new FileSystemEventHandler(OnChanged);
           // worker();
            watcher.Created += new FileSystemEventHandler(this.OnChanged);
            watcher.Deleted += new FileSystemEventHandler(this.OnChanged);
            watcher.Renamed += new RenamedEventHandler(this.OnRenamed);
            Thread workerThread = new Thread(new ThreadStart(this.worker));
            workerThread.Start();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            rabbitMQhelper.pushMessage(e,false);
            //Helper.UpdateDatabse(this.driveName,e.FullPath, e.Name, e.ChangeType,this.CreateMd5ForFolder(e.FullPath));
        }

        private void OnRenamed(object source, FileSystemEventArgs e)
        {
            rabbitMQhelper.pushMessage(e, true);
            //RenamedEventArgs revg = (RenamedEventArgs)e;
            //Helper.UpdateDatabse(this.driveName, e.FullPath, e.Name, e.ChangeType, this.CreateMd5ForFolder(revg.OldFullPath));
        }

        public void Create(string TName, string ConString)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES (NOLOCK) WHERE TABLE_NAME = '"+TName+"') "
                    + "BEGIN "
                    +" CREATE TABLE [dbo].[" + TName + "]("
                                + "[Name] [nvarchar](max) NOT NULL,"
                                + "[Size] [bigint] NOT NULL,"
                                + "[Path] [nvarchar](max) NOT NULL,"
                                + "[HashCode] [varchar](42) NOT NULL Primary key )"
                                + "END"
                                , new SqlConnection(ConString)))
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    this.tableCreated = true;
                }
            }
            catch (Exception)
            {

                this.tableCreated = false;
            }
        }

        public string CreateMd5ForFolder(string path)
        {
            MD5 md5 = MD5.Create();

            md5.ComputeHash(Encoding.UTF8.GetBytes(path));

            return String.Concat(path.Substring(path.Length - (path.Length > 6 ? 6 : 3), (path.Length > 6 ? 6 : 3)), BitConverter.ToString(md5.Hash).Replace("-", ""));
        }

        public void worker()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(driveName+"_Queue", true, false, true, null);

                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(driveName + "_Queue", false, consumer);
                    while (true)
                    {
                        var ea =
                            (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);
                        string[] fileinfo = message.Split('?');
                        string oldPath = fileinfo[3];
                        if (oldPath == "##")
                        {
                            oldPath = fileinfo[0];
                        }
                        Helper.UpdateDatabse(this.driveName, fileinfo[0], fileinfo[1], fileinfo[2], this.CreateMd5ForFolder(oldPath));
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }
    }
}
