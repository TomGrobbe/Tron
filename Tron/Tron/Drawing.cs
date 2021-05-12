using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Tron
{
    internal static class Drawing
    {
        /// <summary>
        /// A helper function to easily draw the required sprite poly given the first, second and third positions and some hardcoded offset values.
        /// For UVW mapping (u,v,w parameters): https://en.wikipedia.org/wiki/UVW_mapping
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="u1"></param>
        /// <param name="v1"></param>
        /// <param name="w1"></param>
        /// <param name="u2"></param>
        /// <param name="v2"></param>
        /// <param name="w2"></param>
        /// <param name="u3"></param>
        /// <param name="v3"></param>
        /// <param name="w3"></param>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        private static void DrawPoly(Vector3 first, Vector3 second, Vector3 third, float u1, float v1, float w1, float u2, float v2, float w2, float u3, float v3, float w3, PolyColor color, int alpha)
        {
            DrawSpritePoly(
                first.X,
                first.Y,
                first.Z,
                second.X,
                second.Y,
                second.Z,
                third.X,
                third.Y,
                third.Z,
                color.red,
                color.green,
                color.blue,
                alpha,
                Utils.TEXTURE_DICT,
                Utils.TEXTURE_NAME,
                u1,
                v1,
                w1,
                u2,
                v2,
                w2,
                u3,
                v3,
                w3
            );
        }

        /// <summary>
        /// Draw all deadline trail effects.
        /// </summary>
        /// <param name="bike"></param>
        /// <returns></returns>
        internal static async Task DrawDeadlineEffects(Vehicle bike)
        {
            await Utils.LoadAssetsAndStartMusic();

            int boneIndex = GetEntityBoneIndexByName(bike.Handle, Main.BONE_NAME);
            Vector3[] positions = new Vector3[Main.MAX_LINE_POSITIONS];
            Vector2[] rotationOffsets = new Vector2[Main.MAX_LINE_POSITIONS];
            Vector3 startingPosition = GetWorldPositionOfEntityBone(bike.Handle, boneIndex);

            // Pre fill the positions and rotation offsets with blank data because we're going to loop over
            // the arrays shortly after.
            for (int i = 0; i < Main.MAX_LINE_POSITIONS - 1; i++)
            {
                positions[i] = startingPosition;
                rotationOffsets[i] = Vector2.Zero;
            }

            int timer = GetGameTimer();

            // Keep drawing while the player is riding the bike and the bike exists and it's alive.
            while (bike != null && bike.Exists() && !bike.IsDead && Game.PlayerPed.IsInVehicle(bike))
            {
                // Get the color of the bike. This determines the color of the trail later on.
                PolyColor color = PolyColor.FromVehicle(bike);

                // This can get very blinding when you set the time to be night time and you bump your PostFX graphics settings to >= HIGH.
                if (!AnimpostfxIsRunning(Utils.SCREEN_EFFECT))
                {
                    AnimpostfxPlay(Utils.SCREEN_EFFECT, 0, true);
                }

                // Get the bone world position of the rear wheel of the bike.
                Vector3 bonePosition = GetWorldPositionOfEntityBone(bike.Handle, boneIndex);

                // Limit position changes to 30 times per second to keep the trail duration consistent between clients all FPS values of 30 or above.
                // People with <30 FPS will have a longer lasting trail, there is not really a way to prevent this easilly.
                if (GetGameTimer() - timer > Main.FPS)
                {
                    // Shift all items in the arrays 1 place to the rear, discard the latest item.
                    Vector3[] oldPositions = positions;
                    Array.Copy(oldPositions, 0, positions, 1, Main.MAX_LINE_POSITIONS - 1);
                    Vector2[] oldOffsets = rotationOffsets;
                    Array.Copy(oldOffsets, 0, rotationOffsets, 1, Main.MAX_LINE_POSITIONS - 1);

                    timer = GetGameTimer();
                }

                // Calculate the leaning offset position of the bike in world coordinates, then get the difference of X and Y to calculate the relative offsets.
                // This is later used to calculate how much we need to shift the top and bottom parts of the trail in 3D space.
                Vector3 rotationOffset = GetOffsetFromEntityInWorldCoords(bike.Handle, MathUtil.DegreesToRadians(bike.Rotation.Y) * Main.LEAN_OFFSET_REDUCER, 0f, 0f);
                float xOff = rotationOffset.X - bike.Position.X;
                float yOff = rotationOffset.Y - bike.Position.Y;

                // Fill the first index with the new values.
                // This happens every frame, regardless of FPS lock to keep the trail connection between the starting point (the rear wheel)
                // to the rest of the trail as smooth as possible.
                positions[0] = bonePosition;
                rotationOffsets[0] = new Vector2(xOff, yOff);

                // Loop for the entire trail length.
                for (int i = 0; i < Main.MAX_LINE_POSITIONS - 2; i++)
                {
                    Vector3 pos1 = positions[i];
                    Vector3 pos2 = positions[i + 1];
                    Vector2 offset1 = rotationOffsets[i];
                    Vector2 offset2 = rotationOffsets[i + 1];

                    // Calculate all positions with the proper offsets. We need to draw 4 triangles to make the proper shape.
                    // One triangle is only visible from one side, so we need to draw it front to back and back to front to show it on both sides.
                    // We do that for both the top and the bottom parts, so 4 in total.
                    // We use the 'bike leaning' offsets which we calculated earlier to determine the absolute x/y offsets needed here. Inverting the values
                    // for the bottom triangles to make them angle towards the opposite side.
                    Vector3 topLeft = pos1 + new Vector3(offset1.X, offset1.Y, 0.3f);
                    Vector3 topRight = pos2 + new Vector3(offset2.X, offset2.Y, 0.3f);
                    Vector3 bottomLeft = pos1 + new Vector3(offset1.X * -1f, offset1.Y * -1f, -0.3f);
                    Vector3 bottomRight = pos2 + new Vector3(offset2.X * -1f, offset2.Y * -1f, -0.3f);

                    // Make the final (ALPHA_FADE_THRESHOLD)% of the trail slowly fade towards 0 alpha.
                    int alpha = Main.ALPHA;
                    if (i > Main.ALPHA_FADE_THRESHOLD)
                    {
                        // Remap the range so it fades out nicely.
                        alpha -= MathExtras.Map(i, Main.ALPHA_FADE_THRESHOLD, Main.MAX_LINE_POSITIONS, 0, Main.ALPHA);
                    }

                    // Draw each triangle with some hardcoded UVW offsets (taken from Rockstar scripts).
                    DrawPoly(topRight, bottomLeft, bottomRight, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1f, color, alpha); /* bottom left side */
                    DrawPoly(bottomRight, bottomLeft, topRight, 0f, 0f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, color, alpha); /* bottom right side */
                    DrawPoly(topRight, topLeft, bottomLeft, 0f, 0f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, color, alpha); /* top left side */
                    DrawPoly(topLeft, topRight, bottomLeft, 0f, 0f, 1f, 1f, 0f, 1f, 1f, 1f, 1f, color, alpha); /* top right side */
                }
                await BaseScript.Delay(0);
            }

            Utils.UnloadAssetsAndStopMusic();
        }
    }
}
