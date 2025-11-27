using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwerkr
{
    public class Eventr
    {
        public string name { get; }
        public event Action<string> received;

        public Eventr(string Name)
        {
            name = Name;
        }

        internal void Invoke(string data)
        {
            received?.Invoke(data);
        }
    }
}