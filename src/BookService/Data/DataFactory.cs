using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BookService.Data
{
    public class DataFactory
    {
        public string SchemaVersion { get; set; }

        public DataFactory()
        {
            using (SqlConnection connection = new SqlConnection(Startup.ConnectionString))
            {
                SqlCommand command = new SqlCommand("proc_GetSchemaVersion", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SchemaVersion = command.ExecuteScalar().ToString();
            }
        }

        public IBookData GetBookDataModel(IBookData dataModel = null)
        {
            if (dataModel != null)
                return dataModel;

            return new BookDataSql(SchemaVersion);
        }

    }

    public static class SchemaVersions
    {
        public const string v1_0_0 = "1.0.0";
        public const string v1_0_1 = "1.0.1";
    }
}
