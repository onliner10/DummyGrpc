﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Meter
{
    public class ThreadOrchestrator<TResult>
    {
        public int N { get; }
        public int Concurrent { get; }
        private int _remaining;
        private readonly Func<TResult> _action;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        public ThreadOrchestrator(int n, int concurrent, Func<TResult> action)
        {
            N = n;
            Concurrent = concurrent;
            _remaining = n;
            _action = action;
        }

        public TResult[] Start()
        {
            var results = new ConcurrentQueue<TResult>();

            var resetEvent = new ManualResetEventSlim(false);
            var threads = new List<Thread>();

            for (int i = 0; i < Concurrent; i++)
            {
                var consumer = new Thread(() =>
                {
                    resetEvent.Wait();
                    while (_remaining > 0)
                    {
                        results.Enqueue(_action());
                        Interlocked.Decrement(ref _remaining);
                    }
                });
                threads.Add(consumer);
                consumer.Start();
            }

            resetEvent.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("Done");
            return results.ToArray();
        }
    }
}