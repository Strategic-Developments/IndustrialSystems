using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared.Interfaces
{
    public interface IIndustrialSystemMachine // we love polymorphism
    {
        void Close();

        Vector3D GetPosition();
    }
}
