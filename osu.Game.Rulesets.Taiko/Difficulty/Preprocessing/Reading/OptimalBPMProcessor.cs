using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing.Reading
{
    public static class OptimalBPMProcessor
    {
        private static double calculateCoefficientOfVariation(IEnumerable<double>? values)
        {
            if (values == null) return 0;

            double[] coefficient = values as double[] ?? values.ToArray();

            if (coefficient.Length == 0)
                return 0.0;

            double mean = coefficient.Average();
            if (mean == 0.0)
                return 0.0;

            double variance = calculateVariance(coefficient, mean);
            double standardDeviation = Math.Sqrt(variance);

            return standardDeviation / mean;
        }

        private static double calculateVariance(IEnumerable<double> values, double mean)
        {
            double[] variance = values as double[] ?? values.ToArray();

            return variance.Length == 0 ? 0.0 : variance.Select(v => Math.Pow(v - mean, 2)).Average();
        }

        public static double CalculateOptimalBPM(IEnumerable<double> deltaTimes, double minProportion = 0.2, double maxProportion = 0.7, double maxCV = 2)
        {
            // Convert IEnumerable into list for calculation
            var deltaTimeList = deltaTimes.ToList();

            // Calculate CV
            double mean = deltaTimes.Average();
            double standardDeviation = Math.Sqrt(calculateVariance(deltaTimes, mean));
            double cv = calculateCoefficientOfVariation(deltaTimes);

            // Determine the proportion (How much of the map should be considered)
            double proportion = minProportion + (maxProportion - minProportion) * (1 - Math.Min(cv / maxCV, 1));

            // Convert proportion to note count
            int countToConsider = (int)Math.Ceiling(deltaTimeList.Count * proportion);

            // Sort deltatimes in ascending order, take only the densest ones
            var densestDeltas = deltaTimeList.OrderBy(x => x).Take(countToConsider);

            double weightStrength;
            const double cv_high_threshold = 1;
            const double cv_low_threshold = 0.8;

            if (cv is >= cv_high_threshold or <= cv_low_threshold)
            {
                weightStrength = 1;
            }
            else
            {
                // Normalize CV value between 0 and 1
                double t = (cv - cv_low_threshold) / (cv_high_threshold - cv_low_threshold);
                // Apply Gaussian function to generate weight strength
                // Maps with CV in between 0.8 and 1 should have a higher weight towards hard sections
                weightStrength = 1 + (3 - 1) * Math.Exp(-Math.Pow(t - (cv_high_threshold - cv_low_threshold)/2, 2) / (2 * Math.Pow(standardDeviation, 2)));
            }

            // Calculate weighted sum using inverse of deltaTime and the adjustable strength
            double[] denseDelta = densestDeltas as double[] ?? densestDeltas.ToArray();

            double weightedSum = denseDelta.Sum(x => Math.Pow(1.0 / x, weightStrength));
            double weightedAverage = denseDelta.Sum(x => x * Math.Pow(1.0 / x, weightStrength)) / weightedSum;

            // Cap minimal BPM at 60
            double optimalBPM = 15000 / weightedAverage;

            if (optimalBPM < 60)
            {
                optimalBPM = 60;
            }

            // Cap maximum BPM at 450
            if (optimalBPM > 450)
            {
                optimalBPM = 450;
            }

            return optimalBPM;
        }
    }
}
