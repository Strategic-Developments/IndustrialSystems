using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class ConveyorLine : IIndustrialSystemMachine
    {
        public ushort Frequency;

        public IIndustrialSystemMachine Destination;
        public IIndustrialSystemMachine Start;

        public List<IMyCubeBlock> Path;
        public List<ConveyorItem> Items;

        public ushort DistanceToInsertAtStart;

        public ConveyorLine(ushort frequency, List<IMyCubeBlock> blocks)
        {
            Frequency = frequency;
            Path = blocks;
            Items = new List<ConveyorItem>();

            DistanceToInsertAtStart = 0;
        }

        public void Close()
        {
            if (Start != null && Start is ConveyorLine)
            {
                ((ConveyorLine)Start).Destination = null;
            }
            if (Destination != null && Destination is ConveyorLine)
            {
                ((ConveyorLine)Destination).Start = null;
            }
        }

        public bool IsBlockAPartOf(IMyCubeBlock block)
        {
            return Path.Contains(block);
        }
    }
}
