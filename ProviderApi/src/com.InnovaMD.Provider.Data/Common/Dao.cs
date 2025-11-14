using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static Dapper.SqlMapper;

namespace com.InnovaMD.Provider.Data.Common
{
    public class Dao : IDisposable
    {
        private readonly IDbConnection connection;

        public Dao(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int Execute(string query, object parameters, IDbTransaction transaction = null)
        {
            return connection.Execute(query, parameters, transaction);
        }

        public long Insert<TEntity>(TEntity entity) where TEntity : class
        {
            return connection.Insert(entity);
        }

        public bool Update<TEntity>(TEntity entity) where TEntity : class
        {
            return connection.Update(entity);
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class
        {
            return connection.Delete(entity);
        }

        public TEntity Get<TEntity>(int id) where TEntity : class
        {
            return connection.Get<TEntity>(id);
        }

        public TResult QuerySingle<TResult>(string query, object parameters = null)
        {
            return connection.Query<TResult>(query, parameters).SingleOrDefault();
        }

        public IEnumerable<TResult> Query<TResult>(string query, object parameters = null) where TResult : class
        {
            return connection.Query<TResult>(query, parameters);
        }

        public TResult ExecuteScalar<TResult>(string query, object parameters = null, IDbTransaction transaction = null)
        {
            return connection.ExecuteScalar<TResult>(query, parameters, transaction);
        }

        public GridReader FindMultiple(IList<string> queries, object parameters = null, int? commandTimeout = 60)
        {
            StringBuilder strBuilder = new StringBuilder();

            foreach (var q in queries)
            {
                strBuilder.Append(q.Replace(';', ' ')).Append(";").AppendLine();
            }

            var query = strBuilder.ToString();
            var results = connection.QueryMultiple(query, parameters, commandTimeout: commandTimeout);
            return results;
        }

        public GridReader FindMultiple<TEntity>(string query, object parameters = null, CommandType? commandType = null, int? commandTimeout = 60)
        {
            return connection.QueryMultiple(query, parameters, commandType: commandType, commandTimeout: commandTimeout);
        }

        public IEnumerable<TEntity> Find<TEntity>(string query, object parameters = null, CommandType? commandType = null, int? commandTimeout = 60)
        {
            var results = connection.Query<TEntity>(query, parameters, commandTimeout: commandTimeout, commandType: commandType);
            return results;
        }

        public IEnumerable<TEntity> Find<TFirst, TSecond, TEntity>(string query, Func<TFirst, TSecond, TEntity> map, object parameters = null, string splitOn = null, CommandType? commandType = null)
        {
            var results = connection.Query<TFirst, TSecond, TEntity>(query, map, parameters, splitOn: splitOn, commandType: commandType);
            return results;
        }

        public IEnumerable<TEntity> Find<TFirst, TSecond, TThird, TEntity>(string query, Func<TFirst, TSecond, TThird, TEntity> map, object parameters = null, string splitOn = null, CommandType? commandType = null, bool appendRecompile = false)
        {
            var results = connection.Query<TFirst, TSecond, TThird, TEntity>(query, map, parameters, splitOn: splitOn, commandType: commandType);
            return results;
        }

        public IEnumerable<TEntity> Find<TFirst, TSecond, TThird, TFourth, TEntity>(string query, Func<TFirst, TSecond, TThird, TFourth, TEntity> map, object parameters = null, string splitOn = null, CommandType? commandType = null)
        {
            var results = connection.Query<TFirst, TSecond, TThird, TFourth, TEntity>(query, map, parameters, splitOn: splitOn, commandType: commandType);
            return results;
        }

        public IEnumerable<TEntity> Find<TFirst, TSecond, TThird, TFourth, TFifth, TEntity>(string query, Func<TFirst, TSecond, TThird, TFourth, TFifth, TEntity> map, object parameters = null, string splitOn = null, CommandType? commandType = null)
        {
            var results = connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TEntity>(query, map, parameters, splitOn: splitOn, commandType: commandType);
            return results;
        }

        public IEnumerable<TEntity> Find<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TEntity>(string cmdTxt, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TEntity> map, object parameters = null, string splitOn = null)
        {
            var results = connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TEntity>(cmdTxt, map, parameters, splitOn: splitOn);
            return results;
        }

        public IEnumerable<TEntity> Find<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeven, TEntity>(string cmdText, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeven, TEntity> map, object parameters = null, string splitOn = null)
        {
            var results = connection.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeven, TEntity>(cmdText, map, parameters, splitOn: splitOn);
            return results;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    //if (connection != null)
                    //{
                    //    connection.Close();
                    //    connection.Dispose();
                    //}
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
