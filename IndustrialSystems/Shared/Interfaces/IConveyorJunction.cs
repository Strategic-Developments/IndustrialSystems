using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;

namespace IndustrialSystems.Shared.Interfaces
{
    public interface IConveyorJunction : IIndustrialSystemMachine
    {
        bool Link(ConveyorLine line, IMyCubeBlock connector, bool isIncommingConnection);
    }
}
