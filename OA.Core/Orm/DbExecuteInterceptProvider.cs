using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OA.Core
{
    public delegate bool DbCommandHandler(IDbCommand command, AdoDbContext dbContext);
    public class DbExecuteInterceptProvider : DbCommandInterceptor, IAdoCommandInterceptor
    {
        public DbCommandHandler OnDbCommand;
        AdoDbContext _dbContext;
        public DbExecuteInterceptProvider(AdoDbContext adoDbContext)
        {
            _dbContext = adoDbContext;
        }
        #region before execute
        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            EFCoreBeforeExecute(result, eventData);
            return base.CommandCreated(eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            EFCoreBeforeExecute(command, eventData);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            EFCoreBeforeExecute(command, eventData);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            EFCoreBeforeExecute(command, eventData);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            EFCoreBeforeExecute(command, eventData);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            EFCoreBeforeExecute(command, eventData);
            return base.ScalarExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            EFCoreBeforeExecute(command, eventData);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
        #endregion

        #region endexecute
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            EFCoreEndExecute(command, eventData);
            return base.ReaderExecuted(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            EFCoreEndExecute(command, eventData);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            EFCoreEndExecute(command, eventData);
            return base.NonQueryExecuted(command, eventData, result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            EFCoreEndExecute(command, eventData);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
        {
            EFCoreEndExecute(command, eventData);
            return base.ScalarExecuted(command, eventData, result);
        }

        public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
        {
            EFCoreEndExecute(command, eventData);
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }
        public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            EFCoreExecuteError(command, eventData);
            base.CommandFailed(command, eventData);
        }


        #endregion

        #region efcore 拦截
        private void EFCoreBeforeExecute(DbCommand command, CommandEventData eventData)
        {
            CheckAndFix(command, eventData.Context as AdoDbContext);
            EFCoreSetSqlLog(command, eventData, true);
        }

        private void EFCoreEndExecute(DbCommand command, CommandEventData eventData)
        {
            EFCoreSetSqlLog(command, eventData, false);
        }

        private void EFCoreExecuteError(DbCommand command, CommandErrorEventData eventData)
        {
            EFCoreSetSqlLog(command, eventData, false);
        }

        private void EFCoreSetSqlLog(DbCommand command, CommandEventData eventData, bool isbeginlog)
        {
            var context = eventData.Context as AdoDbContext;
            //var logger = context.Logger;
            var db = context.Database.GetDbConnection().Database;
            var sql = command.CommandText;
            StringBuilder sbpara = new StringBuilder();
            foreach (DbParameter a in command.Parameters)
            {
                sbpara.Append($"{a.ParameterName} = {a.Value}; ");
            }
            var usems = isbeginlog ? -1 : (DateTime.UtcNow - eventData.StartTime.UtcDateTime).TotalMilliseconds;
            //logger.SqlLog(db, sql, sbpara.ToString(), usems);
            //if (eventData is CommandErrorEventData errordata)
            //{
            //    logger.Error("SqlError", $"{errordata.Exception.Message}", $"sql={sql};param={sbpara.ToString()}");
            //}
        }

        #endregion


        #region ado 拦截接口实现
        public void BeforeExecute(OrmExecuteContext context)
        {
            CheckAndFix(context.DbCommand, _dbContext);
            AdoSetSqlLog(context, true);
        }

        public void EndExecute(OrmExecuteContext context)
        {
            AdoSetSqlLog(context, false);
        }

        public void ExecuteError(OrmExecuteContext context)
        {
            AdoSetSqlLog(context, false);
        }

        private void AdoSetSqlLog(OrmExecuteContext exeContext, bool isbeginlog)
        {
            if (_dbContext.PauseLog)
                return;
            //var logger = _dbContext.Logger ?? Core.Logging.LoggerFactory.Create();
            var db = exeContext.Conn.Database;
            var sql = exeContext.DbCommand.CommandText;
            StringBuilder sbpara = new StringBuilder();
            foreach (DbParameter a in exeContext.DbCommand.Parameters)
            {
                sbpara.Append($"{a.ParameterName} = {a.Value}; ");
            }
            var usems = isbeginlog ? -1 : (DateTime.UtcNow - exeContext.UtcStartTime).TotalMilliseconds;
            //logger.SqlLog(db, sql, sbpara.ToString(), usems);
            //if (exeContext.Error != null)
            //{
            //    logger.Error("SqlError", $"{exeContext.Error.Message}", $"sql={sql};param={sbpara.ToString()}");
            //}
        }

        #endregion

        #region 公用 Sql拦截器
        public bool CheckAndFix(IDbCommand command, AdoDbContext dbContext)
        {
            if (string.IsNullOrWhiteSpace(command.CommandText))
                return false;
            if (OnDbCommand == null)
                return false;
            return OnDbCommand.Invoke(command, dbContext);
        }

        #endregion
    }

    public interface IAdoCommandInterceptor
    {
        public void BeforeExecute(OrmExecuteContext context);
        public void EndExecute(OrmExecuteContext context);
        public void ExecuteError(OrmExecuteContext context);
    }
}
