using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroUpdateManager.Database;
using ZeroUpdateManager.Properties;

namespace ZeroUpdateManager
{
    public class UpdateManagerPackManager : PackManager
    {
        private readonly StringBuilder outMessage = new StringBuilder();
        
        public UpdateManagerPackManager(ITerminal terminal)
            : base(terminal)
        {
            
        }

        protected override void ImportProcess(PackProcessingEventArgs args)
        {
            base.ImportProcess(args);
            args.Pack.IsUpgrade = true;

            string[] filesToProcess = Directory.GetFiles(args.PackInfo.WorkingDirectory, "*" + Resources.ScripFileExtention);
            if (filesToProcess.Length > 0)
            {
                ProcessScripts(filesToProcess, args.Pack.Code, args.PackInfo.TerminalCode);
                args.Pack.Result = "SQL\n" + outMessage;
            }

            if (args.PackInfo != null && !ConfigurationContext.IsOnServer)
            {
                string dir = Path.Combine(args.PackInfo.WorkingDirectory, "App");
                if (Directory.Exists(dir))
                {
                    ProcessUpgrade(dir, args.Pack.Code);
                }
            }
        }

        private void ProcessUpgrade(string dir, int packCode)
        {
            try
            {
                Directory.Move(dir, Path.Combine(ConfigurationContext.Directories.UpgradeFolder,packCode.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing update pack import - MOVING FILES: " +  ex);    
            }
        }

        private void ProcessScripts(string[] filesToProcess, int packCode, int terminalCode)
        {
            SqlTransaction tran = null;
            SqlConnection conn = null;
            if (filesToProcess.Length > 0)
            {
                int errorCount = 0;
                string lastScript = "";
                try
                {
                    conn = new SqlConnection(ConfigurationContext.GetConnectionForCurrentEnvironment().ConnectionString);
                    conn.Open();
                    conn.FireInfoMessageEventOnUserErrors = true;
                    conn.InfoMessage += (sender, e) =>
                    {
                        for (int i = 0; i < e.Errors.Count; i++)
                        {
                            SqlError error = e.Errors[i];
                            outMessage.AppendLine(error.Message);
                            if(error.Class > 10) 
                            {            
                                // treat this as a genuine error        
                                errorCount++;
                            } 
                        }        
                    };
                    tran = conn.BeginTransaction();
                    IDbCommand command = conn.CreateCommand();
                    command.Transaction = tran;
                    foreach (var file in filesToProcess)
                    {
                        DeployFile deployFile = DeployFile.LoadFrom(file, packCode, terminalCode);
                        foreach (var item in deployFile.GetStatements(conn.Database))
                        {
                            lastScript = item;
                            command.CommandText = item;
                            command.ExecuteNonQuery();
                            if (errorCount!=0)
                                throw new Exception(outMessage.ToString());
                            
                        }
                        
                    }

                    tran.Commit();

                }
                catch (Exception ex)
                {
                    if (tran != null)
                    {
                        tran.Rollback();
                        tran.Dispose();
                    }
                    throw new Exception("Error executing update pack import - LAST SCRIPT: " + lastScript, ex);
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Dispose();
                    }
                }
            }
        }

        
    }
}
