﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Scoring;
using osu.Game.Utils;

namespace osu.Game.Rulesets.Taiko.Difficulty
{
    public class TaikoPerformanceCalculator : PerformanceCalculator
    {
        private int countGreat;
        private int countOk;
        private int countMeh;
        private int countMiss;
        private double? estimatedUnstableRate;

        private double effectiveMissCount;
        //private double HDFLMultiplier = 1;

        public TaikoPerformanceCalculator()
            : base(new TaikoRuleset())
        {
        }

        protected override PerformanceAttributes CreatePerformanceAttributes(ScoreInfo score, DifficultyAttributes attributes)
        {
            var taikoAttributes = (TaikoDifficultyAttributes)attributes;

            countGreat = score.Statistics.GetValueOrDefault(HitResult.Great);
            countOk = score.Statistics.GetValueOrDefault(HitResult.Ok);
            countMeh = score.Statistics.GetValueOrDefault(HitResult.Meh);
            countMiss = score.Statistics.GetValueOrDefault(HitResult.Miss);
            estimatedUnstableRate = computeDeviationUpperBound(taikoAttributes) * 10;

            // The effectiveMissCount is calculated by gaining a ratio for totalSuccessfulHits and increasing the miss penalty for shorter object counts lower than 1000.
            if (totalSuccessfulHits > 0)
                effectiveMissCount = Math.Max(1.0, 1000.0 / totalSuccessfulHits) * countMiss;

            // Converts are detected and omitted from mod-specific bonuses due to the scope of current difficulty calculation.
            bool isConvert = score.BeatmapInfo!.Ruleset.OnlineID != 1;

            double multiplier = 1.13;

            /*if (score.Mods.Any(m => m is ModHidden) && !isConvert)
                multiplier *= 1.075;*/

            if (score.Mods.Any(m => m is ModEasy))
                multiplier *= 0.950;

            /*if(!score.Mods.Any(m => m is ModFlashlight<TaikoHitObject>) || !score.Mods.Any(m => m is ModHidden))
            {
                HDFLMultiplier = 1;
            }*/

            double difficultyValue = computeDifficultyValue(score, taikoAttributes);
            double accuracyValue = computeAccuracyValue(score, taikoAttributes, isConvert);
            double totalValue =
                Math.Pow(
                    Math.Pow(difficultyValue, 1.1) +
                    Math.Pow(accuracyValue, 1.1), 1.0 / 1.1
                ) * multiplier;

            return new TaikoPerformanceAttributes
            {
                Difficulty = difficultyValue,
                Accuracy = accuracyValue,
                EffectiveMissCount = effectiveMissCount,
                EstimatedUnstableRate = estimatedUnstableRate,
                Total = totalValue
            };
        }

        private double computeDifficultyValue(ScoreInfo score, TaikoDifficultyAttributes attributes)
        {
            double difficultyValue = Math.Pow(5 * Math.Max(1.0, attributes.StarRating / 0.115) - 4.0, 2.25) / 1150.0;

            double lengthBonus = 1 + 0.1 * Math.Min(1.0, (double)totalHits / 1500.0);
            /*double HDFLlengthBonus;

            if(totalHits <= 3000)
            {
                HDFLlengthBonus = 1 + 0.2 * (Math.Pow(((double)totalHits / 3000), 0.25));
            }
            else
            {
                HDFLlengthBonus = 1 + 0.2 * (Math.Sqrt((double)totalHits / 3000));
            }*/

            difficultyValue *= lengthBonus;

            difficultyValue *= Math.Pow(0.986, effectiveMissCount);

            if (score.Mods.Any(m => m is ModEasy))
                difficultyValue *= 0.90;

            /*if (score.Mods.Any(m => m is ModFlashlight<TaikoHitObject>) && score.Mods.Any(m => m is ModHidden))
            {
                HDFLMultiplier = Math.Pow(Math.Max(1, HDFLlengthBonus), 1.2);
            }
            else if (score.Mods.Any(m => m is ModHidden))
                difficultyValue *= 1.025;
            else if (score.Mods.Any(m => m is ModFlashlight<TaikoHitObject>))
                difficultyValue *= Math.Max(1, 1.050 - Math.Min(attributes.MonoStaminaFactor / 50, 1) * HDFLlengthBonus);*/

            if (estimatedUnstableRate == null)
                return 0;

            // Scale accuracy more harshly on nearly-completely mono (single coloured) speed maps.
            double accScalingExponent = 2 + attributes.MonoStaminaFactor;
            double accScalingShift = 400 - 100 * attributes.MonoStaminaFactor;

            return difficultyValue * Math.Pow(SpecialFunctions.Erf(accScalingShift / (Math.Sqrt(2) * estimatedUnstableRate.Value)), accScalingExponent);
        }

        private double computeAccuracyValue(ScoreInfo score, TaikoDifficultyAttributes attributes, bool isConvert)
        {
            if (attributes.GreatHitWindow <= 0 || estimatedUnstableRate == null)
                return 0;

            double accuracyValue = Math.Pow(70 / estimatedUnstableRate.Value, 1.1) * Math.Pow(attributes.StarRating, 0.4) * 100.0;

            double lengthBonus = Math.Min(1.15, Math.Pow(totalHits / 1500.0, 0.3));

            // Slight HDFL Bonus for accuracy. A clamp is used to prevent against negative values.
            if (score.Mods.Any(m => m is ModFlashlight<TaikoHitObject>) && score.Mods.Any(m => m is ModHidden) && !isConvert)
                accuracyValue *= Math.Max(1.0, 1.05 * lengthBonus);

            return accuracyValue;
        }

        /// <summary>
        /// Computes an upper bound on the player's tap deviation based on the OD, number of circles and sliders,
        /// and the hit judgements, assuming the player's mean hit error is 0. The estimation is consistent in that
        /// two SS scores on the same map with the same settings will always return the same deviation.
        /// </summary>
        private double? computeDeviationUpperBound(TaikoDifficultyAttributes attributes)
        {
            if (totalSuccessfulHits == 0 || attributes.GreatHitWindow <= 0)
                return null;

            double h300 = attributes.GreatHitWindow;
            double h100 = attributes.OkHitWindow;

            const double z = 2.32634787404; // 99% critical value for the normal distribution (one-tailed).

            // The upper bound on deviation, calculated with the ratio of 300s to objects, and the great hit window.
            double? calcDeviationGreatWindow()
            {
                if (countGreat == 0) return null;

                double n = totalHits;

                // Proportion of greats hit.
                double p = countGreat / n;

                // We can be 99% confident that p is at least this value.
                double pLowerBound = (n * p + z * z / 2) / (n + z * z) - z / (n + z * z) * Math.Sqrt(n * p * (1 - p) + z * z / 4);

                // We can be 99% confident that the deviation is not higher than:
                return h300 / (Math.Sqrt(2) * SpecialFunctions.ErfInv(pLowerBound));
            }

            // The upper bound on deviation, calculated with the ratio of 300s + 100s to objects, and the good hit window.
            // This will return a lower value than the first method when the number of 100s is high, but the miss count is low.
            double? calcDeviationGoodWindow()
            {
                if (totalSuccessfulHits == 0) return null;

                double n = totalHits;

                // Proportion of greats + goods hit.
                double p = Math.Max(0, totalSuccessfulHits - 0.0005 * countOk) / n;

                // We can be 99% confident that p is at least this value.
                double pLowerBound = (n * p + z * z / 2) / (n + z * z) - z / (n + z * z) * Math.Sqrt(n * p * (1 - p) + z * z / 4);

                // We can be 99% confident that the deviation is not higher than:
                return h100 / (Math.Sqrt(2) * SpecialFunctions.ErfInv(pLowerBound));
            }

            double? deviationGreatWindow = calcDeviationGreatWindow();
            double? deviationGoodWindow = calcDeviationGoodWindow();

            if (deviationGreatWindow is null)
                return deviationGoodWindow;

            return Math.Min(deviationGreatWindow.Value, deviationGoodWindow!.Value);
        }

        private int totalHits => countGreat + countOk + countMeh + countMiss;

        private int totalSuccessfulHits => countGreat + countOk + countMeh;
    }
}
