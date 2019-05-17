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
        /// <summary>
        /// Diagnostics monitors the performance of the script, and will step in to prevent egg on face if need be
        /// </summary>
        public class Diagnostics : IRoutine
        {
            #region Variables
            public const int freeTicks = 20;
            public const double maxRuntime = 0.9f;
            public const int tickSampleRate = 30;
            public const int sleepTimer = 200;
            private const float sietaTime = 8;
            public const string headerTitle = "Diagnostics";

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;

            private double peakLrt = -1f;
            private double averageLrt = 0f;

            private string peakLrtString = "";
            #endregion

            #region properties
            public string Name { get { return headerTitle; } }
            public string Tag { get { return tag; } }
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
                if (program.Tick < freeTicks) return;

                double lrt = program.Runtime.LastRunTimeMs;

                averageLrt = averageLrt * 0.99 + lrt * 0.01;

                //Throttle if average goes over
                if (averageLrt > maxRuntime) {

                    output.PrintWarning("Max average runtime was peaked: " + averageLrt + "ms");
                    program.Pause();
                    averageLrt = 0f;
                } else {
                    output.Print(lrt + "ms", true);
                }

                //Track big spikes
                if (lrt > peakLrt) {
                    peakLrt = lrt;
                    peakLrtString = "Peak LRT: " + peakLrt + "ms";
                }

                output.SetHeader(headerTitle + "\n" + "Average Runtime: " + averageLrt);
                output.Update();
            }

            public void Trigger(string[] arguments)
            {

            }
            #endregion

            #region Private Methods

            #endregion
        }
    }
}
