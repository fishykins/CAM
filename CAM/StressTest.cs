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
        public class StressTest : IRoutine
        {
            #region Variables
            public const int forloopCount = 6;

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;
            #endregion

            #region properties
            public string Name { get { return "Stress Test " + tag; } }
            #endregion

            #region Public Methods
            public StressTest(Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
                this.output = new Output(program, tag, false);
            }


            public void Start()
            {
                output.SetHeader(Name);
            }

            public void Update()
            {

                return;

                /*
                string text = forloopCount.ToString() + " g";

                for (int i = 0; i < forloopCount; i++) {
                    text += "o";
                }

                text += "n";

                output.Print(text, true);
                output.Update();*/
            }
            #endregion

            #region Private Methods

            #endregion
        }
    }
}
