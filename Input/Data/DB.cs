using System.Configuration;
using System.Data.Common;

namespace FT.Input.Data
{
    public class DB
    {
        static readonly DbConnection connection = null;        

        private DB() {}

        static DB()
        {
            string dataprovider = ConfigurationManager.AppSettings["dataprovider"];
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataprovider);
            connection = factory.CreateConnection();                        
        }

        public static DbConnection Connection
        {
            get
            {
                return connection;
            }
        }

        public static DbCommand NewCommand()
        {
            return Connection.CreateCommand();
        }
    }
}
