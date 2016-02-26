using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Security.Permissions;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Configuration;

namespace FileSystemInfo
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                drives = drives.Where(o => o.DriveType.Equals(DriveType.Fixed | DriveType.Removable)).ToArray();
                foreach (var drive in drives)
                {
                    Drive d = new Drive(drive.Name.Substring(0, 1),false);
                    d.Run(drive.Name);
                }
            }
            catch (Exception ex)
            {
                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
                {
                    SqlCommand command = new SqlCommand("proc_inserterror", conn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Message", ex.Message));
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            //String drives = System.Configuration.ConfigurationManager.AppSettings["Drives"].ToString();
            //String[] paths = drives.Split(';');
            //foreach (var path in paths)
            //{
            //    Run(path);
            //}
        }

        protected override void OnStop()
        {
        }

        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        //public void Run(string Path)
        //{
        //    FileSystemWatcher watcher = new FileSystemWatcher();
        //    watcher.Path = Path;
        //    watcher.IncludeSubdirectories = true;
        //    watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
        //    watcher.EnableRaisingEvents = true;
        //    // Add event handlers.
        //   // watcher.Changed += new FileSystemEventHandler(OnChanged);
        //    watcher.Created += new FileSystemEventHandler(OnChanged);
        //    watcher.Deleted += new FileSystemEventHandler(OnChanged);
        //    watcher.Renamed += new RenamedEventHandler(OnRenamed);

        //}

        //private static void OnChanged(object source, FileSystemEventArgs e)
        //{
        //    UpdateDatabse(e.FullPath, e.Name, e.ChangeType);
        //    // Specify what is done when a file is changed, created, or deleted.
        //    //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        //}

        //private static void OnRenamed(object source, FileSystemEventArgs e)
        //{
        //    UpdateDatabse(e.FullPath, e.Name, e.ChangeType);
        //    // Specify what is done when a file is changed, created, or deleted.
        //    //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        //}

        //private static  void UpdateDatabse(string path, string FileName,WatcherChangeTypes changeType)
        //{
        //    string fileName = FileName.Substring(FileName.LastIndexOf('\\')+1,FileName.Length - FileName.LastIndexOf('\\')-1);
        //    if (changeType == WatcherChangeTypes.Deleted)
        //    {
        //        using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
        //        {
        //            conn.Open();
        //            SqlDataAdapter dAdaptor = new SqlDataAdapter("Select * from SpaceDetails", conn);
        //            dAdaptor.InsertCommand = new
        //                SqlCommand("proc_DeleteFileorDirectory", conn);
        //            SqlCommand cmdInsert = dAdaptor.InsertCommand;
        //            cmdInsert.CommandType = CommandType.StoredProcedure;
        //            cmdInsert.Parameters.Add(new SqlParameter("@HashCode",
        //                                                     CreateMd5ForFolder(path)));
        //            dAdaptor.InsertCommand.ExecuteNonQuery();
        //            conn.Close(); 
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {

        //            FileAttributes attr = File.GetAttributes(path);
        //            if (attr.HasFlag(FileAttributes.Directory))
        //            {
        //                DirectoryInfo di = new DirectoryInfo(path);
        //                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
        //                {
        //                    conn.Open();
        //                    SqlDataAdapter dAdaptor = new SqlDataAdapter("Select * from SpaceDetails", conn);

        //                    dAdaptor.InsertCommand = new
        //                        SqlCommand("proc_InsertDisrectory", conn);
        //                    SqlCommand cmdInsert = dAdaptor.InsertCommand;
        //                    cmdInsert.CommandType = CommandType.StoredProcedure;
        //                    cmdInsert.Parameters.Add(new SqlParameter("@DirectoryName", fileName));

        //                    cmdInsert.Parameters.Add(new SqlParameter("@size",
        //                                                 Convert.ToInt64(DircetorySize(di))));

        //                    cmdInsert.Parameters.Add(new SqlParameter("@Path",
        //                                                 path));
        //                    cmdInsert.Parameters.Add(new SqlParameter("@HashCode",
        //                                                             CreateMd5ForFolder(path)));
        //                    dAdaptor.InsertCommand.ExecuteNonQuery();
        //                    conn.Close();
        //                }
        //            }
        //            else
        //            {
        //                FileInfo di = new FileInfo(path);

        //                using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
        //                {
        //                    conn.Open();
        //                    SqlDataAdapter dAdaptor = new SqlDataAdapter("Select * from FileDetails", conn);

        //                    dAdaptor.InsertCommand = new
        //                        SqlCommand("proc_InsertFiles", conn);
        //                    SqlCommand cmdInsert = dAdaptor.InsertCommand;
        //                    cmdInsert.CommandType = CommandType.StoredProcedure;
        //                    cmdInsert.Parameters.Add(new SqlParameter("@FileName", fileName));

        //                    cmdInsert.Parameters.Add(new SqlParameter("@size",
        //                                                 Convert.ToInt64(di.Length)));

        //                    cmdInsert.Parameters.Add(new SqlParameter("@Path",
        //                                                 path));
        //                    cmdInsert.Parameters.Add(new SqlParameter("@HashCode",
        //                                                             CreateMd5ForFolder(path)));
        //                    dAdaptor.InsertCommand.ExecuteNonQuery();
        //                    conn.Close();
        //                }
        //            }
        //        }
        //        catch (FileNotFoundException)
        //        {

        //        }
        //        catch (UnauthorizedAccessException)
        //        {
        //        }
        //    }
        //}

        //public static long DircetorySize(DirectoryInfo DI)
        //{
        //    long size = 0;
        //    try
        //    {
        //        DirectoryInfo[] SubDirectories = DI.GetDirectories();
        //        if (SubDirectories.Length != 0)
        //        {
        //            foreach (var sb in SubDirectories)
        //            {
        //                size = size + DircetorySize(sb);
        //            }
        //        }
        //        FileInfo[] files = DI.GetFiles();
        //        if (files.Length != 0)
        //        {
        //            foreach (var file in files)
        //            {
        //                size = size + file.Length;
        //            }
        //        }
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        Console.WriteLine("File not accessible");
        //    }
        //    return size;
        //}

        //public static string CreateMd5ForFolder(string path)
        //{
        //    MD5 md5 = MD5.Create();

        //    md5.ComputeHash(Encoding.UTF8.GetBytes(path));

        //    return String.Concat(path.Substring(path.Length - (path.Length > 6 ? 6 : 3), (path.Length > 6 ? 6 : 3)), BitConverter.ToString(md5.Hash).Replace("-", ""));
        //}
    }
}
