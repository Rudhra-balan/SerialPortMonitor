using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPortBus.Models
{
    public class CrossoverPortPair
    {
        public CrossoverPortPair(string portNameA, string portNameB, int number)
        {
            PortNameA = portNameA;
            PortNameB = portNameB;
            PairNumber = number;
        }

        public string PortNameA { get; }
        public string PortNameB { get; }
        public int PairNumber { get; }
    }
}
