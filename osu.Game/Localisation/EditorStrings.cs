﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class EditorStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.Editor";

        /// <summary>
        /// "File"
        /// </summary>
        public static LocalisableString File => new TranslatableString(getKey(@"file"), @"File");

        /// <summary>
        /// "Edit"
        /// </summary>
        public static LocalisableString Edit => new TranslatableString(getKey(@"edit"), @"Edit");

        /// <summary>
        /// "Undo"
        /// </summary>
        public static LocalisableString Undo => new TranslatableString(getKey(@"undo"), @"Undo");

        /// <summary>
        /// "Redo"
        /// </summary>
        public static LocalisableString Redo => new TranslatableString(getKey(@"redo"), @"Redo");

        /// <summary>
        /// "Cut"
        /// </summary>
        public static LocalisableString Cut => new TranslatableString(getKey(@"cut"), @"Cut");

        /// <summary>
        /// "Copy"
        /// </summary>
        public static LocalisableString Copy => new TranslatableString(getKey(@"copy"), @"Copy");

        /// <summary>
        /// "Paste"
        /// </summary>
        public static LocalisableString Paste => new TranslatableString(getKey(@"paste"), @"Paste");

        /// <summary>
        /// "Clone"
        /// </summary>
        public static LocalisableString Clone => new TranslatableString(getKey(@"clone"), @"Clone");

        /// <summary>
        /// "View"
        /// </summary>
        public static LocalisableString View => new TranslatableString(getKey(@"view"), @"View");

        /// <summary>
        /// "Waveform opacity"
        /// </summary>
        public static LocalisableString WaveformOpacity => new TranslatableString(getKey(@"waveform_opacity"), @"Waveform opacity");

        /// <summary>
        /// "Show hit markers"
        /// </summary>
        public static LocalisableString ShowHitMarkers => new TranslatableString(getKey(@"show_hit_markers"), @"Show hit markers");

        /// <summary>
        /// "Timing"
        /// </summary>
        public static LocalisableString Timing => new TranslatableString(getKey(@"timing"), @"Timing");

        /// <summary>
        /// "Set preview point to current time"
        /// </summary>
        public static LocalisableString SetPreviewPointToCurrent => new TranslatableString(getKey(@"set_preview_point_to_current"), @"Set preview point to current time");

        /// <summary>
        /// "Export package"
        /// </summary>
        public static LocalisableString ExportPackage => new TranslatableString(getKey(@"export_package"), @"Export package");

        /// <summary>
        /// "Create new difficulty"
        /// </summary>
        public static LocalisableString CreateNewDifficulty => new TranslatableString(getKey(@"create_new_difficulty"), @"Create new difficulty");

        /// <summary>
        /// "Change difficulty"
        /// </summary>
        public static LocalisableString ChangeDifficulty => new TranslatableString(getKey(@"change_difficulty"), @"Change difficulty");

        /// <summary>
        /// "Delete difficulty"
        /// </summary>
        public static LocalisableString DeleteDifficulty => new TranslatableString(getKey(@"delete_difficulty"), @"Delete difficulty");

        /// <summary>
        /// "Exit"
        /// </summary>
        public static LocalisableString Exit => new TranslatableString(getKey(@"exit"), @"Exit");

        /// <summary>
        /// "setup"
        /// </summary>
        public static LocalisableString SetupScreen => new TranslatableString(getKey(@"setup_screen"), @"setup");

        /// <summary>
        /// "compose"
        /// </summary>
        public static LocalisableString ComposeScreen => new TranslatableString(getKey(@"compose_screen"), @"compose");

        /// <summary>
        /// "design"
        /// </summary>
        public static LocalisableString DesignScreen => new TranslatableString(getKey(@"design_screen"), @"design");

        /// <summary>
        /// "timing"
        /// </summary>
        public static LocalisableString TimingScreen => new TranslatableString(getKey(@"timing_screen"), @"timing");

        /// <summary>
        /// "verify"
        /// </summary>
        public static LocalisableString VerifyScreen => new TranslatableString(getKey(@"verify_screen"), @"verify");

        /// <summary>
        /// "Playback speed"
        /// </summary>
        public static LocalisableString PlaybackSpeed => new TranslatableString(getKey(@"playback_speed"), @"Playback speed");

        /// <summary>
        /// "Test!"
        /// </summary>
        public static LocalisableString TestBeatmap => new TranslatableString(getKey(@"test_beatmap"), @"Test!");

        /// <summary>
        /// "Waveform"
        /// </summary>
        public static LocalisableString TimelineWaveform => new TranslatableString(getKey(@"timeline_waveform"), @"Waveform");

        /// <summary>
        /// "Ticks"
        /// </summary>
        public static LocalisableString TimelineTicks => new TranslatableString(getKey(@"timeline_ticks"), @"Ticks");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
