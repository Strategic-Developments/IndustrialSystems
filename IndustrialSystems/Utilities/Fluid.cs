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
            return $"{Amount}L of fluid {GasName}";
        }

        public static Fluid CreateInvalid()
        {
            return new Fluid
            {
                GasName = null,
                Amount = 0,
            };
        }

        public bool IsInvalid()
        {
            return GasName == null;
        }
    }

    public struct FluidContainer
    {
        public Fluid Fluid;
        public int MaxAmount;

        public float PercentageFull => Fluid.Amount / MaxAmount;
        public static FluidContainer CreateInvalid()
        {
            return new FluidContainer
            {
                Fluid = Fluid.CreateInvalid(),
                MaxAmount = 0,
            };
        }

        public bool IsInvalid()
        {
            return Fluid.IsInvalid() || MaxAmount == 0;
        }
    }
}
