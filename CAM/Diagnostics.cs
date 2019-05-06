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
            public const int sleepTimer = 100;
            public const string headerTitle = "Diagnostics";

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;

            private double peakLrt = -1f;
            private int lastSpike = -1;

            private int spikeCount = 0;
            private int spikeSum = 0;
            private float spikeAverage = 0;

            private string peakLrtString = "";
            #endregion

            #region properties
            public string Name { get { return headerTitle; } }
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

                if (lrt > peakLrt) {
                    peakLrt = lrt;
                    
                    peakLrtString = "Peak LRT: " + peakLrt + "ms";
                }

                if (lrt < maxRuntime) {
                    output.Print(lrt + "ms", true);
                } else {
                    int timeSinceLast = program.Tick - lastSpike;
                    lastSpike = program.Tick;
                    spikeCount++;
                    spikeSum += timeSinceLast;
                    spikeAverage = spikeSum / spikeCount;
                    output.SetHeader(headerTitle + "\n" + peakLrtString + "\nAverage spike width: " + spikeAverage);
                    program.output.PrintWarning("Max runtime was peaked: " + lrt + "ms (" + timeSinceLast + " ticks since last spike)");

                    int timer = (int)((lrt/ maxRuntime) * sleepTimer);
                    program.Sleep(sleepTimer);
                }
                
                output.Update();
            }
            #endregion

            #region Private Methods

            #endregion
        }
    }
}
