using Defs = System.Collections.Generic.List<IndustrialSystems.Definitions.Definition>;
using Sandbox.ModAPI;
using static IndustrialSystems.Definitions.DefinitionConstants;
using System.Linq;
using VRage.Game;
using VRage.Game.Components;
using System;
using VRage.Utils;

namespace ISDefinitions
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class DefinitionSender : MySessionComponentBase
    {
        private bool SentDefinitions;
        private void OnModMessageRecieved(object obj)
        {
            MyLog.Default.WriteLine($"DefinitionSender OnModMessageRecieved called with {obj}");
            if (obj is ushort && !SentDefinitions)
            {
                ushort signal = (ushort)obj;

                if (signal == DefinitionSendSignal)
                {
                    SentDefinitions = true;

                    MyLog.Default.WriteLine($"DefinitionSender recieved signal. Sending definitions.");
                    foreach (var definitions in new Definitions().GetDefs())
                    {
                        MyLog.Default.WriteLine($"DefinitionSender sending definition {definitions.Base.SubtypeId}.");
                        MyAPIUtilities.Static.SendModMessage(MessageHandlerId, definitions.ConvertToObjectArray());
                    }
                }
            }
        }

        public override void LoadData()
        {
            SentDefinitions = false;
            MyLog.Default.WriteLine($"DefinitionSender registering message handler.");
            MyAPIUtilities.Static.RegisterMessageHandler(MessageHandlerId, OnModMessageRecieved);
            MyAPIUtilities.Static.SendModMessage(MessageHandlerId, ReadyToSendDefinitions);
            
        }
    }
}