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
        public struct Instruction
        {
            public IPrinterPart part;
            public string commnad;
            public double[] values;
            public bool wait;
            public float duration;

            public Instruction(IPrinterPart part, string commnad, double[] values, float duration = .1f, bool wait = true)
            {
                this.part = part;
                this.commnad = commnad;
                this.values = values;
                this.wait = wait;
                this.duration = duration;
            }

            public Instruction(IPrinterPart part, string command, float duration = .1f, bool wait = true)
            {
                this.part = part;
                this.commnad = command;
                this.values = new double[0];
                this.wait = wait;
                this.duration = duration;
            }

            public void Trigger()
            {
                part.Action(commnad, values);
            }
        }
    }
}
