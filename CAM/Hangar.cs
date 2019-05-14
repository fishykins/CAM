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
        //Hard coded hangar type for Sonny Station. 
        public class Hangar : IRoutine
        {
            #region Variables
            public const float rotorWaitTime = 2f;
            public const float rotorSpeed = 5f;
            public char[] delimiterChars = { ' ', '-' };

            public const string carouselTag = "carousel";

            public readonly Program program;
            public readonly Output output;
            public readonly string tag;

            private enum Direction{None, Clockwise, Counterclockwise}
            private int tick = 0;
            

            //Objects
            private List<IMyShipConnector> allConnectors = new List<IMyShipConnector>();

            //Carousel
            public int carouselIndex = 0;
            private CarouselSlot[] carouselSlots = new CarouselSlot[8];
            private IMyMotorStator vendingRotor;
            public IMyTextPanel[] carouselScreens = new IMyTextPanel[8];
            #endregion

            #region properties
            public string Name { get { return "hangar " + tag; } }
            public string Tag { get { return tag; } }
            #endregion

            #region public Variables
            public Hangar(Program program, string tag)
            {
                this.program = program;
                this.tag = tag;
                this.output = new Output(program, tag, false);
            }

            public void Start()
            {
                program.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(allConnectors, connector => connector.CustomName.Contains(tag));

                InitCarousel();
                SetRotorPosition(0);

                output.SetHeader(Name);
            }

            public void Update()
            {
                EarlyUpdate();

                float currentAngle = vendingRotor.Angle / (float)Math.PI * 180f;
                LateUpdate();
            }

            public void Trigger(string argument)
            {
                int i = 0;
                if (Int32.TryParse(argument, out i)) {
                    SetRotorPosition(i);
                } else {
                    switch (argument.ToLower()) {
                        case "carouselinc":
                            SetRotorPosition(carouselIndex + 1);
                            break;
                        case "carouseldec":
                            SetRotorPosition(carouselIndex - 1);
                            break;
                        default:
                            break;
                    }
                }


            }
            #endregion

            #region Private Methods
            private void EarlyUpdate()
            {

            }

            private void LateUpdate()
            {
                tick++;
                output.Update();
            }

            private bool IsRotor(IMyTerminalBlock block)
            {
                var cast = block as IMyMotorStator;
                return (cast != null) && block.CustomName.Contains(tag);
            }

            private void SetRotorPosition(int index)
            {
                if (index < 0) index += carouselSlots.Length;
                if (index >= carouselSlots.Length) index -= carouselSlots.Length;
                
                var currentSLot = carouselSlots[index];
                if (currentSLot == null) return;

                float speed = (GetDirection(index, carouselIndex, 8) == Direction.Clockwise) ? -rotorSpeed : rotorSpeed;
                carouselIndex = index;

                vendingRotor.SetValue<float>("UpperLimit", currentSLot.angle);
                vendingRotor.SetValue<float>("LowerLimit", currentSLot.angle);
                vendingRotor.SetValue<float>("Velocity", speed);

                foreach (var slot in carouselSlots) {
                    int screenIndex = slot.index - carouselIndex;
                    if (screenIndex < 0)
                        screenIndex += carouselScreens.Length;

                    slot.SetScreen(carouselScreens[screenIndex]);
                }

                output.Print("Moving rotor to " + currentSLot.angle + " degrees");
            }

            private static Direction GetDirection(int a, int b, int limmit)
            {
                if (a == b) {
                    return Direction.None;
                }
                return (a - b + limmit) % limmit > (limmit/2) ? Direction.Clockwise : Direction.Counterclockwise;
            }

            /// <summary>
            /// Carousel
            /// </summary>
            private void InitCarousel()
            {
                for (int i = 0; i < 8; i++) {
                    carouselSlots[i] = new CarouselSlot(this, i);
                }


                var rotors = new List<IMyTerminalBlock>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(rotors, IsRotor);

                //Parse Connectors
                foreach (var item in allConnectors) {
                    if (item.CustomName.ToLower().Contains(carouselTag)) {
                        //Find out what index
                        string[] splits = item.CustomName.ToLower().Split(delimiterChars);

                        if (splits.Length >= 4) {
                            int i;
                            if (Int32.TryParse(splits[3], out i)) {
                                carouselSlots[i].SetConnector(item);

                                if (item.Status == MyShipConnectorStatus.Connected) {
                                    output.Print(item.OtherConnector.CubeGrid.DisplayName + " is connected to slot " + i);
                                } else {
                                    output.Print(item.CustomName + " status: " + item.Status);
                                }
                            }
                        }
                    }
                }

                //Parse Screens
                var tempScreens = new List<IMyTextPanel>();
                program.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(tempScreens, screen => screen.CustomName.Contains(tag));

                foreach (var item in tempScreens) {
                    if (item.CustomName.ToLower().Contains(carouselTag)) {
                        //This is a screen for displaying a carousel slot- parse
                        string[] splits = item.CustomName.ToLower().Split(delimiterChars);

                        if (splits.Length >= 4) {
                            int i;
                            if (Int32.TryParse(splits[3], out i)) {
                                carouselSlots[i].SetScreen(item);
                                carouselScreens[i] = item;
                                output.Print("Display " + item.CustomName + " has been allocated");
                            }
                        }
                    }
                }

                if (rotors.Count > 0) {
                    vendingRotor = rotors[0] as IMyMotorStator;
                    vendingRotor.SetValue<float>("UpperLimit", 180);
                    vendingRotor.SetValue<float>("LowerLimit", 0);
                    vendingRotor.SetValue<float>("Velocity", rotorSpeed);

                    output.Print("Found vending rotor: " + vendingRotor.CustomName);
                }
                else {
                    output.Print("did not find a rotor :(");
                }
            }
            #endregion
        }
    }
}
