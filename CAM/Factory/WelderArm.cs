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
            public List<IMyShipWelder> welder = new List<IMyShipWelder>();

            public Vector2 targetPosition = new Vector2(-1f,-1f);
            public Vector2 position;
            private bool isMoving = false;
            #endregion

            #region Properties

            #endregion

            #region Public Methods
            public WelderArm(Factory factory, string name)
            {
                this.factory = factory;
                this.name = name;
            }

            public void Update()
            {
                CalculatePosition();

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
            }

            public void ManualPosition(string stringA, string stringB)
            {
                if (targetPosition == new Vector2(-1f, -1f))
                    targetPosition = position;

                Double x;
                Double y;

                if (Double.TryParse(stringA, out x)) {
                    //Coorinate
                    if (Double.TryParse(stringB, out y)) {

                        targetPosition = new Vector2((float)x, (float)y);

                        isMoving = true;

                        factory.output.Print(name + " is moving to " + targetPosition);
                    }
                }
                else {
                    //Additive
                    double a;
                    if (Double.TryParse(stringB, out a)) {
                        if (stringA.ToLower() == "x") {
                            targetPosition.X += (float)a;
                        }
                        else if (stringA.ToLower() == "y") {
                            targetPosition.Y += (float)a;
                        }

                        isMoving = true;
                    }
                }

                if (targetPosition.X > xPistons.Count * 10) targetPosition.X = xPistons.Count * 10;
                if (targetPosition.Y > yPistons.Count * 10) targetPosition.Y = yPistons.Count * 10;
            }

            public void PrintObjects(Output output)
            {
                foreach (var item in xPistons) {
                    output.Print(name + ": X piston " + item.CustomName);
                }

                foreach (var item in yPistons) {
                    output.Print(name + ": Y piston " + item.CustomName);
                }
            }
            #endregion

            #region Private Methods
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
