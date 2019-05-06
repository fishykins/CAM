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
        public class Diagnostics : IRoutine
        {
            #region Variables
            public const int freeTicks = 20;
            public const double maxRuntime = 0.3f;
            public const int tickSampleRate = 30;

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;
            #endregion

            #region properties
            public string Name { get { return "Diagnostics"; } }
            #endregion

            #region Public Methods
            public Diagnostics(Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
                this.output = new Output(program, tag, false);
            }

            public void Start()
            {
                output.SetHeader("Diagnostics");
            }

            public void Update()
            {
                double lrt = program.Runtime.LastRunTimeMs;

                output.Print(program.Tick + ": " + lrt + "ms");
                output.Update();
            }
            #endregion

            #region Private Methods

            #endregion
        }
    }
}
