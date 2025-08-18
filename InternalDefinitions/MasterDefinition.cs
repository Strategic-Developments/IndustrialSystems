using Defs = System.Collections.Generic.List<IndustrialSystems.Definitions.Definition>;
using Sandbox.ModAPI;
using static IndustrialSystems.Definitions.DefinitionConstants;
using System.Linq;
using VRage.Game;
using VRage.Game.Components;

namespace ISDefinitions
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate, Priority = int.MinValue /* DO NOT MODIFY PRIORITY HERE!!!!!*/)]
    public partial class Definitions : MySessionComponentBase
    {
        public override void Init(MyObjectBuilder_SessionComponent ignore)
        {
            foreach (var definition in new Defs
            {
                // put all block definitions here

                Template_Drill,
                Template_GasRefiner,
                Template_OutputCargo,
                Template_Crusher,
                Template_Purifier,
                Template_Smelter,
                Template_ConveyorDefinitions,
                Template_ConveyorSplitter,

                Template_MaterialDefinitions,
                Template_Ice,

                // don't modify below here

            })
            {
                MyAPIGateway.Utilities.SendModMessage(MessageHandlerId, definition.ConvertToObjectArray());
            }
        }
    }
}