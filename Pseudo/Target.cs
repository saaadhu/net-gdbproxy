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

        string ReadRegister(IEnumerable<string> args)
        {
            var reg = uint.Parse (args.First(), System.Globalization.NumberStyles.HexNumber);
            return GetEncodedRegisterContents(GetRegisterContents(reg));
        }

        protected abstract IEnumerable<uint> GetAllRegisterContents();
        protected abstract uint GetRegisterContents(uint reg);
        
        private void RegisterKnownCommands()
        {
            commands.Register("p", this.ReadRegister);
            commands.Register("g", this.ReadRegisters);
            commands.Register("?", this.ReportState);
            commands.Register("c", this.Continue);
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