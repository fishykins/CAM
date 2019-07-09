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

        public class Airlock
        {
            public const float calMargin = 0.1f;

            public IMyDoor inner;
            public IMyDoor outer;
            public readonly string tag;
            public AirlockManager manager;
            public List<IMyAirVent> airVents = new List<IMyAirVent>();
            public IMyAirVent primaryVent;

            private bool complete = false;

            public Airlock(AirlockManager manager, string tag)
            {
                this.tag = tag;
                this.manager = manager;
            }

            public void Update()
            {
                if (!IsReady()) return;

                float pr = primaryVent.GetOxygenLevel();

                if (pr > calMargin) {
                    ForceClose(outer);

                    if (pr < 1f - calMargin) {
                        ForceClose(inner);
                    } else {
                        inner.Enabled = true;
                    }

                } else {
                    ForceClose(inner);
                    outer.Enabled = true;
                }
            }

            public void AddDoor(IMyDoor door, string role)
            {
                switch (role.ToLower()) {
                    case "in":
                        inner = door;
                        break;
                    case "out":
                        outer = door;
                        break;
                    default:
                        break;
                }
            }


            public void AddVent(IMyAirVent vent)
            {
                airVents.Add(vent);
                if (primaryVent == null)
                    primaryVent = vent;
            }

            private bool IsReady()
            {
                if (complete) return true;

                if (inner != null && outer != null && primaryVent != null) {
                    manager.output.Print(tag + " is opperational");
                    complete = true;
                    return true;
                }

                return false;
            }

            private void ForceClose(IMyDoor door)
            {
                if (!(door.Status == DoorStatus.Closed)) {
                    if (door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening) {
                        if (!door.Enabled)
                            door.Enabled = true;
                        door.CloseDoor();
                        manager.output.Print(tag + " force closing " + door.CustomName);
                    }
                } else {
                    door.Enabled = (door.Status != DoorStatus.Closed);
                }
            }
        }
    }
}
