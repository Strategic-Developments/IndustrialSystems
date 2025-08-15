using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndustrialSystems.Definitions;
using IndustrialSystems.Utilities;

namespace IndustrialSystems.Shared
{
    // needs a redo tbh
    public interface IItemProducer
    {
        Item GetProducedItem();
    }
}