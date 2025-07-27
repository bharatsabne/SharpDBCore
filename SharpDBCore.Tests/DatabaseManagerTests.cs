using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using SharpDBCore.Core;
using SharpDBCore.Factories;
using SharpDBCore.Interfaces;
using SharpDBCore.Loggers;
using System.Data.Common;

namespace SharpDBCore.Tests
{
    public static  class DatabaseManagerExtensions
    {
        public static  void Test(this DatabaseManager dbManager)
        {
            System.Diagnostics.Debug.WriteLine($"Testing extensation method");
        }
    }
    public class MyLogger : NullLogger
    {
        public override void LogError(string message, Exception? ex = null)
        {
            System.Diagnostics.Debug.WriteLine($"MyLogger.LogError called with message: {message}, Exception: {ex}");
        }

        public override void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"MyLogger.LogInfo called with message: {message}");
        }
    }
    public class DatabaseManagerTests
    {
        private const string TestConnectionString = "Server=DatabaseServer;Database=TestDb;Integrated Security=True;TrustServerCertificate=True;";
        public DatabaseManagerTests()
        {
            //Test MS SQL Server
            DbConnectionFactory.SetConnectionString(TestConnectionString);
            var db = DatabaseManager.Instance;
            db.SetLogger(new MyLogger());
            db.Logger.LogError("This is error");
        }
        [Fact]
        public async Task ExecuteScalarAsync_ShouldReturn_1_WhenQuerySelects1()
        {
            // Arrange
            var db = DatabaseManager.Instance;
            string query = "SELECT 1";

            // Act
            var result = await db.ExecuteScalarAsync(query);

            // Assert
            Assert.Equal(1, Convert.ToInt32(result));
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_ShouldWork_WithTempTable()
        {
            var db = DatabaseManager.Instance;
            db.SetLogger(new NullLogger());

            db.BeginTransaction();
            //extensation method test
            db.Test();
            try
            {
                await db.ExecuteNonQueryAsync("CREATE TABLE #Temp(Id INT)");
                await db.ExecuteNonQueryAsync("INSERT INTO #Temp(Id) VALUES (99)");

                var result = await db.ExecuteScalarAsync("SELECT COUNT(*) FROM #Temp");

                Assert.Equal(1, Convert.ToInt32(result));

                var id = await db.ExecuteScalarAsync("SELECT Id FROM #Temp");
                Assert.Equal(99, Convert.ToInt32(id));
                db.CommitTransaction();
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }
        }
    }
}
