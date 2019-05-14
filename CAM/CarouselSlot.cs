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
        public class CarouselSlot
        {
            public readonly Hangar hangar;
            public readonly int index;
            public readonly int angle;
            public IMyShipConnector connector;
            public IMyTextSurface screen;

            public CarouselSlot(Hangar hangar, int index)
            {
                this.hangar = hangar;
                this.index = index;
                this.angle = index * 45;
            }

            public void SetConnector(IMyShipConnector connector)
            {
                this.connector = connector;
            }

            public void SetScreen(IMyTextPanel panel)
            {
                screen = panel as IMyTextSurface;
                UpdateScreen();
            }

            private void UpdateScreen()
            {
                if (connector == null || screen == null) return;

                if (connector.Status == MyShipConnectorStatus.Connected) {
                    var name = connector.OtherConnector.CubeGrid.DisplayName.Replace(" ", "\n");
                    screen.WriteText(name);

                    screen.FontColor = (hangar.carouselIndex == index) ? Color.Gold : Color.Aqua;
                } else {
                    screen.WriteText("Empty");
                    screen.FontColor = (hangar.carouselIndex == index) ? Color.Orange : Color.Red;
                }
                
            }
        }
    }
}
