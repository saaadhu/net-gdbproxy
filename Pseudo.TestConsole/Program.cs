using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pseudo.TestConsole
{
    class TestTarget : Target
    {
        public TestTarget()
            : /* AVR base(Enumerable.Concat(Enumerable.Repeat(1, 33), Enumerable.Repeat(2, 1)).Concat(Enumerable.Repeat(4, 1))) */
            base(Enumerable.Repeat(4, 26))
        {
        }
        protected override string Continue(long addr)
        {
            throw new NotImplementedException();
        }

        protected override string Continue()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<uint> GetAllRegisterContents()
        {
            return Enumerable.Repeat((uint)0, 26);
        }

        protected override bool SetAllRegisterContents(IEnumerable<uint> contents)
        {
            return true;
        }

        protected override uint GetRegisterContents(uint reg)
        {
            return 0;
        }

        protected override IEnumerable<byte> GetMemoryContents(uint addr, uint length)
        {
            return new byte[] { 0xCA, 0xFE, 0xBA, 0xBE };
        }

        protected override bool SetMemoryContents(uint addr, uint length, IEnumerable<byte> contents)
        {
            return true;
        }

        protected override void Kill()
        {
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var t = new TestTarget();
            var proxy = new Proxy(t);
            proxy.Listen(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 5565));
        }
    }
}
