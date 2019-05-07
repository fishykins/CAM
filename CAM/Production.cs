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
        public class Production
        {
            public readonly IMyProductionBlock block;
            public readonly string name;
            public readonly Container input;
            public readonly Container output;

            private readonly Logistics logistics;

            public Production(IMyProductionBlock block, Logistics logistics)
            {
                this.block = block;
                this.name = block.CustomName;
                this.logistics = logistics;

                input = new Container(block.InputInventory, block.CustomName, logistics);
                output = new Container(block.OutputInventory, block.CustomName, logistics);

                input.canFilter = false;
            }

            public bool Update()
            {
                input.canFilter = block.IsQueueEmpty;
                return true;
            }
        }
    }
}
