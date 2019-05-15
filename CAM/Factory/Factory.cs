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
            public const string kebabTag = "k";

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;

            private List<IMyProjector> allProjectors = new List<IMyProjector>();
            private List<IMyPistonBase> allPistons = new List<IMyPistonBase>();
            private List<IMyMotorStator> allRotors = new List<IMyMotorStator>();
            private IMyMotorStator centralRotor;

            private Dictionary<string, IPrinterPart> printerDictionary = new Dictionary<string, IPrinterPart>();

            private WelderArm leftArm;
            private WelderArm rightArm;
            private bool isPrinting = true;
            private float targetRotorPos = 0f;
            
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
                leftArm = new WelderArm(this, "Left Arm");
                rightArm = new WelderArm(this, "Right Arm");

                program.GridTerminalSystem.GetBlocksOfType<IMyProjector>(allProjectors, block => block.CustomName.Contains(tag));
                program.GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(allPistons, block => block.CustomName.Contains(tag));
                program.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(allRotors, block => block.CustomName.Contains(tag));

                if (allRotors.Count > 0) 
                    centralRotor = allRotors[0];

                foreach (var item in allPistons) {
                    //output.Print(item.CustomName);

                    string[] splits = item.CustomName.ToLower().Split(program.delimiterChars);

                    if (splits.Length > 4) {
                        string side = splits[2].ToLower();
                        string direction = splits[3].ToLower();

                        if (side == "r") {
                            rightArm.AddPiston(item, direction);
                        } else if (side == "l") {
                            leftArm.AddPiston(item, direction);
                        }
                    }
                }

                foreach (var projector in allProjectors) {
                    output.Print(projector.CustomName + " blocks: ");

                    int i = 0;
                    /*
                    foreach (var item in projector.RemainingBlocksPerType) {

                        var type = item.Key.GetType();
                        var count = item.Value;
                        output.Print("-- " + i + ": " + type);
                        i++;
                    }*/
                }

                leftArm.PrintObjects(output);
                rightArm.PrintObjects(output);

                output.SetHeader(Name);
            }

            public void Update()
            {
                StartUpdate();

                float rotorAngle = (centralRotor.Angle * 57.2958f);

                if (isPrinting) {
                    leftArm.Update();
                    rightArm.Update();

                    float range = (targetRotorPos - rotorAngle);
                    centralRotor.TargetVelocityRPM = range / 3f;
                }

                string header = Name + "\nLeft: " + leftArm.position + ", Right: " + rightArm.position + "\nRotor: " + rotorAngle + "/" + targetRotorPos;

                output.SetHeader(header);

                LateUpdate();
            }

            public void Trigger(string[] arguments)
            {
                switch (arguments[0].ToLower()) {
                    case "l":
                        if (arguments.Length >= 3)
                            leftArm.ManualPosition(arguments[1], arguments[2]);
                        break;
                    case "r":
                        if (arguments.Length >= 3)
                            rightArm.ManualPosition(arguments[1], arguments[2]);
                        break;
                    case "r+":
                        targetRotorPos += 90;
                        break;
                    case "r-":
                        targetRotorPos -= 90;
                        break;
                    default:
                        break;
                }

                if (targetRotorPos > 360) targetRotorPos -= 360;
                if (targetRotorPos < 0) targetRotorPos += 360;
            }
            #endregion

            #region Private Methods
            private void StartUpdate()
            {

            }

            private void LateUpdate()
            {
                output.Update();
            }
            #endregion
        }
    }
}
