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
        public class Output
        {
            #region Variables
            //Config settings
            private const string headderBuff = "==================================================================";
            private const int maxLength = 20;

            //Main Variables
            private readonly Program program;
            private readonly string tag;
            private List<IMyTextSurface> screens;
            private bool autoUpdate;

            private string header = "";
            private string displayText = "";
            private List<String> messages = new List<string>();

            private int lastUpdate = -1;
            private int lastInteraction = 0;
            #endregion

            #region properties

            #endregion

            #region Public Methods
            public Output(Program program, string tag, bool autoUpdate = false)
            {
                this.program = program;
                this.tag = tag;
                this.screens = program.Analyser.FindScreens(tag, "main");
                this.autoUpdate = autoUpdate;

                if (screens == null) {
                    program.Echo("ERROR: No screens given when spawning!");
                }
            }

            /// <summary>
            /// Adds text to display
            /// </summary>
            /// <param name="text"></param>
            public void Print(string text, bool printMetaData = false)
            {
                lastInteraction = program.Tick;

                if (printMetaData)
                    text = "[" + program.Tick + "]:" + text;

                messages.Insert(0, text);

                if (autoUpdate)
                    Update();
            }

            public void PrintWarning(string text)
            {
                Print("WARNING [" + program.Tick + "]:" + text);
            }

            /// <summary>
            /// Sets the header of the display
            /// </summary>
            /// <param name="text"></param>
            public void SetHeader(string text)
            {
                lastInteraction = program.Tick;
                header = text;

                if (autoUpdate)
                    Update();
            }

            /// <summary>
            /// Clears the console
            /// </summary>
            public void Clear()
            {
                lastInteraction = program.Tick;
                messages = new List<string>();

                if (autoUpdate)
                    Update();
            }

            /// <summary>
            /// Updates the display(s)
            /// </summary>
            public void Update()
            {
                if (lastInteraction == lastUpdate) return;
                lastUpdate = program.Tick;

                //Cull old messages
                if (messages.Count > maxLength) {
                    messages.RemoveRange(maxLength, messages.Count - maxLength);
                }

                //Compile all the text
                displayText = headderBuff + "\n" + header + "\n" + headderBuff + "\n";

                for (int i = 0; i < messages.Count; i++) {
                    displayText += messages[i] + "\n";
                }

                //Write to screen(s)
                foreach (var screen in screens) {
                    screen.WriteText(displayText);
                }
            }
            #endregion

            #region Private Methods

            #endregion
        }
    }
}
