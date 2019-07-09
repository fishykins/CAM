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
        public class ManualProgram
        {
            #region Variables
            private Program program;

            #endregion

            #region Public Methods
            public ManualProgram(Program program)
            {
                this.program = program;
            }

            /// <summary>
            /// Called when script is manually triggered. 
            /// </summary>
            /// <param name="argument"></param>
            public void Trigger(string argument, Output output)
            {
                switch (argument) {
                    case "Continue":
                        program.Continue();
                        break;
                    case "togglePause":
                        if (program.Runtime.UpdateFrequency != UpdateFrequency.None) {
                            program.Pause();
                            output.Print("Paused");
                        }
                        else {
                            program.Continue();
                            output.Print("Unpaused");
                        }
                        break;
                    default:
                        parseTrigger(argument, output);
                        break;
                }
            }
            #endregion

            #region Private Methods
            private void parseTrigger(string argument, Output output)
            {
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] parts = argument.Split(delimiterChars);
                if (parts.Length >= 2) {
                    string tag = parts[0];
                    string[] args = new string[parts.Length - 1];

                    for (int i = 0; i < parts.Length - 1; i++) {
                        args[i] = parts[i + 1];
                    }

                    IRoutine routine;
                    if (program.tagDictionary.TryGetValue(tag, out routine)) {
                        routine.Trigger(args);
                        if (args.Length > 0)
                            output.Print(routine.Name + " has been passed trigger argument \n    -->" + args[0] + "...");
                    }


                    
                }
            }
            #endregion
        }
    }
}
