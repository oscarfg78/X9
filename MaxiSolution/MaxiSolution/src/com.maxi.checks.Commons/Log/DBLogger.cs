using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.Services.SouthSide.Log
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
        }
    }


    public class DBLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _connectionString;

        public DBLogger(string categoryName, string connectionString)
        {
            _categoryName = categoryName;
            _connectionString = connectionString;
        }


        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("Services.st_SaveServiceLog", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        var process = Process.GetCurrentProcess();

                        var eventIdParam = command.CreateParameter();
                        eventIdParam.Value = eventId.Id;
                        eventIdParam.DbType = DbType.Int32;
                        eventIdParam.ParameterName = "@EventId";
                        command.Parameters.Add(eventIdParam);

                        var priorityParam = command.CreateParameter();
                        priorityParam.Value = 1;
                        priorityParam.DbType = DbType.Int32;
                        priorityParam.ParameterName = "@Priority"; // TODO
                        command.Parameters.Add(priorityParam);

                        var severityParam = command.CreateParameter();
                        severityParam.Value = logLevel.ToString();
                        severityParam.DbType = DbType.String;
                        severityParam.ParameterName = "@Severity";
                        command.Parameters.Add(severityParam);

                        var titleParam = command.CreateParameter();
                        titleParam.Value = logLevel.ToString();
                        titleParam.DbType = DbType.String;
                        titleParam.ParameterName = "@Title";
                        command.Parameters.Add(titleParam);

                        var datetimeParam = command.CreateParameter();
                        datetimeParam.Value = DateTime.Now.ToLocalTime();
                        datetimeParam.DbType = DbType.DateTime;
                        datetimeParam.ParameterName = "@LogDate";
                        command.Parameters.Add(datetimeParam);

                        var machineNameParam = command.CreateParameter();
                        machineNameParam.Value = Environment.MachineName;
                        machineNameParam.DbType = DbType.String;
                        machineNameParam.ParameterName = "@MachineName";
                        command.Parameters.Add(machineNameParam);

                        var appDomainNameParam = command.CreateParameter();
                        appDomainNameParam.Value = AppDomain.CurrentDomain.FriendlyName;
                        appDomainNameParam.DbType = DbType.String;
                        appDomainNameParam.ParameterName = "@AppDomainName";
                        command.Parameters.Add(appDomainNameParam);

                        var processIdParam = command.CreateParameter();
                        processIdParam.Value = process.Id;
                        processIdParam.DbType = DbType.String;
                        processIdParam.ParameterName = "@ProcessId";
                        command.Parameters.Add(processIdParam);

                        var processNameParam = command.CreateParameter();
                        processNameParam.Value = process.ProcessName;
                        processNameParam.DbType = DbType.String;
                        processNameParam.ParameterName = "@ProcessName";
                        command.Parameters.Add(processNameParam);

                        var managedThreadNameParam = command.CreateParameter();
                        managedThreadNameParam.Value = Thread.CurrentThread.Name ?? "Unnamed Thread";
                        managedThreadNameParam.DbType = DbType.String;
                        managedThreadNameParam.ParameterName = "@ManagedThreadName";
                        command.Parameters.Add(managedThreadNameParam);

                        var win32ThreadIdParam = command.CreateParameter();
                        win32ThreadIdParam.Value = Thread.CurrentThread.ManagedThreadId;
                        win32ThreadIdParam.DbType = DbType.String;
                        win32ThreadIdParam.ParameterName = "@WinThreadId";
                        command.Parameters.Add(win32ThreadIdParam);

                        var messageParam = command.CreateParameter();
                        messageParam.Value = formatter(state, exception);
                        messageParam.DbType = DbType.String;
                        messageParam.ParameterName = "@Message";
                        command.Parameters.Add(messageParam);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }

    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly string _connectionString;

        public DbLoggerProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DBLogger(categoryName, _connectionString);
        }

        public void Dispose() { }
    }
}
