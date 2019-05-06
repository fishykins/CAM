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
        public class GridAnalyser
        {
            #region Variables
            public readonly Program program;
            #endregion

            #region properties

            #endregion

            #region Public Methods
            public GridAnalyser(Program program)
            {
                this.program = program;
            }

            /// <summary>
            /// Finds all LCD screens with the given tag
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public List<IMyTextSurface> FindScreens(string tag)
            {
                List<IMyTextPanel> tempList = new List<IMyTextPanel>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(tempList, screen => screen.CustomName.Contains(tag));

                List<IMyTextSurface> surfaces = new List<IMyTextSurface>();
                foreach (var item in tempList) {
                    surfaces.Add(item as IMyTextSurface);
                }

                return surfaces;
            }

            public IMyTimerBlock GetTimer(string tag)
            {
                List<IMyTimerBlock> tempList = new List<IMyTimerBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(tempList, block => block.CustomName.Contains(tag));

                if (tempList.Count > 0) {
                    return tempList[0];
                }

                return null;
            }

            #endregion

            #region Private Methods

            #endregion
        }
    }
}
