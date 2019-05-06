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
        //Settings
        private const string programTag = "[M]";
        private const string diagnosticTag = "[D]";
        private const string stressTestTag = "[ST]";
        
        //Variables
        private int routineCount;
        private int tick = 0;
        private int cycle = 0;
        private int routineIndex = 0;

        private ManualProgram manualProgram;
        private List<IRoutine> routines = new List<IRoutine>();
        private IRoutine diagnostics;
        private GridAnalyser gridAnalyser;
        private Output output;
        private IRoutine currentRoutine;
        private IMyTimerBlock timer;

        private UpdateFrequency frequencyAtPause;
        private int sleepTimeLeft = 0;
        
        
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
            //Compile core variables
            manualProgram = new ManualProgram(this);
            gridAnalyser = new GridAnalyser(this);
            output = new Output(this, programTag);
            timer = gridAnalyser.GetTimer(programTag);
            diagnostics = new Diagnostics(this, diagnosticTag);
            diagnostics.Start();
            
            //Add routines here
            routines.Add(new StressTest(this, stressTestTag));
            InitRoutines();

            //Set how often this runs
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        /// <summary>
        /// Main loop where coroutine.Update() is called. 
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="updateSource"></param>
        public void Main(string argument, UpdateType updateSource)
        {
            //Catch manual triggers and exclude from cycles
            if (updateSource == UpdateType.Trigger) {
                manualProgram.Trigger(argument, output);
                output.Update();
                return;
            }

            //Run diagnostics every loop
            diagnostics.Update();

            if (sleepTimeLeft > 0) {
                sleepTimeLeft--;
                return;
            }

            //Loop index
            if (routineIndex >= routineCount) {
                cycle++;
                routineIndex = 0;
            }

            currentRoutine = routines[routineIndex];

            SetDisplayHeader();
            
            //Run a coroutine
            currentRoutine.Update();

            //Updates
            output.Update();
            routineIndex++;
            tick++;
        }

        public void Sleep(float seconds, bool fake = false)
        {
            if (timer == null)
                fake = true;

            if (fake) {
                sleepTimeLeft = (int)(seconds * 100);
            } else {
                sleepTimeLeft = 0;
                timer.SetValueFloat("TriggerDelay", seconds);
                timer.StartCountdown();
                Pause();
            }
        }

        public void Pause()
        {
            if (Runtime.UpdateFrequency != UpdateFrequency.None) {
                    frequencyAtPause = Runtime.UpdateFrequency;
            }

            Runtime.UpdateFrequency = UpdateFrequency.None;
        }

        public void Continue()
        {
            Runtime.UpdateFrequency = frequencyAtPause;
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
            if (output != null)
                output.SetHeader("Main Thread\n" +
                    "Cycle " + cycle + "\n" +
                    "Routine '" + currentRoutine.Name + "'"
                    );
        }
        #endregion
    }
}