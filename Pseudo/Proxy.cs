using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Pseudo
{
    public class Proxy
    {
        TcpTransport transport;
        Target target;

        public Proxy(Target target)
        {
            this.target = target;
        }

        public void Listen(IPEndPoint p)
        {
            transport = new TcpTransport(p);
            transport.ClientConnected += new EventHandler<ClientConnectedEventArgs>(transport_ClientConnected);
            transport.Start();
        }

        void transport_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            var p = new GdbRSProtocol(e.Stream);
            p.CommandReceived += new EventHandler<CommandReceivedEventArgs>(p_CommandReceived);
            p.BeginSequence();
        }

        void p_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            string response;
            var p = (GdbRSProtocol)sender;

            if (!target.ExecuteCommand (e.Command, out response))
            {
                p.SendPacket("");
                return;
            }

            p.SendPacket(response);
        }
    }
}
