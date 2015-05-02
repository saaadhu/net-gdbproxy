using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Pseudo
{
    class ClientConnectedEventArgs : EventArgs
    {
        public Stream Stream {get; private set;}
        public ClientConnectedEventArgs (Stream s)
        {
            this.Stream = s;
        }
    }

    class TcpTransport
    {
        TcpListener listener;
        
        //FIXME: Only one client at a time.
        TcpClient client;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        
        public TcpTransport(IPEndPoint p)
        {
            listener = new TcpListener(p);
        }

        private volatile bool stopAccepting;

        public void Start()
        {   
            listener.Start();
            while (!stopAccepting)
            {
                client = listener.AcceptTcpClient();
                RaiseClientConnected(client.GetStream());
            }
        }

        private void RaiseClientConnected(Stream s)
        {
            var c = ClientConnected;
            if (c != null)
                c(this, new ClientConnectedEventArgs(s));
        }

        public void Stop()
        {
            if (client != null)
                client.Close();
            stopAccepting = true;
            listener.Stop();
        }
    }
}
