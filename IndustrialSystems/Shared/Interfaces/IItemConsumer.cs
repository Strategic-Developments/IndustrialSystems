using IndustrialSystems.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialSystems.Shared.Interfaces
{
    public interface IItemConsumer
    {
        /// <summary>
        /// Return true if item is fully consumed, false if not
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool AcceptItem(ref Item item);
    }
}
