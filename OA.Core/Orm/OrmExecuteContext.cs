using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OA.Core
{
    public class OrmExecuteContext
    {
        public IDbConnection Conn { get; private set; }
        //public AdoDbContext AdoContext { get; private set; }
        public DateTime UtcStartTime { get; set; } = DateTime.UtcNow;
        public IDbCommand DbCommand { get; set; }
        public Exception Error { get; set; }
        public OrmExecuteContext(IDbConnection conn)
        {
            Conn = conn;
        }
        public OrmExecuteContext(AdoDbContext adoContext)
        {
            Conn = adoContext.Database.GetDbConnection();
        }
    }
}
