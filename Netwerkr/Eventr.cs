using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwerkr
{
    public class Eventr
    {
        public string Name { get; }
        public event Action<string> Received;

        public Eventr(string name)
        {
            Name = name;
        }

        internal void Invoke(string data)
        {
            Received?.Invoke(data);
        }
    }
}