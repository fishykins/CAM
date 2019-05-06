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
            public readonly Program program;
            public readonly string tag;
            #endregion

            #region properties
            public string Name { get { return "Stress Test"; } }
            #endregion

            #region Public Methods
            public StressTest(Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
            }


            public void Start()
            {

            }

            public void Update()
            {

            }
            #endregion

            #region Private Methods

            #endregion
        }
    }
}
