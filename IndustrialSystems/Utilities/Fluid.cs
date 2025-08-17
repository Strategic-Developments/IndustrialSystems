using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialSystems.Utilities
{
    // todo
    public struct Fluid
    {
        public string GasName;
        public int Amount;

        public override string ToString()
        {
            return $"{Amount}L of gas {GasName}";
        }
    }
}
