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
        public class ProjectorKebab : IPrinterPart
        {
            public readonly Factory factory;
            public readonly string name;
            public IMyMotorStator rotor;

            public float rotorTarget = -1;
            public float rotorAngle;

            private bool isWorking = false;

            #region Properties
            public string Name { get { return name; } }
            public bool IsWorking { get { return isWorking; } }
            #endregion

            public ProjectorKebab(Factory factory, string name)
            {
                this.factory = factory;
                this.name = name;
            }

            public void Update()
            {
                if (isWorking) {
                    rotorAngle = (rotor.Angle * 57.2958f);

                    if (rotorTarget > 360) rotorTarget -= 360;
                    if (rotorTarget < 0) rotorTarget += 360;

                    float tempTarget = rotorTarget;

                    if (Math.Abs(rotorTarget - rotorAngle) > 180) {
                        tempTarget -= 360;
                    }

                    var velocity = (tempTarget - rotorAngle) / 3f;
                    rotor.TargetVelocityRPM = velocity;

                    isWorking = (Math.Abs(velocity) > 0.01f);

                    if (!isWorking)
                        rotor.TargetVelocityRPM = 0f;
                }
            }

            public void Action(string command, double[] values)
            {
                switch (command) {
                    case "add":
                        if (values.Length > 0) {
                            rotorTarget += (float)values[0];
                            isWorking = true;
                        }
                        break;
                    case "set":
                        if (values.Length > 0) {
                            rotorTarget = (float)values[0];
                            isWorking = true;
                        }
                            
                        break;
                    default:

                        break;
                }
            }

            private void Sanitize()
            {
                if (rotorTarget > 360) rotorTarget -= 360;
                if (rotorTarget < 0) rotorTarget += 360;
            } 
        }
    }
}
