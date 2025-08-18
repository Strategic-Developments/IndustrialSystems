using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Game.ModAPI;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystem
    {
        public int ModularId;

        public HashSet<ConveyorLine> AllConveyorLines;
        public HashSet<ConveyorLine> BackConveyorLines;

        public HashSet<Pipeline> AllPipes;

        public HashSet<IUpdateable> UpdateableBlocks;


        public IndustrialSystem(int modularId)
        {
            this.ModularId = modularId;

            AllConveyorLines = new HashSet<ConveyorLine>();
            BackConveyorLines = new HashSet<ConveyorLine>();
            AllPipes = new HashSet<Pipeline>();
            UpdateableBlocks = new HashSet<IUpdateable>();
        }
        

        
        public void AddPart(IMyCubeBlock b)
        {
            
        }

        public void RemovePart(IMyCubeBlock b)
        {
            
        }
    }
}
