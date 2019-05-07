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
        public class ContainerGroup
        {
            public readonly string tag;
            public readonly string type;

            public List<Container> bins = new List<Container>();

            public ContainerGroup(string tag, string type)
            {
                this.tag = tag;
                this.type = type;
            }

            public void AddBin(Container container)
            {
                bins.Add(container);
                if (!container.whiteList.Contains(type))
                    container.whiteList.Add(type);
            }
        }
    }
}
