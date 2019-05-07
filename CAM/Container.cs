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
        public class Container
        {
            #region Variables
            public enum CycleJob {compile, sort, filter}
            private const int maxJobIndex = 4;

            public readonly IMyInventory inventory;
            public readonly Logistics logistics;
            public readonly string name;

            public CycleJob job = CycleJob.compile;
            public bool canFilter = true; //Set to false to prevent the logistics removing items

            public List<string> whiteList = new List<string>();

            public List<MyInventoryItem> oldItems = new List<MyInventoryItem>();
            public List<MyInventoryItem> items = new List<MyInventoryItem>();
            public Dictionary<string, int> itemDict = new Dictionary<string, int>();

            private List<int> itemsToSort = new List<int>();
            private List<int> itemsToFilter = new List<int>();
            #endregion

            #region Public Variables
            public Container(IMyCargoContainer container, Logistics logistics)
            {
                this.name = container.CustomName;
                this.inventory = container.GetInventory();
                this.logistics = logistics;
            }

            public Container(IMyInventory inventory, string name, Logistics logistics)
            {
                this.inventory = inventory;
                this.name = name;
                this.logistics = logistics;
            }

            public bool Update()
            {
                bool done = false;

                switch (job) {
                    case CycleJob.compile:
                        Compile();
                        break;
                    case CycleJob.sort:
                        Sort();
                        break;
                    case CycleJob.filter:
                        Filter();
                        done = true;
                        break;
                    default:
                        break;
                }

                CycleJobs();
                return done;
            }
            #endregion

            #region private Variables
            private void Compile()
            {
                //logistics.output.Print("Compiling " + name + "...", true);

                oldItems = new List<MyInventoryItem>(items);
                items = new List<MyInventoryItem>();
                itemsToSort = new List<int>();
                itemsToFilter = new List<int>();
                itemDict = new Dictionary<string, int>();

                inventory.GetItems(items);

                if (items.Count == 0) return;

                for (int i = 0; i < items.Count; i++) {
                    var item = items[i];
                    int index;

                    if (!itemDict.TryGetValue(item.Type.ToString(), out index)) {
                        itemDict.Add(item.Type.ToString(), i);
                        itemsToFilter.Add(i);
                    }
                    else {
                        //Hard coded tools to not stack. Bite me
                        if (item.Type.TypeId != "MyObjectBuilder_PhysicalGunObject" && item.Type.TypeId != "MyObjectBuilder_GasContainerObject")
                            itemsToSort.Add(i);
                    }
                }
            }

            /// <summary>
            /// Goes through and stacks any objects we can
            /// </summary>
            /// <param name="logistics"></param>
            private void Sort()
            {
                if (itemsToSort.Count == 0) return;

                logistics.output.Print("Sorting " + itemsToSort.Count + " items in " + name, true);

                for (int i = 0; i < itemsToSort.Count; i++) {

                    int index = itemsToSort[i];
                    MyInventoryItem item = items[index];

                    if (inventory.TransferItemTo(inventory, index, null, true)) {
                        logistics.output.Print("--- '" + item.Type.SubtypeId + "' [" + index + "] has been stacked");
                    } else {
                        logistics.output.Print("!!! '" + item.Type.SubtypeId + "' [" + index + "] failed to move");
                    }
                }
            }

            private void Filter()
            {
                if (itemsToFilter.Count == 0 || !canFilter) return;

                for (int i = 0; i < itemsToFilter.Count; i++) {

                    int index = itemsToFilter[i];
                    MyInventoryItem item = items[index];

                    if (!whiteList.Contains(item.Type.TypeId)) {
                        //This item is not legal- move it!
                        ContainerGroup group;

                        if (logistics.containerGroups.TryGetValue(item.Type.TypeId, out group)) {
                            if (group.bins.Count > 0) {
                                Container targetBin = group.bins[0];
                                if (inventory.TransferItemTo(targetBin.inventory, index, null, true)) {
                                    logistics.output.Print("--- '" + item.Type.SubtypeId + "' [" + index + "] moved to " + targetBin.name, true);
                                } else {
                                    logistics.output.Print("!!! '" + item.Type.SubtypeId + "' [" + index + "] failed to move to " + targetBin.name);
                                }
                            }
                        }
                    }
                }
            }

            private void CycleJobs()
            {
                job++;
                if ((int)job > maxJobIndex)
                    job = 0;
            }
            #endregion
        }
    }
}
