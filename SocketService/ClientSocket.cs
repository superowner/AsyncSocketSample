﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketService
{
    public class ClientSocket : IClientSocket
    {
        private Socket _innerSocket;
        private ISocketService _socketService;

        public ClientSocket()
        {
            _innerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketService = new DefaultSocketService();
        }

        public IClientSocket Connect(string address, int port)
        {
            _innerSocket.Connect(new IPEndPoint(IPAddress.Parse(address), port));
            return this;
        }
        public IClientSocket Start(Action<string> replyMessageReceivedCallback)
        {
            Task.Factory.StartNew(() =>
            {
                _socketService.ReceiveMessage(_innerSocket, (reply) =>
                {
                    try
                    {
                        replyMessageReceivedCallback(reply);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
                new ManualResetEvent(false).WaitOne();
            });
            return this;
        }
        public IClientSocket Shutdown()
        {
            _innerSocket.Shutdown(SocketShutdown.Both);
            _innerSocket.Close();
            return this;
        }
        public IClientSocket SendMessage(string messageContent, Action<string> messageSentCallback)
        {
            _socketService.SendMessage(_innerSocket, messageContent, messageSentCallback);
            return this;
        }
    }
}
