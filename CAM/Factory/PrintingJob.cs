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
        public class PrintingJob
        {
            #region Variables
            
            public float timePerMove = 1f;
            public string name;

            private Factory factory;
            private IMyProjector projector;
            private List<Instruction> instructions;
            private Instruction currentInstruction;
            private bool waiting = false;
            private int progress = 0;
            private uint lastMove = 0;
            #endregion

            #region Public Methods
            public PrintingJob(Factory factory, IMyProjector projector)
            {
                this.factory = factory;
                this.projector = projector;
                this.name = projector.CustomName;

                if (projector != null) {
                    ParsePrintOrder(projector.CustomData);
                }

                factory.output.Print(name + " has " + instructions.Count + " printing instructions attached");
            }

            public void Start()
            {
                ParsePrintOrder(projector.CustomData); //Remove this after debugging
                progress = 0;
                factory.output.Print("Starting job " + name + "...");
            }

            public void Abort()
            {

            }

            public void Update()
            {
                uint tick = factory.program.Tick;

                if (waiting) {
                    lastMove = tick;
                    waiting = currentInstruction.part.IsWorking;
                } else {
                    if (tick - lastMove > timePerMove) {
                        if (instructions.Count > progress) {
                            currentInstruction = instructions[progress];
                            currentInstruction.Trigger();
                            timePerMove = currentInstruction.duration * Program.TicksPerSecond;// * factory.timeMultiplier;
                            waiting = currentInstruction.wait;
                            lastMove = tick;
                            progress++;

                            factory.output.Print(tick + ": triggering action " + progress + ": " + currentInstruction);
                        }
                        else {
                            //Done!
                            factory.output.Print(tick + ": Printing job done!");
                            factory.job = null;
                        }
                    }
                }

                
            }
            #endregion

            #region Private Methods
            private void ParsePrintOrder(string order)
            {
                char[] stepSplitter = { ',', '\n' };
                char[] mainSpliter = { ':', ';' };
                char[] dataSplitter = { ' ' };

                string[] steps = order.Split(stepSplitter);

                instructions = new List<Instruction>();

                foreach (var step in steps) {
                    var init = step.Split(mainSpliter);

                    //Ignore any comments or poorly formatted steps
                    if (init.Length != 2 || step[0] == '/') {
                        continue;
                    }

                    IPrinterPart printerPart;
                    if (factory.printerDictionary.TryGetValue(init[0].ToLower(), out printerPart)) {

                        //We have found a part of this name so parse data
                        var dataString = init[1];
                        var data = dataString.Split(dataSplitter);

                        if (data.Length >= 2) {
                        
                            //we have at least initial command and data provided so extract those first
                            string command = data[0];
                            var values = ParseArray(data[1]);
                            bool wait = true;
                            double time = 0.1f;

                            //Parse any optional params
                            for (int i = 2; i < data.Length; i++) {
                                var optCommand = data[i].ToLower();
                                switch (optCommand) {
                                    case "nowait":
                                        wait = false;
                                        break;
                                    case "sleep":
                                        if (i + 1 < data.Length) {
                                            if (double.TryParse(data[i + 1], out time)) {
                                                i++;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            instructions.Add(new Instruction(printerPart, command, values, (float)time, wait));
                        }
                    }
                }
            }

            private double[] ParseArray(string arrayString)
            {
                char[] arraySplitter = { '/' };
                var splitString = arrayString.Split(arraySplitter);
                double[] values = new double[splitString.Length];

                for (int i = 0; i < values.Length; i++) {
                    double ret;
                    if (double.TryParse(splitString[i], out ret)) {
                        values[i] = ret;
                    }
                }

                return values;
            }
            #endregion

        }
    }
}
