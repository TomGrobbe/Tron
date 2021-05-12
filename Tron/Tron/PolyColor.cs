using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Tron
{
    internal class PolyColor
    {
        public int red;
        public int green;
        public int blue;

        internal PolyColor(int red, int green, int blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <summary>
        /// Returns a new PolyColor class by detecting the vehicle color combination. Vehicle must be a shotaro without custom paint mods.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        internal static PolyColor FromVehicle(Vehicle vehicle)
        {
            List<int[]> _color_presets = new List<int[]>
            {
                new int[3] {255, 25, 25},
                new int[3] {25, 255, 247},
                new int[3] {171, 255, 25},
                new int[3] {255, 159, 25},
                new int[3] {255, 255, 255},
            };

            if (vehicle == null || (uint)vehicle.Model.Hash != Main.SHOTARO_HASH)
            {
                return null;
            }
            int index = MathUtil.Clamp(GetVehicleColourCombination(vehicle.Handle), 0, 4);
            return new PolyColor(_color_presets[index][0], _color_presets[index][1], _color_presets[index][2]);
        }
    }
}
