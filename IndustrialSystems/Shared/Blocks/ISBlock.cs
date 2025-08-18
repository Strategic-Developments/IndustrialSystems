using IndustrialSystems.Shared.Interfaces;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace IndustrialSystems.Shared.Blocks
{
    public class ISBlock<TBlock> : IIndustrialSystemMachine, IUpdateable where TBlock : IMyCubeBlock
    {
        public readonly TBlock Self;
        public IndustrialSystem ParentSystem;

        public ISBlock(TBlock Self, IndustrialSystem ParentSystem)
        {
            this.Self = Self;
            this.ParentSystem = ParentSystem;
        }
        public virtual void Close()
        {
            
        }

        public virtual void Update()
        {

        }

        public bool IsBlockAPartOf(IMyCubeBlock block)
        {
            return (IMyCubeBlock)Self == block;
        }
    }
}
