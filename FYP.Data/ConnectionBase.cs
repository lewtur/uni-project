using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FYP.Data
{
    public abstract class ConnectionBase
    {        
        private const string LocalConnectionString = "***REMOVED***";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(LocalConnectionString);
        }
    }
}
