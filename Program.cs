using System;
using System.Data.Common;
using FT.Input.Data;
using FT.Input.Files;
using log4net;
using log4net.Config;

namespace FT
{          
    //sqlite .net: http://sourceforge.net/project/showfiles.php?group_id=132486&package_id=145568
    class Program
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static Program()
        {
            XmlConfigurator.Configure();
        }

        public static void Print(string sql, DbDataReader reader)
        {
            log.Info("Results of [" + sql + "]");
            string results = "\n";
            if (reader.HasRows)
            {
                for (int j = 0; j < reader.FieldCount; j++)
                {
                    results += reader.GetName(j);
                    if (j < reader.FieldCount - 1)
                    {
                        results += "|";
                    }
                }
                results += "\n";
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        results += reader.GetValue(i);
                        if (i < reader.FieldCount - 1)
                        {
                            results += "|";
                        }
                    }
                    results += "\n";
                }
                results = results.Substring(0, results.Length - 1);
                log.Info(results);
            }
        }

        static void Main()
        {
            Schema.Validate();
            Transaction.ExecuteQuery("select * from properties", Print);
            new FTReader().Read(@"C:\FT20090201.txt");
            Transaction.ExecuteQuery("select * from players", Print);
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
