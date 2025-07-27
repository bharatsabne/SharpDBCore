using SharpDBLight.Core;
using SharpDBLight.Factories;
using Xunit;
using Assert = Xunit.Assert;

namespace SharpDBLight.Test
{
    public class DatabaseManagerTests
    {
        private const string TestConnectionString = "Data Source=C:\\Test\\test.db";
        public DatabaseManagerTests()
        {
            DbConnectionLightFactory.SetConnectionString(TestConnectionString);
            var db = DatabaseLightManager.Instance;
        }
        [Fact]
        public async Task ExecuteScalarAsync_ShouldReturn_1_WhenQuerySelects1()
        {
            // Arrange
            var db = DatabaseLightManager.Instance;
            string query = "SELECT 1";

            // Act
            var result = await db.ExecuteScalarAsync(query);

            // Assert
            Assert.Equal(1, Convert.ToInt32(result));
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_ShouldWork_WithTempTable()
        {
            var db = DatabaseLightManager.Instance;
            db.BeginTransaction();
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
