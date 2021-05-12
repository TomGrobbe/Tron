using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Tron
{
    public class Main : BaseScript
    {
        // If you're planning on editing this script, I'd recommend you start by playing around with the 
        // following 3 constants before touching anything else in the code.
        internal const int ALPHA = 175;
        internal const int MAX_LINE_POSITIONS = 180;
        internal const int ALPHA_FADE_THRESHOLD = (int)(MAX_LINE_POSITIONS * 0.90);

        // I do not recommend changing any of these constants, by doing that you risk breaking the core functionality.
        internal static readonly uint SHOTARO_HASH = (uint)GetHashKey("SHOTARO");
        internal const string BONE_NAME = "wheel_lr";
        internal const float LEAN_OFFSET_REDUCER = 0.4f;
        internal const int FPS = 1000 / 30;


        public Main()
        {
#if DEBUG
            Debug.WriteLine($"[{GetCurrentResourceName()}] Loaded resource.");
#endif
        }

        /// <summary>
        /// Checks if the current player vehicle is a shotaro and then triggers the DrawDeadLineEffects method for the actual drawing.
        /// </summary>
        /// <returns></returns>
        [Tick]
        internal async Task VehicleChecker()
        {
            if (!Game.PlayerPed.IsInVehicle())
            {
                await Task.FromResult(0);
                return;
            }

            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null || (uint)vehicle.Model.Hash != SHOTARO_HASH)
            {
                await Task.FromResult(0);
                return;
            }

            vehicle.IsRadioEnabled = false;

            await Drawing.DrawDeadlineEffects(vehicle);

            await Delay(10);
        }
    }
}
