using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroUpdateManager.Database;
using ZeroUpdateManager.Properties;

namespace ZeroUpdateManager
{
    public class UpdateManagerPackManager : PackManager
    {
        public UpdateManagerPackManager(ITerminal terminal)
            : base(terminal)
        {
            Importing += UpdateManagerPackManager_Importing;
        }

        private void UpdateManagerPackManager_Importing(object sender, PackEventArgs e)
        {
            e.Pack.IsUpgrade = true;
            
            string[] filesToProcess = Directory.GetFiles(e.WorkingDirectory, "*" + Resources.ScripFileExtention);
            if (filesToProcess.Length > 0)
            {
                e.Pack.Result = "SQL";
                ProcessScripts(filesToProcess,e.Pack.Code,e.PackInfo.TerminalCode);
            }

            if (e.PackInfo != null && !ContextInfo.IsOnServer)
            {
                string dir = Path.Combine(e.WorkingDirectory, "App");
                if(Directory.Exists(dir))
                {
                    ProcessUpgrade(dir,e.Pack.Code);
                }
            }
        }

        private void ProcessUpgrade(string dir, int packCode)
        {
            try
            {
                Directory.Move(dir, Path.Combine(ContextInfo.Directories.UpgradeFolder,packCode.ToString()));
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
                string lastScript = "";
                try
                {
                    conn = new SqlConnection(ContextInfo.GetConnectionForCurrentEnvironment().ConnectionString);
                    conn.Open();
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
