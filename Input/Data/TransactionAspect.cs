using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using PostSharp.Laos;

namespace FT.Input.Data
{
    [Serializable]
    public class TransactionAspect : OnMethodBoundaryAspect
    {
        private string datasource = null;
        private static object transaction = null;

        public override void OnEntry(MethodExecutionEventArgs eventArgs)
        {                        
            if (String.IsNullOrEmpty(DB.Connection.ConnectionString))
            {         
                if (datasource == null)
                {
                    datasource = ConfigurationManager.AppSettings["datasource"];
                }

                DB.Connection.ConnectionString = datasource;
            }

            if (DB.Connection.State != ConnectionState.Open)
            {
                DB.Connection.Open();
            }
            
            transaction = DB.Connection.BeginTransaction();
        }

        public override void OnSuccess(MethodExecutionEventArgs eventArgs)
        {
            if (transaction != null)
            {
                ((SQLiteTransaction) transaction).Commit();
            }

            if (DB.Connection.State == ConnectionState.Open)
            {
                DB.Connection.Close();
            }
        }

        public override void OnException(MethodExecutionEventArgs eventArgs)
        {
            if (transaction != null)
            {
                try
                {
                    ((SQLiteTransaction) transaction).Rollback();
                } 
                catch (Exception) {}
            }

            if (DB.Connection.State == ConnectionState.Open)
            {
                DB.Connection.Close();
            }
        }
    }
}