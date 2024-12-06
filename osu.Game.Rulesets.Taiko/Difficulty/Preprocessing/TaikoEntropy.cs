using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing
{
    public static class EntropyCalculator
    {
        public static double CalculateEntropy(IEnumerable<double>? values, double binSize)
        {
            if (values == null || !values.Any())
                return 0.0;

            var binnedValues = values
                               .Select(v => Math.Round(v / binSize) * binSize)
                               .ToList();

            var groups = binnedValues
                         .GroupBy(v => v)
                         .Select(g => new { Value = g.Key, Count = g.Count() })
                         .ToList();

            int total = binnedValues.Count;
            double entropy = 0.0;

            foreach (var group in groups)
            {
                double probability = (double)group.Count / total;
                entropy -= probability * Math.Log(probability, 2);
            }

            return entropy;
        }

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

        public static double CalculateAverage(IEnumerable<double> values)
        {
            if (!values.Any())
                return 0.0;

            return values.Average();
        }

        public static double CalculateVariance(IEnumerable<double> values, double mean)
        {
            if (!values.Any())
                return 0.0;

            return values.Select(v => Math.Pow(v - mean, 2)).Average();
        }

        public static double CalculateOptimalBPM(IEnumerable<double> deltaTimes, double minProportion = 0.3, double maxProportion = 0.6, double maxCV = 2)
        {
            // Convert IEnumerable into list for calculation
            var deltaTimeList = deltaTimes.ToList();

            // Calculate mean, standard deviation and CV
            double mean = deltaTimeList.Average();
            double standardDeviation = Math.Sqrt(deltaTimeList.Sum(x => Math.Pow(x - mean, 2)) / deltaTimeList.Count);
            double cv = standardDeviation / mean;

            // Determine the proportion (How much of the map should be considered)
            double proportion = minProportion + (maxProportion - minProportion) * (1 - Math.Min(cv / maxCV, 1));

            // Convert proportion to note count
            int countToConsider = (int)Math.Ceiling(deltaTimeList.Count * proportion);            

            // Sort deltatimes in ascending order, take only the densest ones
            var densestDeltas = deltaTimeList.OrderBy(x => x).Take(countToConsider);

            double weightStrength;
            double cvHighThreshold = 1;
            double cvLowThreshold = 0.8;

            if (cv >= cvHighThreshold || cv <= cvLowThreshold)
            {
                weightStrength =  1;
            }
            else
            {
                // Normalize CV value between 0 and 1
                double t = (cv - cvLowThreshold) / (cvHighThreshold - cvLowThreshold);
                // Apply Gaussian function to generate weight strength
                weightStrength = 1 + (3 - 1) * Math.Exp(-Math.Pow(t - (cvHighThreshold - cvLowThreshold)/2, 2) / (2 * Math.Pow(standardDeviation, 2)));
            }

            // Calculate weighted sum using inverse of deltaTime and the adjustable strength
            double weightedSum = densestDeltas.Sum(x => Math.Pow(1.0 / x, weightStrength));
            double weightedAverage = densestDeltas.Sum(x => x * Math.Pow(1.0 / x, weightStrength)) / weightedSum;

            // Cap minimal BPM at 60
            double OptimalBPM = 15000 / weightedAverage;
            if (OptimalBPM < 60)
            {
                OptimalBPM = 60;
            }

            return OptimalBPM;
        }
    }
}
