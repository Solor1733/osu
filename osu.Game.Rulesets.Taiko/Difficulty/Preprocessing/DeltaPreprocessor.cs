using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing
{
    public static class DeltaPreprocessor
    {
        public static double CalculateCoefficientOfVariation(IEnumerable<double>? values)
        {
            if (values == null || !values.Any())
                return 0.0;

            double mean = values.Average();
            if (mean == 0.0)
                return 0.0;

            double variance = CalculateVariance(values, mean);
            double standardDeviation = Math.Sqrt(variance);

            return standardDeviation / mean;
        }

        public static double CalculateVariance(IEnumerable<double> values, double mean)
        {
            if (!values.Any())
                return 0.0;

            return values.Select(v => Math.Pow(v - mean, 2)).Average();
        }

        public static double CalculateOptimalBPM(IEnumerable<double> deltaTimes, double minProportion = 0.2, double maxProportion = 0.7, double maxCV = 2)
        {
            // Convert IEnumerable into list for calculation
            var deltaTimeList = deltaTimes.ToList();

            // Calculate CV
            double mean = deltaTimes.Average();
            double standardDeviation = Math.Sqrt(CalculateVariance(deltaTimes, mean));
            double cv = CalculateCoefficientOfVariation(deltaTimes);

            // Determine the proportion (How much of the map should be considered)
            double proportion = minProportion + (maxProportion - minProportion) * (1 - Math.Min(cv / maxCV, 1));

            // Convert proportion to note count
            int countToConsider = (int)Math.Ceiling(deltaTimeList.Count * proportion);

            // Sort deltatimes in ascending order, take only the densest ones
            var densestDeltas = deltaTimeList.OrderBy(x => x).Take(countToConsider);

            double weightStrength;
            const double cv_high_threshold = 1;
            const double cv_low_threshold = 0.8;

            if (cv >= cv_high_threshold || cv <= cv_low_threshold)
            {
                weightStrength = 1;
            }
            else
            {
                // Normalize CV value between 0 and 1
                double t = (cv - cv_low_threshold) / (cv_high_threshold - cv_low_threshold);
                // Apply Gaussian function to generate weight strength
                weightStrength = 1 + (3 - 1) * Math.Exp(-Math.Pow(t - (cv_high_threshold - cv_low_threshold)/2, 2) / (2 * Math.Pow(standardDeviation, 2)));
            }

            // Calculate weighted sum using inverse of deltaTime and the adjustable strength
            double weightedSum = densestDeltas.Sum(x => Math.Pow(1.0 / x, weightStrength));
            double weightedAverage = densestDeltas.Sum(x => x * Math.Pow(1.0 / x, weightStrength)) / weightedSum;

            // Cap minimal BPM at 60
            double optimalBPM = 15000 / weightedAverage;

            if (optimalBPM < 60)
            {
                optimalBPM = 60;
            }

            return optimalBPM;
        }
    }
}
