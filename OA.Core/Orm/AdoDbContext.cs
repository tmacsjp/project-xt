using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OA.Core
{
    public class AdoDbContext : DbContext
    {
        public static AdoDbContext CreateSqlServer(string connstring)
        {
            return new AdoDbContext(new DbContextOptionsBuilder().UseSqlServer(connstring).Options);
        }
        public static AdoDbContext CreateMySql(string connstring)
        {
            return new AdoDbContext(new DbContextOptionsBuilder().UseMySql(connstring, ServerVersion.AutoDetect(connstring)).Options);
        }
        DbExecuteInterceptProvider interceptProvider;

        protected DbTransaction dbTransaction;
        protected DbConnection dbConnection;
        const int cmdTimeoutDefault = 30;
        int cmdTimeoutSeconds = 0;
        public bool PauseLog { get; set; }
        TaskFactory taskFactory = Task.Factory;

        bool isAttachMessage;
        /// <summary>
        /// 输出信息
        /// </summary>
        public Action<string> OnMessage;
        public AdoDbContext()
        {
            interceptProvider = new DbExecuteInterceptProvider(this);
            interceptProvider.OnDbCommand = OnCommanding;
        }
        public AdoDbContext(DbContextOptions options)
            : base(options)
        {
            interceptProvider = new DbExecuteInterceptProvider(this);
            interceptProvider.OnDbCommand = OnCommanding;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(interceptProvider);
        }

        public bool IsMySql()
        {
            return Database.IsMySql();
        }
        public bool IsSqlServer()
        {
            return Database.IsSqlServer();
        }

        public int GetDbType()
        {
            return Database.IsMySql() ? 1 : 2;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                if (mutableEntityType.ClrType == null)
                    continue;
                List<PropertyInfo> keyProps = new List<PropertyInfo>();


                //列名前标（f_等)
                MyTableAttribute? outsoftsTable = mutableEntityType.ClrType.GetCustomAttribute<MyTableAttribute>();
                if (outsoftsTable != null)
                {
                    if (!string.IsNullOrWhiteSpace(outsoftsTable.TableName))
                        mutableEntityType.SetTableName(outsoftsTable.TableName);
                    foreach (var a in mutableEntityType.GetDeclaredProperties())
                    {
                        if (a.FieldInfo == null) continue;
                        var delc = a.PropertyInfo.GetCustomAttribute<ColumnAttribute>();
                        if (delc == null || string.IsNullOrWhiteSpace(delc.Name))
                        {
                            string columName = a.GetColumnBaseName();
                            if (outsoftsTable.SplitWord)
                            {
                                columName = SplitWords(columName);
                            }
                            if (outsoftsTable.LowerCase)
                                columName = columName.ToLower();
                            a.SetColumnName(outsoftsTable.Prefix + columName);
                        }
                    }
                }

                foreach (var fieldProp in mutableEntityType.ClrType.GetProperties())
                {
                    //默认值处理
                    var fieldDefaultValue = fieldProp.GetCustomAttribute<FieldDefaultValueAttribute>();
                    var keyAttr = fieldProp.GetCustomAttribute<KeyAttribute>();
                    if (fieldDefaultValue != null && fieldDefaultValue.DefaultValue != null)
                    {
                        mutableEntityType.FindProperty(fieldProp).SetDefaultValue(fieldDefaultValue.DefaultValue);
                    }
                    if (keyAttr != null)
                    {
                        keyProps.Add(fieldProp);
                    }
                }
                //多列主键
                if (keyProps.Count > 1)
                {
                    var props = mutableEntityType.FindProperties(new ReadOnlyCollection<string>(keyProps.Select(x => x.Name).ToList()));
                    mutableEntityType.SetPrimaryKey(props);
                }
            }
        }

        Regex regexOfSplit = new Regex("([A-Z]+)");
        private string SplitWords(string words)
        {
            var nword = regexOfSplit.Replace(words, "_$1");
            nword = nword.Trim('_');
            return nword;
        }

        public void SetCommandTimeout(int seconds)
        {
            if (seconds > 0)
            {
                Database.SetCommandTimeout(TimeSpan.FromSeconds(seconds));
                cmdTimeoutSeconds = seconds;
            }
            else
            {
                Database.SetCommandTimeout(TimeSpan.FromSeconds(seconds));
                cmdTimeoutSeconds = cmdTimeoutDefault;
            }
        }

        public int GetCommandTimeout()
        {
            return Database.GetCommandTimeout() ?? 0;
        }

        public void ResetCommandTimeout()
        {
            SetCommandTimeout(0);
        }

        private bool OnCommanding(IDbCommand command, AdoDbContext dbContext)
        {
            if (string.IsNullOrEmpty(command.CommandText))
                return false;
            if (cmdTimeoutSeconds > 0)
                command.CommandTimeout = cmdTimeoutSeconds;

            if (OnMessage != null && isAttachMessage == false)
            {
                PrepareAdoConnection();
                if (dbConnection is SqlConnection _sqlconn)
                {
                    _sqlconn.InfoMessage += Sqlconn_InfoMessage;
                }
                isAttachMessage = true;
            }
            var r = OnCommandExecuting(command);
            //#if DEBUG
            //            Console.WriteLine(command.CommandText);
            //#endif
            return r;
        }
        protected virtual bool OnCommandExecuting(IDbCommand command)
        {
            return false;
        }

        private void Sqlconn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            if (OnMessage == null)
                return;
            if (!string.IsNullOrEmpty(e.Message))
                OnMessage(e.Message);
        }

        protected bool ReplaceCommandIntereptor(IDbCommand cmd, Regex reg, string replace)
        {
            var ntext = reg.Replace(cmd.CommandText, replace);
            if (ntext == cmd.CommandText)
                return false;
            cmd.CommandText = ntext;
            return true;
        }


        private DbConnection PrepareAdoConnection()
        {
            if (dbConnection != null)
                return dbConnection;
            dbConnection = Database.GetDbConnection();
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
            return dbConnection;
        }

        #region Ado事务方式1
        public IDbTransaction AdoBeginTransaction()
        {
            PrepareAdoConnection();
            dbTransaction = dbConnection.BeginTransaction();
            Database.UseTransaction(dbTransaction);
            return dbTransaction;
        }

        public bool AdoIsTransaction()
        {
            return dbTransaction != null;
        }


        public void AdoCommit()
        {
            if (dbTransaction == null)
                throw new Exception("请先开启事务！");
            dbTransaction.Commit();
            dbTransaction = null;
            Database.UseTransaction(null);
        }

        public void AdoRollback()
        {
            if (dbTransaction == null)
                throw new Exception("请先开启事务！");
            dbTransaction.Rollback();
            dbTransaction = null;
            Database.UseTransaction(null);
        }
        #endregion



        /// <summary>
        /// 检查Sql并过滤
        /// </summary>
        /// <param name="sqlstring"></param>
        /// <returns></returns>
        public string PrepareSql(string sqlstring)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText = sqlstring;
            interceptProvider.CheckAndFix(cmd, this);
            return cmd.CommandText;
        }

        protected OrmExecuteContext BuildContext()
        {
            return new OrmExecuteContext(dbConnection) { };
        }

        public DateTime AdoGetDateTime()
        {
            if (Database.IsMySql())
            {
                return (DateTime)AdoExecuteScalar("select now()", null);
            }
            else
            {
                return (DateTime)AdoExecuteScalar("select getdate()", null);
            }
        }

        public async Task<DateTime> AdoGetDateTimeAsync()
        {
            if (Database.IsMySql())
            {
                return (DateTime)await AdoExecuteScalarAsync("select now()", null);
            }
            else
            {
                return (DateTime)await AdoExecuteScalarAsync("select getdate()", null);
            }
        }

        public long AdoGetIdentityLong()
        {
            if (Database.IsMySql())
            {
                var obj = AdoExecuteScalar("select LAST_INSERT_ID();", null);
                if (obj == null)
                    return 0;
                return obj.NullToStr().StrToLong();
            }
            else
            {
                var obj = AdoExecuteScalar("select @@Identity;", null);
                if (obj == null)
                    return 0;
                return obj.NullToStr().StrToLong();
            }
        }

        public async Task<long> AdoGetIdentityLongAsync()
        {
            if (Database.IsMySql())
            {
                var obj = await AdoExecuteScalarAsync("select LAST_INSERT_ID();", null);
                if (obj == null)
                    return 0;
                return obj.NullToStr().StrToLong();
            }
            else
            {
                var obj = await AdoExecuteScalarAsync("select @@Identity;", null);
                if (obj == null)
                    return 0;
                return obj.NullToStr().StrToLong();
            }
        }

        public int AdoGetIdentity()
        {
            var obj = AdoGetIdentityLong();
            return (int)obj;
        }
        public async Task<int> AdoGetIdentityAsync()
        {
            var obj = await AdoGetIdentityLongAsync();
            return (int)obj;
        }
        public object AdoExecuteScalar(string sql, object paras)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();

            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.Transaction = dbTransaction;
                cmd.CommandText = sql;
                FillParameters(cmd, paras);
                interceptProvider.BeforeExecute(dbcontext);
                var obj = cmd.ExecuteScalar();
                ProcessReturnPara(cmd, paras);
                interceptProvider.EndExecute(dbcontext);
                return obj;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
        }

        public async Task<object> AdoExecuteScalarAsync(string sql, object paras)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();

            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.Transaction = dbTransaction;
                cmd.CommandText = sql;
                FillParameters(cmd, paras);
                interceptProvider.BeforeExecute(dbcontext);
                var obj = await cmd.ExecuteScalarAsync();
                ProcessReturnPara(cmd, paras);
                interceptProvider.EndExecute(dbcontext);
                return obj;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
        }

        public int AdoExecuteSql(string sql, object parameters)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();

            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);
                var reader = cmd.ExecuteNonQuery();
                ProcessReturnPara(cmd, parameters);
                interceptProvider.EndExecute(dbcontext);
                return reader;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
        }

        public async Task<int> AdoExecuteSqlAsync(string sql, object parameters)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();

            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);
                var reader = await cmd.ExecuteNonQueryAsync();
                ProcessReturnPara(cmd, parameters);
                interceptProvider.EndExecute(dbcontext);
                return reader;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
        }


        public DataTable AdoExecuteQuery(string sql, object parameters)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);

                DataTable dt = new DataTable();
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                interceptProvider.EndExecute(dbcontext);
                return dt;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task<DataTable> AdoExecuteQueryAsync(string sql, object parameters)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);

                DataTable dt = new DataTable();
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    await taskFactory.StartNew(() => da.Fill(dt));
                }
                interceptProvider.EndExecute(dbcontext);
                return dt;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public DataSet AdoExecuteQuerySet(string sql, object parameters)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);

                DataSet dt = new DataSet();
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                interceptProvider.EndExecute(dbcontext);
                return dt;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task<DataSet> AdoExecuteQuerySetAsync(string sql, object parameters)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);

                DataSet dt = new DataSet();
                using (var da = DbProviderFactories.GetFactory(dbConnection).CreateDataAdapter())
                {
                    da.SelectCommand = cmd;
                    await taskFactory.StartNew(() => da.Fill(dt));
                    da.Fill(dt);
                }
                interceptProvider.EndExecute(dbcontext);
                return dt;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
        }


        public DataTable AdoExecuteQuery2(string sql, object parameters, int rowcount = 0)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);
                DataTable dt = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    if (rowcount > 0)
                    {
                        DataColumn col;
                        DataRow row;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            col = new DataColumn();
                            col.ColumnName = reader.GetName(i);
                            col.DataType = reader.GetFieldType(i);
                            dt.Columns.Add(col);
                        }

                        int j = 0;
                        while (reader.Read() && j < rowcount)
                        {
                            row = dt.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }
                            dt.Rows.Add(row);
                            j++;
                        }
                    }
                    else
                    {
                        dt.Load(reader);
                    }
                }
                interceptProvider.EndExecute(dbcontext);
                return dt;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public async Task<DataTable> AdoExecuteQuery2Async(string sql, object parameters, int rowcount = 0)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();
            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                cmd.CommandText = sql;
                cmd.Transaction = dbTransaction;
                FillParameters(cmd, parameters);
                interceptProvider.BeforeExecute(dbcontext);
                DataTable dt = new DataTable();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (rowcount > 0)
                    {
                        DataColumn col;
                        DataRow row;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            col = new DataColumn();
                            col.ColumnName = reader.GetName(i);
                            col.DataType = reader.GetFieldType(i);
                            dt.Columns.Add(col);
                        }

                        int j = 0;
                        while (await reader.ReadAsync() && j < rowcount)
                        {
                            row = dt.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }
                            dt.Rows.Add(row);
                            j++;
                        }
                    }
                    else
                    {
                        await taskFactory.StartNew(() => dt.Load(reader));
                    }
                }
                interceptProvider.EndExecute(dbcontext);
                return dt;
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public List<T> AdoExecuteQuery<T>(string sql, object parameters, Func<DataRow, T> fillFunc)
        {
            var table = AdoExecuteQuery(sql, parameters);
            List<T> ts = new List<T>();
            foreach (DataRow dr in table.Rows)
            {
                ts.Add(fillFunc.Invoke(dr));
            }
            return ts;
        }

        public async Task<List<T>> AdoExecuteQueryAsync<T>(string sql, object parameters, Func<DataRow, T> fillFunc)
        {
            var table = await AdoExecuteQueryAsync(sql, parameters);
            List<T> ts = new List<T>();
            foreach (DataRow dr in table.Rows)
            {
                ts.Add(fillFunc.Invoke(dr));
            }
            return ts;
        }

        public DataTable AdoExecutePage(string getColsSql, string orderBySql, string tableFromSql,
   List<OrmParameter> parameters, int pageNo, int pageSize,
    out int pageCount, out int totalResults)
        {
            if (parameters == null) parameters = new List<OrmParameter>();
            tableFromSql = (tableFromSql ?? "").TrimStart();
            if (tableFromSql.Length > 4 && tableFromSql.Substring(0, 4).ToLower() == "from")
            {
                tableFromSql = tableFromSql.Substring(4);
            }
            pageSize = Math.Max(1, pageSize);
            pageNo = Math.Max(1, pageNo);
            int startIndex = (pageNo - 1) * pageSize + 1;
            int endIndex = startIndex + pageSize - 1;
            string countsql = string.Empty;
            string itemsql = string.Empty;
            if (IsMySql())
            {
                countsql = "  select count(1) as a FROM " + tableFromSql + " ";
                itemsql = "select " + getColsSql + " from " + tableFromSql + " order by " + orderBySql + "  " +
                  $" limit   {startIndex - 1}, {pageSize};";
            }
            else
            {
                parameters.Add(new OrmParameter("@_startIndex", startIndex));
                parameters.Add(new OrmParameter("@_endIndex", endIndex));
                countsql = "select count(1) as f_sl FROM " + tableFromSql + "  ";
                itemsql = "select * from" +
                  " (select ROW_NUMBER() OVER (order by " + orderBySql + ") as _rownum," +
                  " " + getColsSql + " from " + tableFromSql + ") N " +
                  " where _rownum between @_startIndex and @_endIndex;";
            }
            DataTable _dt = AdoExecuteQuery(countsql, parameters);
            totalResults = _dt.Rows[0]["f_sl"].ObjToInt();
            pageCount = (totalResults + pageSize - 1) / pageSize;
            _dt = AdoExecuteQuery(itemsql, parameters);
            return _dt;
        }



        public List<T> AdoExecutePage<T>(string getColsSql, string orderBySql, string tableFromSql,
   List<OrmParameter> parameters, int pageNo, int pageSize,
    out int pageCount, out int totalResults, Func<DataRow, T> fillFunc)
        {
            var table = AdoExecutePage(getColsSql, orderBySql, tableFromSql, parameters, pageNo, pageSize, out pageCount, out totalResults);
            List<T> ts = new List<T>();
            foreach (DataRow dr in table.Rows)
            {
                ts.Add(fillFunc.Invoke(dr));
            }
            return ts;
        }

        public Params AdoParams()
        {
            return new Params();
        }

        private void FillParameters(DbCommand command, object paras)
        {
            if (paras == null)
                return;
            DbParameterCollection paraCollection = command.Parameters;
            if (paras is IEnumerable<OrmParameter>)
            {
                foreach (var a in (IEnumerable<OrmParameter>)paras)
                {
                    var pa = command.CreateParameter();
                    ParamTrans(pa, a);
                    paraCollection.Add(pa);
                }
            }
            else if (paras is IEnumerable<DbParameter>)
            {
                foreach (var a in (List<DbParameter>)paras)
                {
                    var pa = command.CreateParameter();
                    pa.ParameterName = a.ParameterName;
                    pa.Direction = a.Direction;
                    pa.DbType = a.DbType;
                    pa.Value = a.Value;
                    paraCollection.Add(pa);
                }
            }
            else
            {
                foreach (var prop in paras.GetType().GetProperties())
                {
                    var pa = command.CreateParameter();
                    pa.ParameterName = prop.Name;
                    pa.Value = prop.GetValue(paras);
                    if (pa.Value is string)
                        pa.DbType = DbType.String;
                    else if (pa.Value is int)
                        pa.DbType = DbType.Int32;
                    else if (pa.Value is long)
                        pa.DbType = DbType.Int64;
                    else if (pa.Value is double)
                        pa.DbType = DbType.Double;
                    else if (pa.Value is decimal)
                        pa.DbType = DbType.Decimal;
                    else if (pa.Value is DateTime)
                        pa.DbType = DbType.DateTime;
                    paraCollection.Add(pa);
                }
            }
        }


        private void ParamTrans(DbParameter dbpara, OrmParameter para)
        {
            switch (para.ParType)
            {
                case OrmParamType.Int16:
                    dbpara.DbType = DbType.Int16;
                    break;
                case OrmParamType.Int32:
                    dbpara.DbType = DbType.Int32;
                    break;
                case OrmParamType.Int64:
                    dbpara.DbType = DbType.Int64;
                    break;
                case OrmParamType.Single:
                    dbpara.DbType = DbType.Single;
                    break;
                case OrmParamType.Double:
                    dbpara.DbType = DbType.Double;
                    break;
                case OrmParamType.Decimal:
                    dbpara.DbType = DbType.Decimal;
                    break;
                case OrmParamType.Char:
                    dbpara.DbType = DbType.AnsiStringFixedLength;
                    break;
                case OrmParamType.VarChar:
                    dbpara.DbType = DbType.AnsiString;
                    break;
                case OrmParamType.NVarchar:
                    dbpara.DbType = DbType.String;
                    break;
                case OrmParamType.NChar:
                    dbpara.DbType = DbType.StringFixedLength;
                    break;
                case OrmParamType.Binary:
                    dbpara.DbType = DbType.Binary;
                    break;
                case OrmParamType.DateTime:
                    dbpara.DbType = DbType.DateTime;
                    break;
                case OrmParamType.Default:
                default:
                    break;
            }
            dbpara.ParameterName = para.Name;
            dbpara.Value = para.Value;
            dbpara.Size = para.Size;
            dbpara.Direction = (ParameterDirection)para.Direction;
            if (para.Precision > 0)
            {
                dbpara.Precision = (byte)para.Precision;
                dbpara.Scale = (byte)para.Scale;
            }
        }


        public bool AdoTableExist(string name)
        {
            if (Database.IsMySql())
            {
                string sql = @"SELECT count(1) FROM information_schema.TABLES 
                    where   `TABLE_SCHEMA`=@dbname 
                    AND `TABLE_NAME`=@tbname;";
                var obj = AdoExecuteScalar(sql, new
                {
                    dbname = Database.GetDbConnection().Database,
                    tbname = name
                });
                return obj.ObjToInt() > 0;
            }
            else
            {
                string sql = @" if   object_id(@tbname,'u') is not null  
                        select 1 as 'cname'
                    else 
                    select 0 as 'cname'";
                var obj = AdoExecuteScalar(sql, new
                {
                    tbname = name
                });
                return obj.ObjToInt() > 0;
            }
        }

        public bool AdoProcedureExist(string name)
        {
            if (Database.IsMySql())
            {
                string sql = @"SELECT count(1) FROM information_schema.ROUTINES 
                    where   `ROUTINE_SCHEMA`=@dbname 
                    AND `ROUTINE_TYPE`='PROCEDURE'
                    AND `ROUTINE_NAME`=@proname;";
                var obj = AdoExecuteScalar(sql, new
                {
                    dbname = Database.GetDbConnection().Database,
                    proname = name
                });
                return obj.ObjToInt() > 0;
            }
            else
            {
                string sql = @" if   object_id(@pname,'P') is not null  
                        select 1 as 'cname'
                    else 
                    select 0 as 'cname'";
                var obj = AdoExecuteScalar(sql, new
                {
                    pname = name
                });
                return obj.ObjToInt() > 0;
            }
        }


        public bool AdoColumnExist(string tableName, string columnName)
        {
            if (Database.IsMySql())
            {
                string sql = @"SELECT count(1) FROM information_schema.COLUMNS 
                            where   `TABLE_SCHEMA`=@dbname 
                            AND `TABLE_NAME`=@tbname
                            AND `COLUMN_NAME`=@colname;";
                var obj = AdoExecuteScalar(sql, new
                {
                    dbname = Database.GetDbConnection().Database,
                    tbname = tableName,
                    colname = columnName
                });
                return obj.ObjToInt() > 0;
            }
            else
            {
                string sql = @"select count(1) from sys.columns 
                    where [name] =@colname 
                    and [object_id]=OBJECT_ID(@tbname,'U')";
                var obj = AdoExecuteScalar(sql, new
                {
                    tbname = tableName,
                    colname = columnName
                });
                return obj.ObjToInt() > 0;
            }

        }

        public int AdoExecuteProcedure(string procedureName, object paras)
        {
            PrepareAdoConnection();
            var cmd = dbConnection.CreateCommand();

            var dbcontext = BuildContext();
            dbcontext.DbCommand = cmd;
            try
            {
                FillParameters(cmd, paras);
                DbParameter returnpara = null;
                foreach (DbParameter a in cmd.Parameters)
                {
                    if (a.Direction == ParameterDirection.ReturnValue)
                    {
                        returnpara = a;
                        break;
                    }
                }
                if (returnpara == null)
                {
                    returnpara = cmd.CreateParameter();
                    returnpara.ParameterName = "__core_return";
                    returnpara.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnpara);
                }

                cmd.Transaction = dbTransaction;
                cmd.CommandText = procedureName;
                cmd.CommandType = CommandType.StoredProcedure;

                interceptProvider.BeforeExecute(dbcontext);
                var r = cmd.ExecuteNonQuery();
                ProcessReturnPara(cmd, paras);
                interceptProvider.EndExecute(dbcontext);
                return returnpara.Value.ObjToInt();
            }
            catch (Exception ex)
            {
                dbcontext.Error = ex;
                interceptProvider.ExecuteError(dbcontext);
                throw;
            }
        }

        private void ProcessReturnPara(DbCommand cmd, object paras)
        {
            if (paras is IEnumerable<OrmParameter> _dbparas)
            {
                foreach (DbParameter _p in cmd.Parameters)
                {
                    if (_p.Direction == ParameterDirection.InputOutput
                       || _p.Direction == ParameterDirection.Output
                       || _p.Direction == ParameterDirection.ReturnValue)
                    {
                        var to_p = _dbparas.FirstOrDefault(x => x.Name == _p.ParameterName);
                        if (to_p != null)
                        {
                            to_p.Value = _p.Value;
                        }
                    }
                }
            }
        }

        public TableDefi AdoGetTableStructure(string tableName)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");

            //table
            string sqltable = $"select name,object_id from sys.tables where [name]='{tableName}';";
            var tb = AdoExecuteQuery(sqltable, new { name = tableName });
            if (tb.Rows.Count != 1)
                throw new Exception("表不存在！");
            var tbdefi = new TableDefi()
            {
                Name = tb.Rows[0]["name"].ToString(),
                ObjectId = tb.Rows[0]["object_id"].ObjToInt(),
            };

            //columns
            string sqlcolumn = @" 
            select 
            col.[column_id],col.[name],col.[object_id],col.[is_identity],col.[system_type_id],col.[max_length],
            col.[precision],col.[scale],col.[is_nullable],
            ty.name as typename
            from sys.all_columns col
            left join sys.types ty on col.system_type_id=ty.system_type_id and col.user_type_id=ty.user_type_id  
            where  [object_id]=@id;";
            tbdefi.Columns = AdoExecuteQuery(sqlcolumn, new { id = tbdefi.ObjectId }, dr =>
            {
                return new ColumnDefi()
                {
                    ColumnId = dr["column_id"].ObjToInt(),
                    IsIdentity = dr["is_identity"].ObjToInt() > 0,
                    MaxLength = dr["max_length"].ObjToInt(),
                    Nullable = dr["is_nullable"].ObjToInt() > 0,
                    Name = dr["name"].ToString(),
                    ObjectId = dr["object_id"].ObjToInt(),
                    Precision = dr["precision"].ObjToInt(),
                    Scale = dr["scale"].ObjToInt(),
                    Type = dr["typename"].ToString(),
                    TypeId = dr["system_type_id"].ObjToInt(),
                };
            });

            //index
            string sqlindex = "select [object_id],[name],[index_id],[type],[type_desc],is_unique,is_primary_key,is_unique_constraint" +
                " from sys.indexes where [object_id]=@id and [type]>0";
            tbdefi.Indexs = AdoExecuteQuery(sqlindex, new { id = tbdefi.ObjectId }, dr =>
            {
                return new IndexDefi()
                {
                    IsUniqueConstraint = dr["is_unique_constraint"].ObjToInt() > 0,
                    IsPrimaryKey = dr["is_primary_key"].ObjToInt() > 0,
                    Name = dr["name"].ToString(),
                    ObjectId = dr["object_id"].ObjToInt(),
                    IndexId = dr["index_id"].ObjToInt(),
                    IsUnique = dr["is_unique"].ObjToInt() > 0,
                    Type = dr["type"].ObjToInt(),
                    TypeDesc = dr["type_desc"].ObjToStr(),
                };
            });
            string sqlindexcols = "select  object_id,index_id,column_id,key_ordinal,[is_descending_key],[is_included_column],[index_column_id] " +
                " from sys.index_columns where [object_id]=@id";
            var index_cols = AdoExecuteQuery(sqlindexcols, new { id = tbdefi.ObjectId }, dr =>
            {
                return new IndexColumnDefi()
                {
                    ColumnId = dr["column_id"].ObjToInt(),
                    IsDescending = dr["is_descending_key"].ObjToInt() > 0,
                    IsIncluded = dr["is_included_column"].ObjToInt() > 0,
                    ObjectId = dr["object_id"].ObjToInt(),
                    IndexId = dr["index_id"].ObjToInt(),
                    Ordinal = dr["key_ordinal"].ObjToInt(),
                };
            });

            foreach (var ix in tbdefi.Indexs)
            {
                ix.Columns = index_cols.Where(x => x.IndexId == ix.IndexId).OrderBy(x => x.Ordinal).ToList();
            }

            //defaultconstraint

            string sqldefaultconstraint = @" 
    select [name],[object_id],parent_object_id,parent_column_id,definition from sys.default_constraints
  where  [parent_object_id]=@id;";
            tbdefi.DefaultConstraints = AdoExecuteQuery(sqldefaultconstraint,
                new { id = tbdefi.ObjectId },
                dr =>
                {
                    return new DefaultConstraintDefi()
                    {
                        ColumnId = dr["parent_column_id"].ObjToInt(),
                        Name = dr["name"].ToString(),
                        ObjectId = dr["object_id"].ObjToInt(),
                        ParentObjectId = dr["parent_object_id"].ObjToInt(),
                        Definition = dr["definition"].NullToStr(),
                    };
                });
            string identitysql1 = "select object_id,name,column_id,seed_value,increment_value,last_value from sys.identity_columns where object_id=@objid";
            DataTable tbidentity = AdoExecuteQuery(identitysql1, new { objid = tbdefi.ObjectId });
            if (tbidentity.Rows.Count > 0)
            {
                foreach (DataRow dr in tbidentity.Rows)
                {
                    string cn = dr["name"].ToString();
                    var r = tbdefi.Columns.FirstOrDefault(x => x.Name == cn);
                    if (r == null)
                    {
                        throw new Exception(cn + "在表中不存在");
                    }
                    var iden = new IdentityDefi()
                    {
                        column = r,
                        send = Convert.ToInt64(dr["seed_value"]),
                        crement = Convert.ToInt64(dr["increment_value"]),
                        lastvalue = dr["last_value"] == null || dr["last_value"].ToString() == "" ? Convert.ToInt64(dr["seed_value"]) - 1 : Convert.ToInt64(dr["last_value"])
                    };
                    r.IsIdentity = true;
                    r.Identity = iden;
                    tbdefi.Identity = iden;
                    break;
                }
            }

            LinkTableStructure(tbdefi);
            return tbdefi;
        }



        private bool ExistObjectSqlServer(string objtype, string objname)
        {
            string sql = "select count(1) from sys.all_objects where type=@objtype and [name]=@objname ";
            var count = Convert.ToInt32(AdoExecuteScalar(sql, new { objtype, objname }));
            return count > 0;
        }

        public bool AdoExistProcedure(string name)
        {
            if (IsSqlServer())
                return ExistObjectSqlServer("P", name);
            throw new NotSupportedException("Not support current dbtype!");
        }
        public bool AdoExistFunction(string name)
        {
            if (IsSqlServer())
                return ExistObjectSqlServer("FN", name);
            throw new NotSupportedException("Not support current dbtype!");
        }

        public string AdoGetProcedureSql(string proname)
        {
            if (IsSqlServer())
                return GetDefiSqlSqlServer("P", proname);
            throw new NotSupportedException("Not support current dbtype!");
        }


        public string AdoGetFunctionSql(string proname)
        {
            if (IsSqlServer())
                return GetDefiSqlSqlServer("FN", proname);
            throw new NotSupportedException("Not support current dbtype!");
        }

        public string GetDefiSqlSqlServer(string type, string proname)
        {
            string sql = "select [definition] from sys.sql_modules where object_id=OBJECT_ID(@objname,@tp)";
            var tbid = AdoExecuteScalar(sql, new { objname = proname, tp = type });
            if (tbid == null)
                return string.Empty;
            return tbid.ToString();
        }


        private void LinkTableStructure(TableDefi tableDefi)
        {
            //default  constraint
            foreach (var a in tableDefi.DefaultConstraints)
            {
                a.Table = tableDefi;
                var col = tableDefi.Columns.First(x => x.ColumnId == a.ColumnId);
                a.Column = col;
                col.DefaualtConstraint = a;
            }

            //index
            foreach (var a in tableDefi.Indexs)
            {
                a.Table = tableDefi;
                foreach (var icol in a.Columns)
                {
                    var col = tableDefi.Columns.First(x => x.ColumnId == icol.ColumnId);
                    icol.Column = col;
                    icol.Index = a;
                }
            }

            //col
            foreach (var a in tableDefi.Columns)
            {
                a.Table = tableDefi;
                a.Indexs = tableDefi.Indexs.Where(x => x.Columns.Any(y => y.ColumnId == a.ColumnId)).ToList();
                a.IsPrimaryKey = a.Indexs.Any(x => x.IsPrimaryKey);
            }
        }

        public int AdoSuperDropColumn(string tableName, string columnName)
        {
            return AdoSuperDropColumn(AdoGetTableStructure(tableName), columnName);
        }
        public int AdoSuperDropColumn(TableDefi tableDefi, string columnName)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");
            var col = tableDefi.Columns.FirstOrDefault(x => x.Name.ToLower() == columnName.ToLower());
            if (col == null)
                throw new Exception("列不存在！");
            if (col.DefaualtConstraint != null)
            {
                AdoDropConstraint(tableDefi.Name, col.DefaualtConstraint.Name);
                tableDefi.DefaultConstraints.Remove(col.DefaualtConstraint);
                col.DefaualtConstraint = null;
            }
            //删除索引 
            foreach (var a in col.Indexs)
            {
                AdoDropIndex(a.Table.Name, a.Name);
            }

            //删除列
            int r = AdoDropColumn(col.Table.Name, col.Name);
            tableDefi.Columns.Remove(col);
            foreach (var a in col.Indexs)
            {
                a.Columns.RemoveAll(x => x.ColumnId == col.ColumnId);
            }
            //重新添加索引
            foreach (var a in col.Indexs)
            {
                if (a.Columns.Count == 0)
                    tableDefi.Indexs.Remove(a);

                var sqlcreateindex = $"CREATE {(a.IsUnique ? " UNIQUE " : "")} {a.TypeDesc} INDEX {a.Name} ON {a.Table.Name} " +
                     $"({string.Join(",", a.Columns.Where(x => !x.IsIncluded).Select(x => x.Column.Name + (x.IsDescending ? " DESC" : "")))}) " +
                     $"{(a.Columns.Any(x => x.IsIncluded) ? $" INCLUDE ({string.Join(",", a.Columns.Where(x => x.IsIncluded).Select(x => x.Column.Name))})" : "")}";
                AdoExecuteSql(sqlcreateindex, null);
            }
            return r;
        }

        public int AdoDropColumn(string tableName, string columnName)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");
            return AdoExecuteSql($"alter table [{tableName}] drop column {columnName}", null);
        }

        public int AdoDropConstraint(string tableName, string constraintName)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");
            return AdoExecuteSql($"alter table [{tableName}] drop constraint {constraintName}", null);
        }
        public int AdoDropIndex(string tableName, string indexName)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");
            return AdoExecuteSql($"drop index {indexName} on [{tableName}];", null);
        }


        public async Task BuckCopyAsync(DataTable tabledata, int batch = 1000, int timeoutseconds = 30)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");
            PrepareAdoConnection();
            try
            {
                var conn = dbConnection as SqlConnection;
                var tran = dbTransaction as SqlTransaction;
                SqlBulkCopy sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                sqlBulk.BatchSize = batch;
                sqlBulk.BulkCopyTimeout = timeoutseconds;
                sqlBulk.DestinationTableName = tabledata.TableName;
                for (int ci = 0; ci < tabledata.Columns.Count; ci++)
                {
                    sqlBulk.ColumnMappings.Add(tabledata.Columns[ci].ColumnName, tabledata.Columns[ci].ColumnName);
                }
                await sqlBulk.WriteToServerAsync(tabledata);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void BuckCopy(DataTable tabledata, int batch = 1000, int timeoutseconds = 30)
        {
            if (!IsSqlServer())
                throw new NotSupportedException("不支持该数据库！");
            PrepareAdoConnection();
            try
            {
                var conn = dbConnection as SqlConnection;
                var tran = dbTransaction as SqlTransaction;
                SqlBulkCopy sqlBulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                sqlBulk.BatchSize = batch;
                sqlBulk.BulkCopyTimeout = timeoutseconds;
                sqlBulk.DestinationTableName = tabledata.TableName;
                for (int ci = 0; ci < tabledata.Columns.Count; ci++)
                {
                    sqlBulk.ColumnMappings.Add(tabledata.Columns[ci].ColumnName, tabledata.Columns[ci].ColumnName);
                }
                sqlBulk.WriteToServer(tabledata);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
