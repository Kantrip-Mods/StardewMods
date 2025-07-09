// Copyright (C) 2025 Kantrip
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SweetTokens
{
    public interface IContentPatcherAPI
    {
        // Basic api
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);

        // Advanced api
        void RegisterToken(IManifest mod, string name, object token);
    }

    internal class Globals
    {
        public static IManifest Manifest { get; set; }
        public static IModHelper Helper { get; set; }
        public static IMonitor Monitor { get; set; }
        public static ModConfig Config { get; set; }
	}

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        internal ModConfig config;
        internal static IModHelper help = null!;
        public static IContentPatcherAPI api = null;
        internal static SuitorsToken SuitorsToken { get; private set; } = new SuitorsToken();
        internal static MaxHeartSuitorsToken MaxHeartSuitorsToken { get; private set; } = new MaxHeartSuitorsToken();
        internal static RivalSuitorsToken RivalSuitorsToken { get; private set; } = new RivalSuitorsToken();
        internal static PartnerToken PartnerToken { get; private set; } = new PartnerToken();
        internal static FianceeToken FianceeToken { get; private set; } = new FianceeToken();
        internal static BlackHeartSuitorToken BlackHeartSuitorToken { get; private set; } = new BlackHeartSuitorToken();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            help = helper;

            this.config = this.Helper.ReadConfig<ModConfig>();

            SetUpGlobals(helper);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            //helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Access Content Patcher API
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            if (api != null)
            {
                api.RegisterToken(Globals.Manifest, "Suitors", SuitorsToken);
                api.RegisterToken(Globals.Manifest, "MaxHeartSuitors", MaxHeartSuitorsToken);
                api.RegisterToken(Globals.Manifest, "RivalSuitors", RivalSuitorsToken);
                api.RegisterToken(Globals.Manifest, "Partner", PartnerToken);
                api.RegisterToken(Globals.Manifest, "Fiancee", FianceeToken);
                api.RegisterToken(Globals.Manifest, "BlackHeartSuitor", BlackHeartSuitorToken);

                Globals.Monitor.Log($"Finished registering sweet tokens");
            }
        }

        //private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        //{
        //SuitorsToken.Debug();
        //MaxHeartSuitorsToken.Debug();
        //}

        /// <summary>Initializes Global variables.</summary>
        /// <param name="helper" />
        private void SetUpGlobals(IModHelper helper)
        {
            Globals.Helper = helper;
            Globals.Monitor = Monitor;
            Globals.Manifest = ModManifest;
            Globals.Config = this.config;
        }
    }
}