using System.Data.Common;

namespace FT.Input.Data
{
    public delegate void PostProcessReader(string sql, DbDataReader reader);

    public class Transaction
    {
        public static void ExecuteNonQuery(string sql)
        {
            using (DbCommand command = DB.NewCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        public static void ExecuteQuery(string sql, PostProcessReader postProcess)
        {            
            using (DbCommand command = DB.NewCommand())
            {
                command.CommandText = sql;
                postProcess(sql, command.ExecuteReader());
            }
        }

        public static object ExecuteScalar(string sql)
        {
            object returnValue;
            using (DbCommand command = DB.NewCommand())
            {
                command.CommandText = sql;
                returnValue = command.ExecuteScalar();
            }
            return returnValue;
        }
    }
}