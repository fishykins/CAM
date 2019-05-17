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
        /// A hardcoded routine for a very spesific piston/welder setup
        /// </summary>
        public class Factory : IRoutine
        {
            #region Variables
            public const string leftArmTag = "l";
            public const string rightArmTag = "r";
            public const string rotorTag = "k";

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;

            public Dictionary<string, IPrinterPart> printerDictionary = new Dictionary<string, IPrinterPart>();
            public List<PrintingJob> printingJobs = new List<PrintingJob>();
            public int printingJobIndex = 0;
            public PrintingJob job = null;
            public bool isPrinting = true;

            private List<IMyProjector> allProjectors = new List<IMyProjector>();
            private List<IMyPistonBase> allPistons = new List<IMyPistonBase>();
            private List<IMyMotorStator> allRotors = new List<IMyMotorStator>();
            private List<IMyShipWelder> allWelders = new List<IMyShipWelder>();
            #endregion

            #region Properties
            public string Name { get { return "Factory " + tag; } }
            public string Tag { get { return tag; } }
            #endregion

            #region Public Methods
            public Factory(Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
                output = new Output(program, tag, false);
            }

            public void Start()
            {
                WelderArm leftArm = new WelderArm(this, "Left Arm");
                WelderArm rightArm = new WelderArm(this, "Right Arm");
                ProjectorKebab rotor = new ProjectorKebab(this, "Rotor");

                printerDictionary.Add(leftArmTag, leftArm);
                printerDictionary.Add(rightArmTag, rightArm);
                printerDictionary.Add(rotorTag, rotor);

                program.GridTerminalSystem.GetBlocksOfType<IMyProjector>(allProjectors, block => block.CustomName.Contains(tag));
                program.GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(allPistons, block => block.CustomName.Contains(tag));
                program.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(allRotors, block => block.CustomName.Contains(tag));
                program.GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(allWelders, block => block.CustomName.Contains(tag));

                char[] tagDelimiter = { ':'};

                if (allRotors.Count > 0)
                    rotor.rotor = allRotors[0];

                foreach (var item in allPistons) {
                    string[] splits = item.CustomName.ToLower().Split(program.delimiterChars);

                    if (splits.Length > 4) {
                        string side = splits[2].ToLower();
                        string direction = splits[3].ToLower();

                        if (side == rightArmTag) {
                            rightArm.AddPiston(item, direction);
                        } else if (side == leftArmTag) {
                            leftArm.AddPiston(item, direction);
                        }
                    }
                }

                foreach (var item in allWelders) {
                    string[] splits = item.CustomName.ToLower().Split(tagDelimiter);

                    if (splits.Length >= 2) {
                        string itemTag = splits[1].ToLower();
                        IPrinterPart part;
                        if (printerDictionary.TryGetValue(itemTag, out part)) {
                            WelderArm arm = part as WelderArm;
                            if (arm != null) {
                                arm.welders.Add(item);
                                output.Print("Added welder " + item.CustomName + " to " + arm.Name);
                            }
                        }

                    }
                }

                foreach (var projector in allProjectors) {
                    printingJobs.Add(new PrintingJob(this, projector));
                }

                output.SetHeader(Name);
            }

            public void Update()
            {
                if (isPrinting) {
                    if (job != null)
                        job.Update();

                    foreach (var item in printerDictionary) {
                        item.Value.Update();
                    }
                }

                output.Update();
            }

            public void Trigger(string[] arguments)
            {
                if (arguments.Length > 0) {
                    switch (arguments[0].ToLower()) {
                        case "startjob":
                            StartJob();
                            break;
                        default:
                            PrintingPartTrigger(arguments);
                            break;
                    }
                }
            }
            #endregion

            #region Private Methods
            private void StartJob()
            {
                if (job != null) {
                    job.Abort();
                    job = null;
                    isPrinting = false;
                } else {
                    if (!(printingJobIndex < printingJobs.Count)) return;
                    job = printingJobs[printingJobIndex];
                    job.Start();
                    isPrinting = true;
                }
            }

            //Called manually by player. Should not be used by script-side commands as we can avoid all the parsing
            private void PrintingPartTrigger(string[] arguments)
            {
                string printerPart = arguments[0].ToLower();
                string command = arguments[1];

                double[] values = new double[0];
                bool canCall = true;

                if (arguments.Length > 2) {
                    //Parse all subsequent values
                    values = new double[arguments.Length - 2];

                    for (int i = 0; i < arguments.Length - 2; i++) {
                        double result;
                        if (double.TryParse(arguments[i + 2], out result)) {
                            values[i] = result;
                        }
                        else {
                            canCall = false;
                            break;
                        }
                    }
                }

                if (canCall) {
                    IPrinterPart part;
                    if (printerDictionary.TryGetValue(printerPart, out part)) {
                        part.Action(command, values);
                    }
                }
            }
            #endregion
        }
    }
}
