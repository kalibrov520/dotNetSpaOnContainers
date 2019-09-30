using System;
using System.IO;
using System.Net.Sockets;
using Polly;
using Polly.CircuitBreaker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusRabbitMQ
{
    public class RabbitMqPersistentConnection : IRabbitMqPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly int _retryCount;
        private IConnection _connection;
        private bool _disposed;
        private object _sync_root = new object();

        public RabbitMqPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _retryCount = retryCount;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException e)
            {

            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMq connections!");
            }

            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            lock (_sync_root)
            {
                var policy = Policy.Handle<SocketException>().Or<BrokenCircuitException>().WaitAndRetry(_retryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallBackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    return true;
                }

                return false;
            }
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            TryConnect();
        }

        private void OnCallBackException(object sender, CallbackExceptionEventArgs reason)
        {
            if (_disposed) return;

            TryConnect();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs reason)
        {
            if (_disposed) return;

            TryConnect();
        }
    }
}