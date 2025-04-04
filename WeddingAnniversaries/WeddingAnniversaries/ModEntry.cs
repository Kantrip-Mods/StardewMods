using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace WeddingAnniversaries
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        internal static IModHelper help = null!;
        internal static int kPeriod = 112;
        internal static string[] reminders = new string[3];
        internal static string[] annivText = new string[5];
        internal static string[] supportedSpouses = new string[12]
        {
            "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Shane", "Sebastian"
        };

        /// <summary>The loaded data.</summary>
         private Dictionary<string, string> Dialogue;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            help = helper;
            
            readAnniversaryReminders();

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Content.AssetReady += this.OnAssetReady;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            helper.Events.GameLoop.DayStarted += this.DayStarted;
        }

        

        /*********
        ** Private methods
        *********/
        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            //
            // 1. define the custom asset based on the internal file
            //
            if (e.Name.IsEquivalentTo("Mods/Kantrip.WeddingAnniversaries/Dialogue"))
            {
                e.LoadFromModFile<Dictionary<string, string>>("assets/Dialogue.json", AssetLoadPriority.Medium);
            }
        }

        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            //
            // 2. update the data when it's reloaded
            //
            if (e.Name.IsEquivalentTo("Mods/Kantrip.WeddingAnniversaries/Dialogue"))
            {
                this.Dialogue = Game1.content.Load<Dictionary<string, string>>("Mods/Kantrip.WeddingAnniversaries/Dialogue");
            }
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //
            // 3. load the data
            //    (This doesn't need to be in OnGameLaunched, you can load it later depending on your mod logic.)
            //
            this.Dialogue= Game1.content.Load<Dictionary<string, string>>("Mods/Kantrip.WeddingAnniversaries/Dialogue");
        }

        /// <summary>Called once to setup the anniversary reminders</summary>
        private void readAnniversaryReminders()
        {
            reminders[0] = help.Translation.Get("AnniversaryReminder.0").Default("Missing translation");
            reminders[1] = help.Translation.Get("AnniversaryReminder.1").Default("Missing translation");
            reminders[2] = help.Translation.Get("AnniversaryReminder.2").Default("Missing translation");
        }

        /// <summary>Populates the list with the appropriate text for each spouse</summary>
        /// <param name="key">Usually the spouse's name, but can be a value like "Bad" or "Default"</param>
        private void readAnniversaryText( string key )
        {
            this.Monitor.Log($"current dialogue key: {key}.", LogLevel.Debug);
            for( int ct = 0; ct < 5; ct++ )
            {
                string lineKey =  key + ".Anniversary." + ct;
                string line = help.Translation.Get(lineKey);

                string giftKey = key + ".gifts";
                string gifts = help.Translation.Get(giftKey).Default("Missing translation");
                annivText[ct] = line + " [" + gifts + "]";
            }
        }

        //1. Check if dialogue key exists in the asset. If so, use that.
        //2. If it doesn't, create a new dialogue key and load directly from this mod's translation files
        private void PushAnniversaryText(NPC npc)
        {
            //Refresh this, in case someone has written to it
            this.Dialogue = Game1.content.Load<Dictionary<string, string>>("Mods/Kantrip.WeddingAnniversaries/Dialogue");

            string nameKey = npc.getName();
            string dialogueKey = "Anniversary_" + nameKey;
            string giftsKey = "Gifts_" + nameKey;
            
            bool lineFound = false;
            string anniversaryLine = "";
            string anniversaryGifts = "";
            if (this.Dialogue.TryGetValue(dialogueKey, out string? dialogue))
            {
                lineFound = true;
                anniversaryLine = dialogue;
            }
            else{
                this.Monitor.Log($"Unable to load anniversary line from {dialogueKey}", LogLevel.Debug);
            }

            //Make sure gifts are populated
            if( this.Dialogue.TryGetValue(giftsKey, out string? gifts ))
            {
                anniversaryGifts = gifts;
                anniversaryLine += " [" + anniversaryGifts + " ]";
            }
            else{ //fallback to default
                anniversaryGifts = help.Translation.Get("Default.gifts").Default("62 72 797 595");
                anniversaryLine += " [" + anniversaryGifts + " ]";
            }

            //Couldn't find the line we needed on the resource, so ignore what we just did and pull it 
            // directly from this mod's i18n
            if( !lineFound )
            {
                string i18nKey = nameKey;
                if( !supportedSpouses.Contains(nameKey) )
                {
                    i18nKey = "Default";
                }

                if (Game1.player.getFriendshipHeartLevelForNPC(nameKey) < 9)
                {
                    i18nKey = "Bad";    
                }

                //Re-populate the list with values for the current spouse
                readAnniversaryText(i18nKey);
                int idx = Random.Shared.Next(0,4);
                anniversaryLine = annivText[idx];

                dialogueKey = "Anniversary_" + i18nKey;
            }

            //Overwrite the dialogue now
            if(!string.IsNullOrEmpty(anniversaryLine))
            {
                npc.CurrentDialogue.Clear();
                npc.currentMarriageDialogue.Clear();
                npc.CurrentDialogue.Clear();
                npc.CurrentDialogue.Push(new Dialogue(npc, dialogueKey, anniversaryLine) { removeOnNextMove = true });
            }
        }

        private void PushReminderText(NPC npc)
        {
            string nameKey = npc.getName();
            string dialogueKey = "Reminder_" + nameKey;
            
            bool foundOnResource = false;
            string reminderLine = "";
            if (npc.Dialogue.TryGetValue(dialogueKey, out string? dialogue))
            {
                foundOnResource = true;
                reminderLine = dialogue;
            }

            if( !foundOnResource )
            {
                //Re-populate the list with values for the current spouse
                int idx = Random.Shared.Next(0,2);
                reminderLine = reminders[idx];
            }

            if(!string.IsNullOrEmpty(reminderLine))
            {
                npc.CurrentDialogue.Clear();
                npc.currentMarriageDialogue.Clear();
                npc.CurrentDialogue.Clear();
                npc.CurrentDialogue.Push(new Dialogue(npc, dialogueKey, reminderLine) { removeOnNextMove = true });
            }
        }

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            StardewValley.Network.NetStringDictionary<Friendship, Netcode.NetRef<Friendship>> fData = Game1.player.friendshipData;
             
            foreach (string name in Game1.player.friendshipData.Keys)
            {
                NPC spouse = Game1.getCharacterFromName(name);
                if (spouse == null)
                {
                    continue;
                }

                Friendship friendship = Game1.player.friendshipData[name];
                if( !spouse.isMarried() )
                {
                    //this.Monitor.Log($"{Game1.player.Name} not married to {spouse.getName()} ({name}).", LogLevel.Debug);
                    continue;
                }
                else if (friendship.DaysMarried <= 0)
                {
                    this.Monitor.Log($"{Game1.player.Name} wedding date with {spouse.getName()}: {friendship.WeddingDate}", LogLevel.Debug);
                    continue;
                }

                this.Monitor.Log($"{Game1.player.Name} married to {spouse.getName()} for {friendship.DaysMarried} days.", LogLevel.Debug);

                //Anniversary text on the day
                if( friendship.DaysMarried % kPeriod == 0 )
                {
                    this.Monitor.Log($"{Game1.player.Name}'s anniversary day with {spouse.getName()}", LogLevel.Debug);
                    PushAnniversaryText(spouse);
                }

                //Anniversary reminders one week before
                else if(friendship.DaysMarried % kPeriod == (kPeriod - 7))
                {
                    this.Monitor.Log($"Time for an anniversary reminder from {spouse.getName()}", LogLevel.Debug);
                    PushReminderText(spouse);
                }
            }
        }
    }
}