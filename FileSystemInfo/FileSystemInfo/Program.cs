using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;

namespace FileSystemInfo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //if(args[0].ToUpper() == "NOSERVICE")
            //{
            //    DriveInfo[] drives = DriveInfo.GetDrives();
            //    drives = drives.Where(o => o.DriveType.Equals(DriveType.Fixed | DriveType.Removable)).ToArray();
            //    foreach (var drive in drives)
            //    {
            //        Drive d = new Drive(drive.Name.Substring(0, 1),false);
            //        d.Run(drive.Name);
            //        Console.WriteLine("");
            //    }
            //    Console.ReadLine();
            //}
            //else
            //{
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new Service1() 
			};
            ServiceBase.Run(ServicesToRun);
            //}
        }
    }
}
