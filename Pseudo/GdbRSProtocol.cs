using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pseudo
{   
    class CommandReceivedEventArgs : EventArgs
    {
        public string Command { get; private set; }
        public CommandReceivedEventArgs (string command)
        {
            this.Command = command;
        }
    }

    class GdbRSProtocol
    {
        Stream s;
        BinaryWriter w;
        BinaryReader r;
        public event EventHandler<CommandReceivedEventArgs> CommandReceived;

        public GdbRSProtocol(Stream s)
        {
            this.s = s;
            r = new BinaryReader(s);
            w = new BinaryWriter(s, Encoding.ASCII);
        }

        public void BeginSequence()
        {
            GdbPacket p;
            while ((p = ReadOnePacket()) != null)
            {
                if (p.ValidateChecksum())
                    w.Write("+");
                else
                {
                    w.Write("-");
                    continue;
                }

                RaiseDataRead(p);
            }
        }

        private void RaiseDataRead(GdbPacket p)
        {
            Console.WriteLine("REQ: " + p.Data);
            var e = CommandReceived;
            if (e != null)
                e(this, new CommandReceivedEventArgs(p.Data));
        }

        private GdbPacket ReadOnePacket()
        {
            //TODO: Unescape
            string data = "";
            ushort checksum = 0;

            char c;
            while ((c = r.ReadChar()) != '$')
                ;
            while ((c = r.ReadChar()) != '#')
                data += c.ToString();

            string checksumstr = new string(r.ReadChars(2));
            checksum = ushort.Parse(checksumstr, System.Globalization.NumberStyles.HexNumber);

            return new GdbPacket() { Data = data, Checksum = checksum };
        }

        public void SendPacket(string data)
        {
            //TODO: Escape
            var p = new GdbPacket() { Data = data };
            p.ComputeChecksum();

            var contents = p.ToString();
            Console.WriteLine("RES: " + contents);

            do
            {
                w.Write(contents);
                w.Flush();
            }
            while (r.ReadChar() != '+');
            
        }
    }
}
