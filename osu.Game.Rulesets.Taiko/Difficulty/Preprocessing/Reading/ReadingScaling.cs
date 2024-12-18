// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Taiko.Mods;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing.Reading
{
    public class ReadingScaling
    {
        /// <summary>
        /// Calculates the star rating of a map when applying HDFL given Lowest SR and multiplier
        /// </summary>
        public static double CalculateReadingStarRating(double starRating, double hdflMultiplier, double readingRatio)
        {
            // Approximate PP multiplier by scaling StarRating directly based on HDFLMultiplier and the calculated a.

            double hdflStarRating = starRating * hdflMultiplier;

            double newStarRating = starRating + (hdflStarRating - starRating) * readingRatio;

            return newStarRating;
        }

        /// <summary>
        /// Calculate the HDFL multipler for SR given how many notes need to be memorized
        /// </summary>
        public static double CalculateMultiplier(double objectCount)
        {
            double ratioCount = objectCount / 3000;

            double hdflMultiplier = ratioCount <= 1
                ? 1 + 0.2 * Math.Pow(ratioCount, 0.25)
                : 1 + 0.2 * Math.Pow(ratioCount, 0.325);

            return Math.Pow(hdflMultiplier, 1.2 / 1.1);
        }

        /// <summary>
        /// Calculates how many notes need to be memorized for a map to memorize the entire map given the original object count and colour diffivulty
        /// </summary>
        public static double NotesToMemorize(double objectCount, double colourDifficultyStrain)
        {
            double colourStrainToRatio = -Math.Tanh(-0.004 * colourDifficultyStrain);
            return objectCount * colourStrainToRatio;
        }

        /// <summary>
        /// Calculates the reading ratio of a map given a reading difficulty and if HDFL is on
        /// </summary>
        public static double CalculateReadingRatio(double readingDifficulty, double objectCount, double HDFLMultiplier, Mod[] mods)
        {
            // If HDFL is on, return readingRatio = 1
            if (mods.Any(m => m is TaikoModFlashlight) && mods.Any(m => m is TaikoModHidden))
            {
                return 1;
            }

            // Please comment this
            double memoryLengthBonus = objectCount <= 3000 ? 0.25 : 0.325;

            //Calculate Exponent for reading ratio
            double exponent = Math.Max((Math.Log(0.25) + (12 * memoryLengthBonus / 11) * Math.Log(3000 / objectCount)) / Math.Log(0.35), 0.6);

            // Calculate reading ratio
            double readingRatio = Math.Pow(CalculateReadingDifficulty(mods), exponent);

            return readingRatio;
        }

        //TESTING VALUES!!! WIP
        public static double CalculateReadingDifficulty(/*IEnumerable<double> NotesEffectiveBPM,*/Mod[] mods)
        {
            if (mods.Any(m => m is TaikoModFlashlight) && mods.Any(m => m is TaikoModHidden))
            {
                return 1;
            }

            if (mods.Any(m => m is TaikoModHidden) && mods.Any(m => m is TaikoModHardRock))
            {
                return 0.6;
            }

            if (mods.Any(m => m is TaikoModHidden) && mods.Any(m => m is TaikoModEasy))
            {
                return 0.2;
            }

            if (mods.Any(m => m is TaikoModHidden))
            {
                return 0.35;
            }

            return 0;
        }
    }
}
