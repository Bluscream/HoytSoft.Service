/*
 HoytSoft Self installing .NET service using the Win32 API
 
 Extended by Dror Gluska
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoytSoft.ServiceBase.Base.Notifications
{
    /// <summary>
    /// Console Notification Window
    /// <para>There seems to be a problem to monitor the Message loop from console application, this 
    /// class solves that problem by having a hidden real window</para>
    /// </summary>
    internal class ConsoleNotificationWindow<T> : IDisposable
    {
        /// <summary>
        /// WndProc handler
        /// </summary>
        /// <param name="m"></param>
        public delegate void WndProcHandler(ref T m);

        /// <summary>
        /// Event for WndProc handler
        /// </summary>
        public event WndProcHandler WndProc;

        /// <summary>
        /// Transform Message Handler
        /// <para>If this property is null, no messages will propegate</para>
        /// </summary>
        public NotificationWindow<T>.TransformMessageDelegate TransformMessage { get; set; }

        /// <summary>
        /// Hidden Window Handle
        /// </summary>
        public IntPtr Handle = IntPtr.Zero;

        /// <summary>
        /// Message Queue
        /// </summary>
        private readonly ConcurrentQueue<T> _messages = new ConcurrentQueue<T>();

        /// <summary>
        /// Hidden Notification Window handle
        /// </summary>
        private NotificationWindow<T> _nw = null;

        /// <summary>
        /// Owning Thread for Window and Message pump
        /// </summary>
        private Thread _messageThread = null;

        /// <summary>
        /// Local timer for dispatching events when they are available in _messages queue
        /// </summary>
        private System.Threading.Timer _messageDispatcher = null;

        private readonly ManualResetEventSlim _threadStartNotification = new ManualResetEventSlim();

        /// <summary>
        /// Starts the Notification Window
        /// </summary>
        public void Start()
        {
            if (_messageThread != null)
            {
                throw new ApplicationException("Message thread is already running");
            }

            //Start thread with Notification Window
            _messageThread = new Thread(new ThreadStart(() =>
            {
                _nw = new NotificationWindow<T>(this.TransformMessage,this._messages);
                this.Handle = _nw.Handle;
                _threadStartNotification.Set();
                //start the message pump, this is a blocking method, that's why its in a different thread.
                Application.Run(_nw);
            }));
            _messageThread.Start();

            _threadStartNotification.Wait();

            //Start Timer for Event loop
            _messageDispatcher = new System.Threading.Timer(new TimerCallback(WndProcQueueHandler), null, 0, 2);
        }

        /// <summary>
        /// Stops the Notification Window
        /// </summary>
        public void Stop()
        {
            _messageDispatcher.Change(Timeout.Infinite, Timeout.Infinite);
            _messageDispatcher.Dispose();
            _messageDispatcher = null;
            _nw._stop = true;
            _messageThread.Join();
        }

        /// <summary>
        /// Private handler for local event loop
        /// </summary>
        /// <param name="o"></param>
        private void WndProcQueueHandler(object o)
        {
            while (!_messages.IsEmpty)
            {
                T tm;
                if (_messages.TryDequeue(out tm))
                {
                    if (WndProc == null)
                    {
                        throw new ApplicationException("WndProc should be assigned before messages are dispatched");
                    }
                    WndProc.Invoke(ref tm);
                }
            }
        }

        /// <summary>
        /// Notification Window
        /// </summary>
        internal class NotificationWindow<T> : Form
        {
            public ConcurrentQueue<T> MessagesQueue { get;private set;}
            public volatile bool _stop = false;
            public delegate T TransformMessageDelegate(ref Message m);

            /// <summary>
            /// Transform Message Handler
            /// <para>If this property is null, no messages will propegate</para>
            /// </summary>
            public TransformMessageDelegate TransformMessage {get;private set;}

            private System.Windows.Forms.Timer _timer;

            public NotificationWindow(TransformMessageDelegate transformMessage, ConcurrentQueue<T> messageQueue)
            {
                this.MessagesQueue = messageQueue;
                this.TransformMessage = transformMessage;

                Debug.Assert(this.MessagesQueue != null);
                Debug.Assert(this.TransformMessage != null);

                _timer = new System.Windows.Forms.Timer();
                _timer.Tick += new EventHandler(TimerCheck);
                _timer.Interval = 10;
                _timer.Start();
            }

            /// <summary>
            /// Timer check for termination request, it will close the window and release Application.Run
            /// </summary>
            private void TimerCheck(object o, EventArgs e)
            {
                if (_stop == true)
                {
                    this.Close();
                }
            }

            /// <summary>
            /// Hide when its shown.
            /// </summary>
            protected override void OnShown(EventArgs e)
            {
                base.OnShown(e);
                this.Hide();
            }


            /// <summary>
            /// WndProc handler, pushes all messages to event queue
            /// <para>The downside of doing it this way is that LParam and WParam are not valid in case they are pointers when the method exits</para>
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                
                if ((MessagesQueue == null) || (TransformMessage == null)) return;

                var tm = TransformMessage(ref m);
                if (tm != null)
                {
                    MessagesQueue.Enqueue(tm);
                }
            }
        }



        public void Dispose()
        {
            this.Stop();
        }
    }
}
