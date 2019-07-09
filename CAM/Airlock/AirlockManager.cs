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
        public class AirlockManager : IRoutine
        {
            //Settings
            public const bool MultiGrid = false;


            #region Varibles
            public readonly Output output;

            private string tag;
            private Program program;

            private List<IMyDoor> allDoors = new List<IMyDoor>();
            private List<IMyAirVent> allVents = new List<IMyAirVent>();
            private List<Airlock> airlocks = new List<Airlock>();
            private Dictionary<string, Airlock> airlockDictionary = new Dictionary<string, Airlock>();

            private int airlockIndex = 0;
            #endregion

            #region Properties
            public string Name { get { return "Airlock " + tag; } }
            public string Tag { get { return tag; } }
            #endregion

            #region Public Methods
            public AirlockManager (Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
                output = new Output(program, tag, false);
            }

            public void Start()
            {
                var tempDoors = new List<IMyDoor>();
                program.GridTerminalSystem.GetBlocksOfType<IMyDoor>(tempDoors, block => block.CustomName.Contains(tag));
                program.GridTerminalSystem.GetBlocksOfType<IMyAirVent>(allVents, block => block.CustomName.Contains(tag));

                foreach (var door in tempDoors) {
                    if (door.CubeGrid == program.Me.CubeGrid || MultiGrid)
                        InitDoor(door);
                }

                foreach (var vent in allVents) {
                    InitVent(vent);
                }

                output.SetHeader(Name);
            }

            public void Trigger(string[] arguments)
            {
            
            }

            public void Update()
            {
                if (airlocks.Count > 0) {
                    if (airlockIndex >= airlocks.Count)
                        airlockIndex = 0;

                    Airlock targetAirlock = airlocks[airlockIndex];
                    targetAirlock.Update();
                    airlockIndex++;
                }
                

                output.Update();
            }
            #endregion

            #region Private Variables
            private void InitDoor(IMyDoor door)
            {
                allDoors.Add(door);
                output.Print("Found door " + door.CustomName);

                string[] data = door.CustomData.Split(program.delimiterChars);
                if (data.Length >= 2) {
                    string airlockTag = data[0];
                    string inOut = data[1];

                    Airlock airlock = GetAirlock(airlockTag);
                    airlock.AddDoor(door, inOut);
                }

            }

            private void InitVent(IMyAirVent vent)
            {
                output.Print("Found airlock " + vent.CustomName);
                string[] data = vent.CustomData.Split(program.delimiterChars);
                if (data.Length >= 1) {
                    string airlockTag = data[0];

                    Airlock airlock = GetAirlock(airlockTag);
                    airlock.AddVent(vent);
                }
            }

            private Airlock GetAirlock(string airlockTag)
            {
                Airlock airlock;

                if (!airlockDictionary.TryGetValue(airlockTag, out airlock)) {
                    airlock = new Airlock(this, airlockTag);
                    airlocks.Add(airlock);
                    airlockDictionary.Add(airlockTag, airlock);
                }

                return airlock;
            }
            #endregion
        }
    }
}
