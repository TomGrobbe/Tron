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

        static readonly Dictionary<uint, List<int[]>> COLOR_PRESETS = new Dictionary<uint, List<int[]>>
        {
            [(uint)GetHashKey("shotaro")] = new List<int[]>() {
                new int[3] {255, 25, 25},
                new int[3] {25, 255, 247},
                new int[3] {171, 255, 25},
                new int[3] {255, 159, 25},
                new int[3] {255, 255, 255},
            },
            [(uint)GetHashKey("deathbike2")] = new List<int[]>() {
                new int[3] {255, 255, 255},
                new int[3] {25, 255, 247},
            }
        };

        internal PolyColor(int red, int green, int blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <summary>
        /// Returns a new PolyColor class by detecting the vehicle color combination.
        /// Vehicle must be a valid model and use vehicle color combinations or the vehicle's secondary color.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        internal static PolyColor FromVehicle(Vehicle vehicle)
        {
            if (vehicle == null) { return null; }
            uint model = (uint)vehicle.Model.Hash;
            if (!Utils.IsValidModel(model)) { return null; }
            int vehicleColorCombination = GetVehicleColourCombination(vehicle.Handle);
            if (vehicleColorCombination == -1)
            {
                int r, g, b;
                r = g = b = 0;
                const float brighter_amount = 3f;
                GetVehicleCustomSecondaryColour(vehicle.Handle, ref r, ref g, ref b);
                r = MathUtil.Clamp((int)(r * brighter_amount), 0, 255);
                g = MathUtil.Clamp((int)(g * brighter_amount), 0, 255);
                b = MathUtil.Clamp((int)(b * brighter_amount), 0, 255);
                return new PolyColor(r, g, b);
            }

            int max_colors = COLOR_PRESETS[model].Count() - 1;
            int index = MathUtil.Clamp(GetVehicleColourCombination(vehicle.Handle), 0, max_colors);

            return new PolyColor(
                COLOR_PRESETS[model][index][0],
                COLOR_PRESETS[model][index][1],
                COLOR_PRESETS[model][index][2]
            );
        }
    }
}
