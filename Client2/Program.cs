﻿using SocketService;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Client2
{
    class Program
    {
        private static ManualResetEvent _signal = new ManualResetEvent(false);
        private const int TotalCount = 10;
        private static int _currentReceived = 0;
        private const string SampleMessage = "hello2";

        static void Main(String[] args)
        {
            var client = new ClientSocket().Connect("127.0.0.1", 11000).Start((reply) =>
            {
                _currentReceived++;
                Console.WriteLine("received: {0}", reply);
                if (_currentReceived == TotalCount)
                {
                    _signal.Set();
                }
            });

            var watch = Stopwatch.StartNew();

            for (var index = 0; index < TotalCount; index++)
            {
                client.SendMessage(SampleMessage, (messageContent) =>
                {
                    Console.WriteLine("sent:{0}", messageContent);
                });
            }

            _signal.WaitOne();
            Console.WriteLine(watch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
