// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing.Reading
{
    public class ReadingScaling
    {
        /// <summary>
        /// Calculates the star rating of a map when applying HDFL given Lowest SR and multiplier
        /// </summary>
        public static double CalculateReadingStarRating(double starRating, double HDFLMultiplier, double ReadingRatio)
        {
            // Calculate the current value of a using the given StarRating
            double a = Math.Pow(5 * Math.Max(1.0, starRating / 0.115) - 4.0, 2.25) / 1150.0;

            // Approximate PP multiplier by scaling StarRating directly based on HDFLMultiplier and the calculated a.
            double HDFLStarRating = starRating * Math.Pow(HDFLMultiplier, 1 / 1.1);

            double newStarRating = starRating + (HDFLStarRating - starRating) * ReadingRatio;

            return newStarRating;
        }

        public static double CalculateMultiplier(double objectCount)
        {
            double ratioCount = objectCount / 3000;

            double HDFLMultiplier = ratioCount <= 1
                ? 1 + 0.2 * Math.Pow(ratioCount, 0.25)
                : 1 + 0.2 * Math.Pow(ratioCount, 0.325);

            return Math.Pow(HDFLMultiplier, 1.2);
        }

        public static double NotesToMemorize(double objectCount, double colourDifficultyStrain)
        {
            double colourStrainToRatio = -Math.Tanh(-0.004 * colourDifficultyStrain);
            return objectCount * colourStrainToRatio;
        }

        public static double CalculateReadingRatio(double readingDifficulty, bool isHDFL)
        {

            double reading_scale_smoothness = 5;
            double reading_scale_tolerence = 30;
            double reading_scale_rate_change = 0.015;


            double exponentialFactor = -reading_scale_rate_change * (readingDifficulty - reading_scale_tolerence);
            double readingRatio = Math.Pow(1 - Math.Exp(exponentialFactor), reading_scale_smoothness);

            if (isHDFL)
            {
                readingRatio = 1;
            }

            if (readingDifficulty <= reading_scale_tolerence)
            {
                readingRatio = 0;
            }

            return readingRatio;
        }

    }
}
