using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pseudo
{
    public abstract class Target
    {
        CommandActionMap commands = new CommandActionMap();

        public int RegisterSize { get; private set; }

        public Target(int registerSize)
        {
            RegisterSize = registerSize;
            RegisterKnownCommands();
        }

        string Continue(IEnumerable<string> args)
        {
            if (args.Any())
                return this.Continue(long.Parse(args.First()));
            else
                return this.Continue();
        }

        internal bool Killed { get; private set; }

        protected abstract string Continue(long addr);
        protected abstract string Continue();

        bool ishalted = true;
        StopReason haltReason = StopReason.Breakpoint;
        uint haltValue = 0;

        enum StopReason
        {
            None,
            Watchpoint,
            Breakpoint,
            Signal
        }

        string ReportState(IEnumerable<string> args)
        {
            if (ishalted == false)
                return "OK";

            if (haltReason == StopReason.Breakpoint)
                return "S05";

            if (haltReason == StopReason.Signal)
                return "S" + haltValue.ToString("X2");

            throw new InvalidOperationException("Other halt reasons not yet handled");
            // TODO: Handle watchpoint
        }

        private string GetEncodedRegisterContents(uint input)
        {
            var s = input.ToString("X" + RegisterSize.ToString());
            if (s.Length % 2 == 0)
                return s;
            return "0" + s;
        }

        string ReadRegisters(IEnumerable<string> args)
        {
            return string.Join("",
                GetAllRegisterContents()
                .Select(c => GetEncodedRegisterContents(c)));
        }

        string WriteRegisters(IEnumerable<string> args)
        {
            List<uint> registerContents = new List<uint>();
            IEnumerable<char> c = args.First();
            while (c.Any())
            {
                var x = string.Join ("", c.Take(RegisterSize * 2));
                c = c.Skip(RegisterSize * 2);
                registerContents.Add(uint.Parse(x, System.Globalization.NumberStyles.HexNumber));
            }

            return SetAllRegisterContents(registerContents) ? "OK" : "E";
        }

        string ReadRegister(IEnumerable<string> args)
        {
            var reg = uint.Parse (args.First(), System.Globalization.NumberStyles.HexNumber);
            return GetEncodedRegisterContents(GetRegisterContents(reg));
        }

        string ReadMemory(IEnumerable<string> args)
        {
            IEnumerable<byte> contents = GetMemoryContents(uint.Parse(args.First()), uint.Parse(args.ElementAt(1)));
            var builder = new StringBuilder();

            foreach (var b in contents)
                builder.Append(b.ToString("X2"));

            return builder.ToString();
        }

        string WriteMemory(IEnumerable<string> args)
        {
            List<byte> contents = new List<byte>();

            var addr = uint.Parse(args.ElementAt(0), System.Globalization.NumberStyles.HexNumber);
            var length = uint.Parse(args.ElementAt(1), System.Globalization.NumberStyles.HexNumber);
            var c = args.ElementAt(2);
            
            for (int i = 0; i<c.Length; i += 2)
            {
                var x = c[i].ToString() + c[i + 1].ToString();
                contents.Add(byte.Parse(x, System.Globalization.NumberStyles.HexNumber));
            }

            return SetMemoryContents(addr, length, contents) ? "OK" : "E";
        }

        protected abstract IEnumerable<uint> GetAllRegisterContents();
        protected abstract bool SetAllRegisterContents(IEnumerable<uint> contents);
        
        protected abstract uint GetRegisterContents(uint reg);

        protected abstract IEnumerable<byte> GetMemoryContents(uint addr, uint length);
        protected abstract bool SetMemoryContents(uint addr, uint length, IEnumerable<byte> contents);

        string Kill(IEnumerable<string> args)
        {
            Kill();
            Killed = true;
            return "";
        }

        protected abstract void Kill();
        
        private void RegisterKnownCommands()
        {
            commands.Register("p", this.ReadRegister);
            commands.Register("g", this.ReadRegisters);
            commands.Register("G", this.WriteRegisters);
            commands.Register("?", this.ReportState);
            commands.Register("c", this.Continue);
            commands.Register("m", this.ReadMemory);
            commands.Register("M", this.WriteMemory);
            commands.Register("k", this.Kill);
        }

        protected internal virtual bool ExecuteCommand(string command, out string response)
        {
            response = "";
            string args;
            var c = commands.Find(command, out args);
            
            if (c == null)
                return false;
            
            response = c(args.Split(',', ';', ':'));
            return true;

        }
    }
}