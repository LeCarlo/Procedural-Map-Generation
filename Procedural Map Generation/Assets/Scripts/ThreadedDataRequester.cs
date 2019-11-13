using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ThreadedDataRequester : MonoBehaviour
{
    // sets the thread data to be instance
    static ThreadedDataRequester instance;

    // creates a new dataQueue
    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();


    private void Awake()
    {
        // on awake, sets the instance to be the object of threaded data requester
        instance = FindObjectOfType<ThreadedDataRequester>();
    }

    // Starts a new thread when requesting data
    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart threadStart = delegate { instance.DataThread(generateData, callback); };

        new Thread(threadStart).Start();
    }

    // enques the data from the thread into the thread info queue
    void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();
        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }
    
    // deqees the data and calls back the info
    private void Update()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

}
