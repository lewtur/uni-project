using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using FYP.Data;
using Xunit;

namespace FYP.IntegrationTests
{
    public class DatabaseConnectionTests
    {
        [Fact]
        public void ShouldBeAbleToConnectToTheDatabaseAndReadAndWriteValuesFromIt()
        {
            // given
            using (var repo = new IntegrationTestRepository())
            {
                // when
                repo.Add(1);
                var result = repo.Read();

                // then
                Assert.Contains(1, result);
            }            
        }
    }

    internal class IntegrationTestRepository : ConnectionBase, IDisposable
    {
        private const string TableName = "IntegrationTests";

        public IntegrationTestRepository()
        {
            Create();
        }

        private void Create()
        {
            using (var conn = GetConnection())
            {
                conn.Execute($"CREATE TABLE {TableName} (Id INTEGER)");
            }
        }

        private void Delete()
        {
            using (var conn = GetConnection())
            {
                conn.Execute($"DROP TABLE {TableName}");
            }
        }

        public void Add(int val)
        {
            using (var conn = GetConnection())
            {
                conn.Execute($"INSERT INTO {TableName} (Id) VALUES ({val})");
            }
        }

        public IEnumerable<int> Read()
        {
            using (var conn = GetConnection())
            {
                return conn.Query<int>($"SELECT * FROM {TableName}");
            }
        }

        public void Dispose()
        {
            Delete();
        }
    }
}
