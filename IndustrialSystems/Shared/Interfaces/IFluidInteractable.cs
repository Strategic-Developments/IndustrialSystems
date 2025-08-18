using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialSystems.Shared.Interfaces
{
    public interface IFluidInteractable : IIndustrialSystemMachine
    {
        bool CanAcceptFluid(string fluidName);

        FluidContainer GetFluidContainer(string fluidName);

        void EqualizeFluid(string fluidName, float fillAmount);
    }
}
