using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Taiko.Difficulty.Preprocessing.Reading
{
    public class OptimalBPMPreprocessor
    {
        private readonly IList<TaikoDifficultyHitObject> noteObjects;
        private const double OptimalBPMDecayBase = 1.25;

        public OptimalBPMPreprocessor(List<TaikoDifficultyHitObject> noteObjects)
        {
            this.noteObjects = noteObjects;
        }

        /// <summary>
        /// Calculates Optimal BPM for all hit notes and ensure no abrupt BPM changes.
        /// </summary>
        public void ProcessOptimalBPM()
        {
            foreach (var currentNoteObject in noteObjects)
            {   
                // Calculate the Instantaneous BPM of current note, and clamp it between 60 and 450.
                double currentInstantaneousBPM = 15000 / currentNoteObject.DeltaTime;
                currentInstantaneousBPM = Math.Clamp(currentInstantaneousBPM, 60, 450);
                
                // If no previous note exist, set the optimal BPM of the current note to the Instantaneous BPM.
                if(currentNoteObject.PreviousNote(1) == null)
                {
                    currentNoteObject.OptimalBPM = currentInstantaneousBPM;
                    continue;
                }

                // The most amount optimal BPM can change from the previous note, based on delta time and decay base.
                double OptimalBPMDecayLimit = currentNoteObject.DeltaTime / 1000 * (OptimalBPMDecayBase - 1) + 1;
                currentNoteObject.OptimalBPM = Math.Clamp(currentInstantaneousBPM, 
                currentNoteObject.PreviousNote(1).OptimalBPM / OptimalBPMDecayLimit,
                currentNoteObject.PreviousNote(1).OptimalBPM * OptimalBPMDecayLimit);
            }
        }
    }
}
