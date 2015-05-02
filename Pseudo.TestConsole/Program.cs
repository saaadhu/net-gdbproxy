using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pseudo.TestConsole
{
    class TestTarget : Target
    {
        public TestTarget()
            : base(1)
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
            return Enumerable.Repeat((uint)0, 36);
        }

        protected override uint GetRegisterContents(uint reg)
        {
            return 0;
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
