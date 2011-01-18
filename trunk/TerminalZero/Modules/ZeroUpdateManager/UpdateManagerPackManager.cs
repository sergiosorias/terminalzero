using System;
using System.Data.SqlClient;
using ZeroUpdateManager.Properties;

namespace ZeroUpdateManager
{
    public class UpdateManagerPackManager : ZeroCommonClasses.PackClasses.PackManager
    {
        public UpdateManagerPackManager(string packDir)
            : base(packDir)
        {
            Importing += UpdateManagerPackManager_Importing;
        }

        private void UpdateManagerPackManager_Importing(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
            e.Pack.IsUpgrade = true;
            SqlTransaction tran = null;
            SqlConnection conn = null;
            string[] filesToProcess = System.IO.Directory.GetFiles(e.WorkingDirectory, "*" + Resources.ScripFileExtention);
            if (filesToProcess.Length > 0)
            {
                string lastScript = "";
                e.Pack.Result = "SQL";
                try
                {
                    conn = new SqlConnection(ZeroCommonClasses.Context.ContextBuilder.GetConnectionForCurrentEnvironment().ConnectionString);
                    conn.Open();
                    tran = conn.BeginTransaction();
                    System.Data.IDbCommand command = conn.CreateCommand();
                    command.Transaction = tran;
                    foreach (var file in filesToProcess)
                    {
                        ZeroUpdateManager.Database.DeployFile deployFile = Database.DeployFile.LoadFrom(file);
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
