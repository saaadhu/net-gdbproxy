using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pseudo
{
    class CommandActionMap
    {
        List<KeyValuePair<string, Func<IEnumerable<string>, string>>> commands = new List<KeyValuePair<string, Func<IEnumerable<string>, string>>>();

        public void Register(string prefix, Func<IEnumerable<string>, string> command)
        {
            commands.Add(new KeyValuePair<string, Func<IEnumerable<string>, string>> (prefix, command));
        }

        public Func<IEnumerable<string>, string> Find(string data, out string args)
        {
            args = data;
            foreach (var pair in commands)
                if (data.StartsWith(pair.Key))
                {
                    args = data.Substring(pair.Key.Length);
                    return pair.Value;
                }
            return null;
        }
    }
}
