using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Logistics : IRoutine
        {
            #region Variables
            public const string binTag = "Bin";
            public const string oreTag = "Ores";
            public const string ingotTag = "Ingots";
            public const string componentTag = "Components";
            public const string toolsTag = "Tools";

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;

            public Dictionary<string, ContainerGroup> containerGroups = new Dictionary<string, ContainerGroup>();
            public Dictionary<string, string> containerTypeKeys = new Dictionary<string, string>();
            public List<string> containerTags = new List<string>();
            public List<string> containerTypes = new List<string>();

            private int tick = 0;
            private int containerCount = 0;
            private int productionCount = 0;
            private int cycleIndex = 0;

            private List<Container> containers = new List<Container>();
            private List<Production> productionUnits = new List<Production>();
            private List<Container> bins = new List<Container>();

            private enum LogisticsJob { containers, production};
            private LogisticsJob job = LogisticsJob.containers;
            #endregion

            #region properties
            public string Name { get { return "Logistics " + tag; } }
            public string Tag { get { return tag; } }
            #endregion

            #region public Methods
            public Logistics(Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
                this.output = new Output(program, tag, false);

                InitGroup(componentTag, "MyObjectBuilder_Component");
                InitGroup(oreTag, "MyObjectBuilder_Ore");
                InitGroup(ingotTag, "MyObjectBuilder_Ingot");
                InitGroup(toolsTag, "MyObjectBuilder_PhysicalGunObject");
            }

            public void Start()
            {
                output.SetHeader(Name);
                FindContainers();
                FindProduction();
                CompileContainers();

                output.Print("Found " + containerCount + " containers");

                foreach (string type in containerTypes) {
                    ContainerGroup group = containerGroups[type];
                    output.Print("Group " + group.tag + " found " + group.bins.Count + " bins");
                }
            }

            public void Update()
            {
                StartUpdate();

                switch (job) {
                    case LogisticsJob.containers:
                        UpdateContainers();
                        break;
                    case LogisticsJob.production:
                        UpdateProduction();
                        break;
                    default:
                        break;
                }

                EndUpdate();
            }

            public void Trigger(string[] arguments)
            {

            }
            #endregion

            #region Private Methods
            private void UpdateContainers()
            {
                if (cycleIndex >= containerCount) {
                    cycleIndex = 0;
                    job = LogisticsJob.production;
                }
                    

                if (containerCount > 0) {
                    if (containers[cycleIndex].Update())
                        cycleIndex++;
                }
            }

            private void UpdateProduction()
            {
                if (cycleIndex >= productionCount) {
                    cycleIndex = 0;
                    job = LogisticsJob.containers;
                }

                if (containerCount > 0) {
                    if (productionUnits[cycleIndex].Update())
                        cycleIndex++;
                }
            }

            /// <summary>
            /// Creates a type group for filtering
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="type"></param>
            private void InitGroup(string tag, string type)
            {
                containerGroups.Add(type, new ContainerGroup(tag, type));
                containerTypeKeys.Add(tag, type);
                containerTags.Add(tag);
                containerTypes.Add(type);
            }

            private void StartUpdate() {
                output.SetHeader(Name + " " + tick);
            }

            private void EndUpdate()
            {
                output.Update();
                tick++;
            }

            /// <summary>
            /// Gets all containers assosiated with this tag
            /// </summary>
            private void FindContainers()
            {
                List<IMyCargoContainer> tempList = new List<IMyCargoContainer>();
                program.GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(tempList, container => container.CustomName.Contains(tag));

                foreach (var item in tempList) {
                    Container container = new Container(item, this);
                    containers.Add(container);
                }
            }

            private void FindProduction()
            {
                List<IMyProductionBlock> tempList = new List<IMyProductionBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyProductionBlock>(tempList, item => item.CustomName.Contains(tag));

                foreach (var item in tempList) {
                    Production prod = new Production(item, this);
                    productionUnits.Add(prod);
                    containers.Add(prod.output);
                    containers.Add(prod.input);
                }

                productionCount = productionUnits.Count;
            }

            private void CompileContainers()
            {
                foreach (var container in containers) {

                    if (container.name.Contains(binTag)) {
                        bins.Add(container);

                        //Finds any types assosiated with this container from tags
                        string type = "goon";

                        foreach (string tag in containerTags) {
                            if (container.name.Contains(tag)) {
                                type = containerTypeKeys[tag];
                                break;
                            }
                        }

                        //Looks for container groups with out tag
                        ContainerGroup group;
                        if (containerGroups.TryGetValue(type, out group)) {
                            group.AddBin(container);
                        }
                    }
                }

                containerCount = containers.Count;
            }
            #endregion
        }
    }
}
