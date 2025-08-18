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
        void EqualizeFluidWith(ref FluidContainer incomingPipe);
    }
}
