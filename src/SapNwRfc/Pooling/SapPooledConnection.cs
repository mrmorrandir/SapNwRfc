using System.Diagnostics.CodeAnalysis;
using System.Threading;
using SapNwRfc.Exceptions;

namespace SapNwRfc.Pooling
{
    /// <summary>
    /// Represents a pooled connection.
    /// </summary>
    public sealed class SapPooledConnection : ISapPooledConnection
    {
        private readonly ISapConnectionPool _pool;
        private ISapConnection _connection = null;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapPooledConnection"/> class.
        /// </summary>
        /// <param name="pool">The connection pool.</param>
        public SapPooledConnection(ISapConnectionPool pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SapPooledConnection"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~SapPooledConnection()
        {
            Dispose();
        }

        /// <summary>
        /// Returns the underlying <see cref="SapConnection"/> to the connection pool and make it available for reuse.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_connection != null)
            {
                _pool.ReturnConnection(_connection);
                _connection = null;
            }

            _disposed = true;
        }

        /// <inheritdoc cref="ISapPooledConnection"/>
        public void InvokeFunction(string name, CancellationToken cancellationToken = default)
        {
            _connection = _connection ?? _pool.GetConnection(cancellationToken);

            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke();
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke();
            }
        }

        /// <inheritdoc cref="ISapPooledConnection"/>
        public void InvokeFunction(string name, object input, CancellationToken cancellationToken = default)
        {
            _connection = _connection ?? _pool.GetConnection(cancellationToken);

            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke(input);
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    function.Invoke(input);
            }
        }

        /// <inheritdoc cref="ISapPooledConnection"/>
        public TOutput InvokeFunction<TOutput>(string name, CancellationToken cancellationToken = default)
        {
            _connection = _connection ?? _pool.GetConnection(cancellationToken);

            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>();
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>();
            }
        }

        /// <inheritdoc cref="ISapPooledConnection"/>
        public TOutput InvokeFunction<TOutput>(string name, object input, CancellationToken cancellationToken = default)
        {
            _connection = _connection ?? _pool.GetConnection(cancellationToken);

            try
            {
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>(input);
            }
            catch (SapCommunicationFailedException)
            {
                // Let the pool collect the dead connection
                _pool.ForgetConnection(_connection);

                // Retry invocation with new connection from the pool
                _connection = _pool.GetConnection(cancellationToken);
                using (ISapFunction function = _connection.CreateFunction(name))
                    return function.Invoke<TOutput>(input);
            }
        }
    }
}
