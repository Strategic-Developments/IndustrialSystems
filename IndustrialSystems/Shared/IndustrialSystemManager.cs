using IndustrialSystems.Definitions;
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
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace IndustrialSystems.Shared
{
    public class IndustrialSystemManager
    {
        public static IndustrialSystemManager I;

        public Dictionary<long, IndustrialSystem> Systems;
        public ObjectPool<List<FluidContainer>> FluidsObjectPool;
        public ObjectPool<List<IMySlimBlock>> SlimBlockObjectPool;

        public static void Load()
        {
            I = new IndustrialSystemManager
            {
                Systems = new Dictionary<long, IndustrialSystem>(),
                FluidsObjectPool = new ObjectPool<List<FluidContainer>>(
                    () => new List<FluidContainer>(),
                    startSize: 10
                    ),
                SlimBlockObjectPool = new ObjectPool<List<IMySlimBlock>>(
                    () => new List<IMySlimBlock>(),
                    startSize: 10
                    ),
            };


            MyAPIGateway.Entities.OnEntityAdd += I.OnEntityAdd;
        }

        private void OnEntityAdd(IMyEntity obj)
        {
            if (obj is IMyCubeGrid)
            {
                if (obj.Physics == null)
                {
                    return;
                }
                IMyCubeGrid grid = obj as IMyCubeGrid;

                Systems.Add(grid.EntityId, new IndustrialSystem(grid));

                
            }
        }
        public static void Unload()
        {
            MyAPIGateway.Entities.OnEntityAdd -= I.OnEntityAdd;

            foreach (var machine in I.AllMachines(I.Systems.Values))
            {
                machine.Close();
            }

            I.Systems.Clear();
            I = null;
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

                                ((MyCubeGrid)block.CubeGrid).ChangeColorAndSkin(((MyCubeGrid)block.CubeGrid).GetCubeBlock(block.Position), new Vector3(-0.5f, 1, 1));
                            }
                        }
                        else
                        {
                            foreach (var block in currentConv.Path)
                            {
                                if (block == null)
                                    continue;

                                ((MyCubeGrid)block.CubeGrid).ChangeColorAndSkin(((MyCubeGrid)block.CubeGrid).GetCubeBlock(block.Position), new Vector3(0.5f, 1, 1) / currentConv.DistanceToInsertAtStart);
                            }
                        }
                        if (currentConv.Destination != null)
                        {
                            Vector4 colorRed = Color.Red;
                            MySimpleObjectDraw.DrawLine(currentConv.Path[0].GetPosition(), currentConv.Destination.GetPosition(),
                                MyStringId.GetOrCompute("WeaponLaser"), ref colorRed, 1, VRageRender.MyBillboard.BlendTypeEnum.AdditiveTop);
                        }
                        else
                        {
                            Vector4 colorRed = Color.Red;
                            MySimpleObjectDraw.DrawLine(currentConv.Path[0].GetPosition(), Vector3D.Zero,
                                MyStringId.GetOrCompute("WeaponLaser"), ref colorRed, 1, VRageRender.MyBillboard.BlendTypeEnum.AdditiveTop);
                        }
                        if (currentConv.Start != null)
                        {
                            Vector4 colorGreen = Color.Green;
                            MySimpleObjectDraw.DrawLine(currentConv.Path[0].GetPosition(), currentConv.Start.GetPosition(),
                                MyStringId.GetOrCompute("WeaponLaser"), ref colorGreen, 1, VRageRender.MyBillboard.BlendTypeEnum.AdditiveTop);
                        }
                        else
                        {
                            Vector4 colorGreen = Color.Green;
                            MySimpleObjectDraw.DrawLine(currentConv.Path[0].GetPosition(), Vector3D.Zero,
                                MyStringId.GetOrCompute("WeaponLaser"), ref colorGreen, 1, VRageRender.MyBillboard.BlendTypeEnum.AdditiveTop);
                        }


                    }

                    foreach (var currentConv in BackConveyors(Systems.Values))
                    {
                        Color colorBlue = Color.Blue;
                        BoundingBoxD box = new BoundingBoxD(new Vector3D(-1.25), new Vector3D(1.25));
                        MatrixD wm = currentConv.Path[0].WorldMatrix;
                        MySimpleObjectDraw.DrawTransparentBox(ref wm, ref box,
                            ref colorBlue, MySimpleObjectRasterizer.Wireframe, 1, 0.1f, null, MyStringId.GetOrCompute("WeaponLaser"),
                            false, -1, VRageRender.MyBillboard.BlendTypeEnum.AdditiveTop);
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
            MyAPIGateway.Parallel.ForEach(BackConveyors(Systems.Values), (conv) =>
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
                            continue;
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
                            && currentConv.DistanceToInsertAtStart >= currentConv.Frequency)
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
        private IEnumerable<ConveyorLine> BackConveyors(ICollection<IndustrialSystem> systems)
        {
            foreach (var system in systems)
            {
                foreach (var conv in system.BackConveyorLines)
                {
                    yield return conv;
                }
            }
        }

        private IEnumerable<ConveyorLine> AllConveyors(ICollection<IndustrialSystem> systems)
        {
            foreach (var system in systems)
            {
                foreach (var conv in system.AllConveyorLines)
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

        private IEnumerable<IIndustrialSystemMachine> AllMachines(ICollection<IndustrialSystem> systems)
        {
            foreach (var system in systems)
            {
                foreach (var machine in system.AllMachines.Values)
                {
                    yield return machine;
                }
            }
        }
    }
}
