// -----------------------------------------------------------------------
// <copyright file="Helper.cs" company="Microsoft">
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
    using System.Data.SqlClient;
    using System.Data;
    using System.Security.Cryptography;
    using Bally.Application.Logger;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Helper
    {
        public static ILogger logger = new Logger(ErrorLevel.Debug);

        public static void UpdateDatabse(string driveName,string path, string FileName, string changeType,string hashCode)
        {
            
            string fileName = FileName.Substring(FileName.LastIndexOf('\\') + 1, FileName.Length - FileName.LastIndexOf('\\') - 1);
            if (changeType == "Deleted")
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
                    {

                        SqlCommand command = new SqlCommand("proc_DeleteFileorDirectory", conn);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@DriveName",
                                                                 driveName));
                        command.Parameters.Add(new SqlParameter("@HashCode",
                                                               hashCode));
                        conn.Open();
                        command.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                catch(Exception ex)
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
                    logger.Log("DeleteError", ex);

                }
            }
            else if (changeType == "Renamed")
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(path);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
                        {
                            SqlCommand command = new SqlCommand("proc_RenameDirectory", conn);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@DriveName", driveName));
                            command.Parameters.Add(new SqlParameter("@DirectoryName", fileName));

                            command.Parameters.Add(new SqlParameter("@size",
                                                         Convert.ToInt64(DircetorySize(di))));

                            command.Parameters.Add(new SqlParameter("@Path",
                                                         path));
                            command.Parameters.Add(new SqlParameter("@HashCode",
                                                                     CreateMd5ForFolder(path)));
                            command.Parameters.Add(new SqlParameter("@OldHashCode",hashCode));
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    else
                    {
                        FileInfo di = new FileInfo(path);

                        using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
                        {
                            SqlCommand command = new SqlCommand("proc_RenameFiles", conn);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@DriveName", driveName));
                            command.Parameters.Add(new SqlParameter("@FileName", fileName));

                            command.Parameters.Add(new SqlParameter("@size",
                                                         di.Length));

                            command.Parameters.Add(new SqlParameter("@Path",
                                                         path));
                            command.Parameters.Add(new SqlParameter("@HashCode",
                                         CreateMd5ForFolder(path)));
                            command.Parameters.Add(new SqlParameter("@OldHashCode", hashCode));
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                catch (FileNotFoundException ex)
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
                    logger.Log("Rename_FileNotFound", ex);
                }
                catch (UnauthorizedAccessException ex)
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
                    logger.Log("Rename_UnauthorizedAccessException", ex);
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
                    logger.Log("Rename_Exception", ex);
                }
                
            }
            else
            {
                try
                {

                    FileAttributes attr = File.GetAttributes(path);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
                        {
                            SqlCommand command = new SqlCommand("proc_InsertDirectory", conn);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@DriveName", driveName));
                            command.Parameters.Add(new SqlParameter("@DirectoryName", fileName));

                            command.Parameters.Add(new SqlParameter("@size",
                                                         Convert.ToInt64(DircetorySize(di))));

                            command.Parameters.Add(new SqlParameter("@Path",
                                                         path));
                            command.Parameters.Add(new SqlParameter("@HashCode",
                                                                     hashCode));
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    else
                    {
                        FileInfo di = new FileInfo(path);

                        using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=SystemSpace;Integrated Security=SSPI;User=sa;Password =abc@123"))
                        {
                            SqlCommand command = new SqlCommand("proc_Insertfiles", conn);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@DriveName", driveName));
                            command.Parameters.Add(new SqlParameter("@FileName", fileName));

                            command.Parameters.Add(new SqlParameter("@size",
                                                         di.Length));

                            command.Parameters.Add(new SqlParameter("@Path",
                                                         path));
                            command.Parameters.Add(new SqlParameter("@HashCode",
                                                                     hashCode));
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                catch (FileNotFoundException ex)
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
                    logger.Log("Create_FileNotFoundException", ex);
                }
                catch (UnauthorizedAccessException ex)
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
                    logger.Log("Create_UnauthorizedAccessException", ex);
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
                    logger.Log("Create_Exception", ex);
                }
            }
        }

        public static string CreateMd5ForFolder(string path)
        {
            MD5 md5 = MD5.Create();

            md5.ComputeHash(Encoding.UTF8.GetBytes(path));

            return String.Concat(path.Substring(path.Length - (path.Length > 6 ? 6 : 3), (path.Length > 6 ? 6 : 3)), BitConverter.ToString(md5.Hash).Replace("-", ""));
        }

        public static long DircetorySize(DirectoryInfo DI)
        {
            long size = 0;
            try
            {
                DirectoryInfo[] SubDirectories = DI.GetDirectories();
                if (SubDirectories.Length != 0)
                {
                    foreach (var sb in SubDirectories)
                    {
                        size = size + DircetorySize(sb);
                    }
                }
                FileInfo[] files = DI.GetFiles();
                if (files.Length != 0)
                {
                    foreach (var file in files)
                    {
                        size = size + file.Length;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Log("DirectorySize_UnauthorizedAccessException", ex);
            }
            return size;
        }
    }
}
