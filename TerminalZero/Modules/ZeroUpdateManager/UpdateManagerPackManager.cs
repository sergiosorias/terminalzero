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
            SqlTransaction tran = null;
            SqlConnection conn = null;
            string[] filesToProcess = Directory.GetFiles(e.WorkingDirectory, "*" + Resources.ScripFileExtention);
            if (filesToProcess.Length > 0)
            {
                string lastScript = "";
                e.Pack.Result = "SQL";
                try
                {
                    conn = new SqlConnection(ContextBuilder.GetConnectionForCurrentEnvironment().ConnectionString);
                    conn.Open();
                    tran = conn.BeginTransaction();
                    IDbCommand command = conn.CreateCommand();
                    command.Transaction = tran;
                    foreach (var file in filesToProcess)
                    {
                        DeployFile deployFile = DeployFile.LoadFrom(file,e.Pack.Code,e.PackInfo.TerminalCode);
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
                    throw new Exception("Error executing update pack import - LAST SCRIPT: "+ lastScript,ex);
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
