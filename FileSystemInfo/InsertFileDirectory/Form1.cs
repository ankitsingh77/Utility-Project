using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FileSystemInfo;
using Bally.Application.Logger;
using System.Data.SqlClient;


namespace InsertFileDirectory
{
    public partial class Form1 : Form
    {
        RabbitMQHelper rbq;
        ILogger logger = new Logger(ErrorLevel.Debug);
        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, System.EventArgs e)
        {
             DriveInfo[] drives = DriveInfo.GetDrives();
             this.cbDrive.DataSource = drives.Where(o => o.DriveType == DriveType.Fixed).ToList();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string driveName = this.cbDrive.SelectedItem.ToString().Substring(0, 1);
            this.Create(driveName + "_DirectoryDetails", "Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123");
            this.Create(driveName + "_FileDetails", "Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123");
            rbq = new RabbitMQHelper(driveName);
            CreateFileSystemEntries(new DirectoryInfo(this.cbDrive.SelectedItem.ToString()));
            //FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.Created,
        }

        public void Create(string TName, string ConString)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES (NOLOCK) WHERE TABLE_NAME = '" + TName + "') "
                    + "BEGIN "
                    + " CREATE TABLE [dbo].[" + TName + "]("
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
                }
            }
            catch (Exception)
            {
            }
        }

        private long CreateFileSystemEntries(DirectoryInfo DI)
        {
            long size = 0;
            try
            {
                DirectoryInfo[] subDirecotries = DI.GetDirectories();
                foreach (var sd in subDirecotries)
                {
                    size = size + CreateFileSystemEntries(sd);
                }

                FileInfo[] files = DI.GetFiles();

                foreach (var file in files)
                {
                    size = size + file.Length;
                    FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.Created, file.DirectoryName, file.Name);
                    rbq.pushMessage(e, false);
                }

                FileSystemEventArgs directory = new FileSystemEventArgs(WatcherChangeTypes.Created, DI.Parent != null ? DI.Parent.FullName : DI.FullName, DI.Name);
                rbq.pushMessage(directory, false);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Log("Unauthorized_Exception", ex);
            }
            catch (Exception ex)
            {
                logger.Log("Exception", ex);
            }
            return size;

        }
    }
}
