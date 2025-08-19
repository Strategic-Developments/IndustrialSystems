using IndustrialSystems.Shared.Interfaces;
using IndustrialSystems.Utilities;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystemManager
    {
        public static IndustrialSystemManager I;

        public Dictionary<int, IndustrialSystem> Systems;
        public ObjectPool<List<FluidContainer>> FluidsObjectPool;
        public Dictionary<long, IIndustrialSystemMachine> AllMachines;
        public static void Load()
        {
            I = new IndustrialSystemManager
            {
                Systems = new Dictionary<int, IndustrialSystem>(),
                FluidsObjectPool = new ObjectPool<List<FluidContainer>>(
                    () => new List<FluidContainer>(),
                    startSize: 10
                    ),
                AllMachines = new Dictionary<long, IIndustrialSystemMachine>(),
            };
            
        }
        public static void Unload()
        {
            foreach (var machine in I.AllMachines)
            {
                machine.Value.Close();
            }


            I.AllMachines.Clear();

            I = null;
        }
        public void OnPartAdd(int assemblyId, IMyCubeBlock block, bool isBasePart)
        {
            if (!Systems.ContainsKey(assemblyId))
                Systems.Add(assemblyId, new IndustrialSystem(assemblyId));

            Systems[assemblyId].AddPart(block);
        }
        public void OnPartRemove(int assemblyId, IMyCubeBlock block, bool isBasePart)
        {
            if (!Systems.ContainsKey(assemblyId))
                return;

            Systems[assemblyId].RemovePart(block);
        }

        public void OnAssemblyDestroy(int assemblyId)
        {
            Systems.Remove(assemblyId);
        }

        public void Update()
        {
            MyAPIGateway.Parallel.Do(UpdatePipes, UpdateConveyors);
            MyAPIGateway.Parallel.ForEach(Systems.Values, (system) =>
            {
                foreach (var updatable in system.UpdateableBlocks)
                    updatable.Update();
            });

            if (IndustrialSystemsSessionComp.I.DebugLevel >= 2)
            {
                try
                {
                    foreach (var currentConv in AllConveyors(Systems.Values))
                    {
                        if (currentConv.Items.Count == 0)
                        {
                            foreach (var block in currentConv.Path)
                            {
                                if (block == null)
                                    continue;

                                ((MyCubeGrid)block.CubeGrid).ChangeColorAndSkin(((MyCubeGrid)block.CubeGrid).GetCubeBlock(block.Position), Vector3.Zero);
                            }
                        }
                        else
                        {
                            foreach (var block in currentConv.Path)
                            {
                                if (block == null)
                                    continue;

                                ((MyCubeGrid)block.CubeGrid).ChangeColorAndSkin(((MyCubeGrid)block.CubeGrid).GetCubeBlock(block.Position), Vector3.One);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLog.Default.Error(ex.ToString());
                }
            }
        }
        public void UpdatePipes()
        {
            MyAPIGateway.Parallel.ForEach(AllPipes(Systems.Values), (pipe) =>
            {
                if (pipe.StoredFluid.IsInvalid())
                    return;

                var fluids = I.FluidsObjectPool.Pop();

                foreach (var machine in pipe.ConnectedMachines)
                {
                    fluids.Add(machine.GetFluidContainer(pipe.StoredFluid.Fluid.GasName));
                }
            });
        }
        public void UpdateConveyors()
        {
            MyAPIGateway.Parallel.ForEach(AllConveyors(Systems.Values), (conv) =>
            {
                int iter = 0;
                ConveyorLine currentConv = conv;

                do
                {
                    if (iter++ >= ushort.MaxValue) // arbitrary value to stop infinite loop
                        throw new Exception();

                    for (int i = 0; i < currentConv.Items.Count; i++)
                    {
                        var item = currentConv.Items[i];


                        if (item.Distance > currentConv.Frequency)
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

                            if (consumer.CanAcceptItem(currentConv, item.Item))
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
                            && currentConv.DistanceToInsertAtStart > currentConv.Frequency)
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
        private IEnumerable<ConveyorLine> AllConveyors(ICollection<IndustrialSystem> systems)
        {
            foreach (var system in systems)
            {
                foreach (var conv in system.BackConveyorLines)
                {
                    yield return conv;
                }
            }
        }

        private IEnumerable<Pipeline> AllPipes(ICollection<IndustrialSystem> systems)
        {
            foreach (var system in systems)
            {
                foreach (var pipe in system.AllPipes)
                {
                    yield return pipe;
                }
            }
        }
    }
}
