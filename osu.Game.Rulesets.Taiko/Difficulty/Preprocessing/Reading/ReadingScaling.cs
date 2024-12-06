// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing.Reading
{
    public class ReadingScaling
    {
        /// <summary>
        /// Calculates the star rating of a map when applying HDFL given Lowest SR and multiplier
        /// </summary>
        public static double CalculateHDFLStarRating(double starRating, double n)
        {
            // Calculate the current value of a using the given StarRating
            double a = Math.Pow(5 * Math.Max(1.0, starRating / 0.115) - 4.0, 2.25) / 1150.0;

            // Approximate PP multiplier by scaling StarRating directly based on n and the calculated a.
            double newStarRating = starRating * Math.Pow(n, 1 / 1.1);

            return newStarRating;
        }

        public static double CalculateHDFLMultiplier(double objectCount)
        {
            double HDFLMultiplier;
            if(objectCount <= 3000)
            {
                HDFLMultiplier = 1 + 0.2 * (Math.Pow((objectCount / 3000), 0.25));
            }
            else
            {
                HDFLMultiplier = 1 + 0.2 * (Math.Sqrt(objectCount / 3000));
            }

            return HDFLMultiplier;
        }
    }
}
