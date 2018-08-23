using System;
using System.Net;
using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.Transport;

namespace Ascon.Pilot.Server.Api
{
    public class HttpPilotClient : IImplementationFactory, IDisposable
    {
        private readonly string _url;
        private readonly IWebProxy _proxy;
        private readonly TransportClient _client;
        private readonly CallbackReceiverAdapter _transportCallback;
        protected readonly Marshaller _marshaller;
        private readonly Unmarshaller _unmarshaller;
        protected IServerUpdateCallback _updateCallback;
        private IServerCallback _serverCallback;
        private IServerAdminCallback _adminCallback;
        private IMessageCallback _messageCallback;
        private bool _isBroken;
        private IConnectionLostListener _connectionLostListener;

        public HttpPilotClient(string url, IWebProxy proxy = null)
        {
            _url = url;
            _proxy = proxy;
            _client = new TransportClient();
            _marshaller = new Marshaller(new CallServiceAdapter(_client));
            _unmarshaller = new Unmarshaller(this);
            _transportCallback = new CallbackReceiverAdapter(_unmarshaller, CallbackError);
        }

        public void SetConnectionLostListener(IConnectionLostListener connectionLostListener)
        {
            _connectionLostListener = connectionLostListener;
        }

        private void CallbackError()
        {
            if (_connectionLostListener != null)
                _connectionLostListener.ConnectionLost(
                    new TransportException("Client connection is disconnected by server."));
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void BreakConnection()
        {
            _isBroken = true;
            _client.Disconnect();
        }

        public void RestoreConnection()
        {
            _isBroken = false;
        }

        private void CheckBroken()
        {
            if (_isBroken)
                throw new TransportException("Connection is not available");
        }

        public virtual void Connect()
        {
            ConnectBase();
#if !DEBUG 
            CheckClientVersion();
#endif
            _client.OpenCallback(_transportCallback);
        }

        protected void ConnectBase()
        {
            CheckBroken();
            _client.SetProxy(_proxy);
            _client.Connect(_url);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public IAuthenticationApi GetAuthenticationApi()
        {
            return _marshaller.Get<IAuthenticationApi>();
        }

        public IServerApi GetServerApi(IServerCallback callback)
        {
            _serverCallback = callback;
            return _marshaller.Get<IServerApi>();
        }

        public IServerAdminApi GetServerAdminApi(IServerAdminCallback callback)
        {
            _adminCallback = callback;
            return _marshaller.Get<IServerAdminApi>();
        }

        protected IServerUpdateApi GetServerUpdateApi(IServerUpdateCallback callback)
        {
            _updateCallback = callback;
            return _marshaller.Get<IServerUpdateApi>();
        }

        public IImportApi GetImportApi()
        {
            return _marshaller.Get<IImportApi>();
        }

        public IFileArchiveApi GetFileArchiveApi()
        {
            return _marshaller.Get<IFileArchiveApi>();
        }

        public IMessagesApi GetMessagesApi(IMessageCallback callback)
        {
            _messageCallback = callback;
            return _marshaller.Get<IMessagesApi>();
        }

        private class CallServiceAdapter : ICallService
        {
            private readonly TransportClient _client;

            public CallServiceAdapter(TransportClient client)
            {
                _client = client;
            }

            public byte[] Call(byte[] data)
            {
                return _client.Call(data);
            }
        }

        public object GetImplementation(string interfaceName)
        {
            if (interfaceName == "IServerCallback")
                return _serverCallback;
            if (interfaceName == "IServerAdminCallback")
                return _adminCallback;
            if (interfaceName == "IServerUpdateCallback")
                return _updateCallback;
            if (interfaceName == "IMessageCallback")
                return _messageCallback;
            throw new NotImplementedException(interfaceName);
        }

        internal void CheckClientVersion()
        {
            //var updateApi = GetServerUpdateApi(new NullServerUpdateCallback());
            //var serverVersion = new Version(updateApi.GetServerVersion());
            //if (serverVersion.CompareTo(_clientVersion) != 0)
            //    throw new VersionMatchException("Server version and client do not match");
        }

        private class CallbackReceiverAdapter : ICallbackReceiver
        {
            private readonly Unmarshaller _unmarshaller;
            private readonly Action _action;

            public CallbackReceiverAdapter(Unmarshaller unmarshaller, Action action)
            {
                _unmarshaller = unmarshaller;
                _action = action;
            }

            public void Receive(byte[] data)
            {
                _unmarshaller.Call(data);
            }

            public void Error()
            {
                _action.Invoke();
            }
        }
    }
}