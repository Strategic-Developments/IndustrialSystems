using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialSystems.Shared
{
    public interface IItemConsumer
    {
        bool AcceptItem(ref Item item);
    }
}
