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

        public HashSet<IUpdateable> UpdateableBlocks;


        public IndustrialSystem(int modularId)
        {
            this.ModularId = modularId;

            AllConveyorLines = new HashSet<ConveyorLine>();
            BackConveyorLines = new HashSet<ConveyorLine>();
        }
        
        public void UpdateConveyors()
        {
            MyAPIGateway.Parallel.ForEach(BackConveyorLines, (conv) =>
            {
                // todo: make this an option
                const ushort ItemSize = 16;

                int iter = 0;
                ConveyorLine currentConv = conv;

                do
                {
                    if (iter++ >= ushort.MaxValue) // arbitrary value to stop infinite loop
                        throw new Exception();

                    for (int i = 0; i < currentConv.Items.Count; i++)
                    {
                        var item = currentConv.Items[i];

                        
                        if (item.Distance > ItemSize)
                        {
                            item.Distance--;
                            currentConv.DistanceToInsertAtStart++;
                            currentConv.Items[i] = item;
                            break;
                        }

                        if (currentConv.Destination == null)
                            continue;


                        var nextBlock = currentConv.Destination;

                        if (nextBlock is IItemConsumer)
                        {
                            var consumer = (IItemConsumer)nextBlock;

                            if (consumer.CanAcceptItem(item.Item))
                            {
                                if (item.Distance > 0)
                                {
                                    item.Distance--;
                                    currentConv.DistanceToInsertAtStart++;
                                    currentConv.Items[i] = item;
                                }
                                else
                                {
                                    consumer.AcceptItem(item.Item);

                                    currentConv.Items.RemoveAt(i);
                                    i--;
                                }
                                break;
                            }
                        }

                        ConveyorLine nextConv = (ConveyorLine)nextBlock;

                        if (nextConv.DistanceToInsertAtStart == 0)
                            continue;

                        if (item.Distance > 0)
                        {
                            item.Distance--;
                            currentConv.DistanceToInsertAtStart++;
                            currentConv.Items[i] = item;
                        }
                        else
                        {
                            item.Distance = nextConv.DistanceToInsertAtStart;
                            nextConv.DistanceToInsertAtStart = 0;
                            nextConv.Items.Add(item);

                            currentConv.Items.RemoveAt(i);
                            i--;
                        }
                        break;
                    }

                    if (currentConv.Start == null)
                    {
                        break;
                    }
                    else if (currentConv.Start is IItemProducer
                            && currentConv.DistanceToInsertAtStart > ItemSize)
                    {
                        IItemProducer prod = (IItemProducer)currentConv.Start;

                        Item itemToAdd;
                        if (prod.GetNextItemFor(currentConv, out itemToAdd))
                        {
                            currentConv.Items.Add(new ConveyorItem(
                                itemToAdd,
                                currentConv.DistanceToInsertAtStart
                                ));

                            currentConv.DistanceToInsertAtStart = 0;
                        }
                    }
                    currentConv = currentConv.Start as ConveyorLine;
                }
                while (currentConv != null);
            });
        }

        
        public void AddPart(IMyCubeBlock b)
        {
            
        }

        public void RemovePart(IMyCubeBlock b)
        {
            
        }


        public void Update()
        {
            UpdateConveyors();

            foreach (var updatable in UpdateableBlocks)
                updatable.Update();
        }

    }
}
