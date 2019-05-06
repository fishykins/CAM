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
    partial class Program : MyGridProgram
    {
        #region Variables
        private int routineCount;
        private int tick = 0;
        private int cycle = 0;
        private int routineIndex = 0;

        private List<IRoutine> routines = new List<IRoutine>();
        private IRoutine diagnostics;
        private GridAnalyser gridAnalyser;
        private Output output;
        #endregion

        #region Properties
        public int Cycle { get { return cycle; } }
        public int RoutineIndex { get { return routineIndex; } }
        public int RoutineCount { get { return routineCount; } }
        public int Tick { get { return tick; } }
        public GridAnalyser Analyser { get { return gridAnalyser; } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize script here. This is essentially "free" computing, and wont affect the program average runtime...
        /// </summary>
        public Program()
        {
            //Set how often this runs
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            //Compile core variables
            output = new Output(this, "[M]");
            diagnostics = new Diagnostics(this, "[D]");

            //Add routines here
            routines.Add(new StressTest(this, "[ST]"));

            InitRoutines();
        }

        /// <summary>
        /// Main loop where coroutine.Update() is called. 
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="updateSource"></param>
        public void Main(string argument, UpdateType updateSource)
        {
            SetDisplayHeader();

            //Run diagnostics every loop
            diagnostics.Update();

            //Loop index
            if (routineIndex >= routineCount) {
                cycle++;
                routineIndex = 0;
            }

            //Run a coroutine
            routines[routineIndex].Update();

            //Updates
            output.Update();
            routineIndex++;
            tick++;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes all routines using their Start() method
        /// </summary>
        private void InitRoutines()
        {
            foreach (var routine in routines) {
                routine.Start();
            }

            routineCount = routines.Count;
        }

        /// <summary>
        /// Handles the main display
        /// </summary>
        private void SetDisplayHeader()
        {
            output.SetHeader("Main Thread\n" +
                "Cycle " + cycle + "\n" +
                "Routine '" + routines[routineIndex].Name + "'\n"
                );
        }
        #endregion
    }
}