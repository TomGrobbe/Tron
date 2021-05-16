using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Tron
{
    internal static class Utils
    {
        internal const string TEXTURE_DICT = "Deadline";
        internal const string TEXTURE_NAME = "Deadline_Trail_01";
        internal const string START_MUSIC_EVENT = "BKR_DEADLINE_START_MUSIC";
        internal const string STOP_MUSIC_EVENT = "FM_SUDDEN_DEATH_STOP_MUSIC";
        internal const string SCREEN_EFFECT = "DeadlineNeon";
        private static readonly uint[] VALID_MODELS = new uint[2] { (uint)GetHashKey("deathbike2"), (uint)GetHashKey("shotaro") };

        /// <summary>
        /// Loads the required texture dict.
        /// </summary>
        /// <returns></returns>
        internal static async Task LoadAssetsAndStartMusic()
        {
            if (!HasStreamedTextureDictLoaded(TEXTURE_DICT))
            {
                RequestStreamedTextureDict(TEXTURE_DICT, true);
                while (!HasStreamedTextureDictLoaded(TEXTURE_DICT))
                {
                    RequestStreamedTextureDict(TEXTURE_DICT, true);
                    await BaseScript.Delay(0);
                }
            }
            TriggerMusicEvent(START_MUSIC_EVENT);
        }

        /// <summary>
        /// Unloads the streamed texture dict.
        /// </summary>
        internal static void UnloadAssetsAndStopMusic()
        {
            SetStreamedTextureDictAsNoLongerNeeded(TEXTURE_DICT);

            if (AnimpostfxIsRunning(SCREEN_EFFECT))
            {
                AnimpostfxStop(SCREEN_EFFECT);
            }
            TriggerMusicEvent(STOP_MUSIC_EVENT);
        }

        internal static bool IsValidModel(uint model)
        {
            return VALID_MODELS.Contains(model);
        }
    }
}
