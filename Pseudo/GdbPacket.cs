using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pseudo
{
    class GdbPacket
    {
        public string Data { get; set; }
        public ushort Checksum { get; set; }

        public override string ToString()
        {
            return "$" + Data + "#" + Checksum.ToString("X2");
        }

        public bool ValidateChecksum()
        {
            return ComputeChecksum(this.Data) == this.Checksum;
        }

        public void ComputeChecksum()
        {
            this.Checksum = ComputeChecksum(this.Data); 
        }

        private ushort ComputeChecksum(string data)
        {
            return (ushort)(data.Sum(c => (uint)c) % 256);
        }
    }
}
