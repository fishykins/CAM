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
        public class WelderArm : IPrinterPart
        {
            #region Variables
            private const float maxVelocity = 2f;
            private const float minVelocity = 0.01f;
            private const float accuracyRange = 0.01f;

            public readonly Factory factory;
            public readonly string name;
            public List<IMyPistonBase> xPistons = new List<IMyPistonBase>();
            public List<IMyPistonBase> yPistons = new List<IMyPistonBase>();
            public List<IMyShipWelder> welders = new List<IMyShipWelder>();

            public Vector2 targetPosition = new Vector2(-1f,-1f);
            public Vector2 position;
            private bool isMoving = false;
            private bool isWelding = false;
            private uint weldStartTime = 0;
            private float weldDuration = 0;
            #endregion

            #region Properties
            public string Name { get { return name; } }
            public bool IsWorking { get { return (isMoving || isWelding); } }
            #endregion

            #region Public Methods
            public WelderArm(Factory factory, string name)
            {
                this.factory = factory;
                this.name = name;
            }

            public void Action(string command, double[] values)
            {
                if (targetPosition == new Vector2(-1f, -1f))
                    targetPosition = position;

                switch (command) {
                    case "set":
                        SetTargetPosition(values);
                        break;
                    case "add":
                        MoveTargetPosition(values);
                        break;
                    case "print":
                        factory.output.Print(name + ": " + position);
                        break;
                    case "weld":
                        Weld(values);
                        break;
                    default:
                        break;
                }
            }

            public void Update()
            {
                CalculatePosition();

                if (isWelding) {
                    if (factory.program.Tick - weldStartTime >= weldDuration) {
                        isWelding = false;
                        foreach (var welder in welders) {
                            welder.Enabled = false;
                        }
                    }
                }

                if ((targetPosition - position).LengthSquared() > accuracyRange && isMoving) {

                    //Sort X
                    float xVel = (targetPosition.X - position.X) / xPistons.Count;

                    foreach (var item in xPistons) {
                        item.Velocity = xVel;
                    }

                    //Sort Y
                    float yVel = (targetPosition.Y - position.Y) / yPistons.Count;

                    foreach (var item in yPistons) {
                        item.Velocity = yVel;
                    }
                } else if (isMoving) {
                    isMoving = false;

                    foreach (var item in yPistons) {
                        item.Velocity = 0;
                    }

                    foreach (var item in xPistons) {
                        item.Velocity = 0;
                    }
                }
            }

            public void AddPiston(IMyPistonBase item, string direction)
            {
                if (direction == "x") {
                    xPistons.Add(item);
                }
                else if (direction == "y") {
                    yPistons.Add(item);
                }

                CalculatePosition();
            }

            #endregion

            #region Private Methods
            private void MoveTargetPosition(double[] values)
            {
                if (values.Length < 2) return;
                if (targetPosition == new Vector2(-1f, -1f))
                    targetPosition = position;

                targetPosition += new Vector2((float)values[0], (float)values[1]);
                SanitizeVectors();
                isMoving = true;
                //factory.output.Print(name + " moving to " + targetPosition);
            }

            private void SetTargetPosition(double[] values)
            {
                if (values.Length < 2) return;
                targetPosition = new Vector2((float)values[0], (float)values[1]);
                isMoving = true;
                SanitizeVectors();

            }

            private void Weld(double[] values)
            {
                if (values.Length < 1) return;

                isWelding = true;
                weldStartTime = factory.program.Tick;
                weldDuration = (float)values[0] * Program.TicksPerSecond;
                foreach (var welder in welders) {
                    welder.Enabled = true;
                }
            }

            private void SanitizeVectors()
            {
                if (targetPosition.X > xPistons.Count * 10) targetPosition.X = xPistons.Count * 10;
                if (targetPosition.Y > yPistons.Count * 10) targetPosition.Y = yPistons.Count * 10;
                if (targetPosition.Y < 0) targetPosition.Y = 0;
                if (targetPosition.X < 0) targetPosition.X = 0;
            }

            private void CalculatePosition()
            {
                //Change this- its cancer
                float x = 0f;
                float y = 0f;

                foreach (var item in xPistons) {
                    x += item.CurrentPosition;
                }

                foreach (var item in yPistons) {
                    y += item.CurrentPosition;
                }

                position = new Vector2(x, y);
            }
            #endregion
        }
    }
}
