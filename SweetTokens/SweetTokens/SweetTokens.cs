using StardewModdingAPI;
using StardewValley;

namespace SweetTokens
{

    /// <summary>A token which returns the names of all the NPCs that the player is dating currently.</summary>
    internal class SuitorsToken : BaseToken
    {
        /*********
        ** Fields
        *********/
        /// <summary>The list of suitors as of the last context update.</summary>
        internal List<NPC> cachedSuitors = new List<NPC>();

        public SuitorsToken()
        {
        }

        internal void Debug()
        {
            string sep = ",";
            var allNames = string.Join(sep, cachedSuitors);
            Globals.Monitor.Log($"Current suitors (all): {allNames}, count = {cachedSuitors.Count}", LogLevel.Debug);
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public override bool CanHaveMultipleValues(string input)
        {
            return true;
        }

        public override bool RequiresInput()
        {
            return false;
        }

        protected override bool DidDataChange()
        {
            //Globals.Monitor.Log($"SuitorsToken: DidDataChange()", LogLevel.Debug);
            bool hasChanged = false;
            List<NPC> suitors = new();

            GetSuitors(ref suitors);

            if (suitors.Count != cachedSuitors.Count)
            {
                hasChanged = true;
            }

            if (!hasChanged)
            {
                foreach (NPC npc in suitors)
                {
                    if (!cachedSuitors.Contains(npc))
                    {
                        hasChanged = true;
                        break;
                    }
                }
            }

            if (hasChanged)
            {
                cachedSuitors.Clear();
                cachedSuitors = suitors;
            }
            return hasChanged;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            return true;
        }

        /// <summary>Get the current values.</summary>
        public override IEnumerable<string> GetValues(string input)
        {
            List<string> output = new();

            // get names
            foreach (NPC npc in cachedSuitors)
            {
                output.Add(npc.Name);
            }

            if (output.Count == 0)
            {
                output.Add("null");
            }
            return output;

            /* //not entirely sure why this isn't OK
            if (output.Count == 0)
            {
                yield break;
            }

            foreach (string name in output)
            {
                yield return name;
            }
            */
        }

        // get names
        private void GetSuitors(ref List<NPC> suitors)
        {
            //Globals.Monitor.Log($"Suitors Token: GetSuitors() called", LogLevel.Debug);
            Farmer farmer = Game1.player;
            foreach (string name in farmer.friendshipData.Keys)
            {
                NPC npc = Game1.getCharacterFromName(name);
                if (npc == null)
                {
                    continue;
                }

                Friendship friendship = farmer.friendshipData[name];
                if (npc.isMarried() || !friendship.IsDating())
                {
                    //this.Monitor.Log($"{Game1.player.Name} not married to {spouse.Name} ({name}).", LogLevel.Debug);
                    continue;
                }

                suitors.Add(npc);
            }
        }
    }

    internal class MaxHeartSuitorsToken : BaseToken
    {
        /*********
        ** Fields
        *********/
        internal static string[] vanillaSuitors = new string[12]
        {
            "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Shane", "Sebastian"
        };

        /// <summary>The list of suitors as of the last context update.</summary>
        internal List<NPC> cachedSuitors = new List<NPC>();

        public MaxHeartSuitorsToken()
        {
        }
        internal void Debug()
        {
            List<string> vanilla = TryFilterNames("vanilla");
            List<string> custom = TryFilterNames("custom");
            List<string> all = TryFilterNames("all");

            string sep = ",";
            var vanillaNames = string.Join(sep, vanilla);
            var customNames = string.Join(sep, vanilla);
            var allNames = string.Join(sep, vanilla);

            Globals.Monitor.Log($"10 heart suitors (vanilla): {vanillaNames}, count = {vanilla.Count}", LogLevel.Debug);
            Globals.Monitor.Log($"10 heart suitors (custom): {customNames}, count = {custom.Count}", LogLevel.Debug);
            Globals.Monitor.Log($"10 heart suitors (all): {allNames}, count = {all.Count}", LogLevel.Debug);
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
		public override bool CanHaveMultipleValues(string input = null)
        {
            return (cachedSuitors.Count > 1);
        }

        public override bool RequiresInput()
        {
            return true;
        }

        protected override bool DidDataChange()
        {
            //Globals.Monitor.Log($"MaxHeartSuitorsToken: DidDataChange()", LogLevel.Debug);

            bool hasChanged = false;
            List<NPC> suitors = new();

            GetMaxHeartSuitors(ref suitors);

            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);

            if (suitors.Count != cachedSuitors.Count)
            {
                hasChanged = true;
            }

            if (!hasChanged)
            {
                foreach (NPC npc in suitors)
                {
                    if (!cachedSuitors.Contains(npc))
                    {
                        hasChanged = true;
                        break;
                    }
                }
            }

            if (hasChanged)
            {
                cachedSuitors.Clear();
                cachedSuitors = suitors;
            }
            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);
            //Globals.Monitor.Log($"hasChanged: {hasChanged}", LogLevel.Debug);
            return hasChanged;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            string[] args = input.ToLower().Trim().Split('|');

            if (args.Length == 1)
            {
                if (!args[0].Contains("type="))
                {
                    error += "Named argument 'type' not provided. Must be a string consisting of alphanumeric characters. ";
                    return false;
                }
                else if (args[0].IndexOf('=') == args[0].Length - 1)
                {
                    error += "Named argument 'type' must be provided a value. ";
                }
                else
                {
                    string type = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");
                    if (type != "vanilla" && type != "custom" && type != "all")
                    {
                        error += "Named argument 'type' must be one of the following values: vanilla, custom, all. ";
                    }
                }
            }
            else
            {
                error += "Incorrect number of arguments provided. A 'type' argument is required. ";
            }

            return error.Equals("");
        }

        /// <summary>Get the current values.</summary>
        public override IEnumerable<string> GetValues(string input)
        {
            List<string> output = new();

            string[] args = input.Split('|');

            string type = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace(" ", "");
            output = TryFilterNames(type);

            if (output.Count == 0)
            {
                output.Add("null");
            }
            return output;
        }

        private List<string> TryFilterNames(string type)
        {
            List<string> output = new();
            if (type == "custom")
            {
                foreach (NPC npc in cachedSuitors)
                {
                    if (!vanillaSuitors.Contains(npc.Name))
                    {
                        output.Add(npc.Name);
                    }
                }
            }
            else if (type == "vanilla")
            {
                foreach (NPC npc in cachedSuitors)
                {
                    if (vanillaSuitors.Contains(npc.Name))
                    {
                        output.Add(npc.Name);
                    }
                }
            }
            else if (type == "all")
            {
                foreach (NPC npc in cachedSuitors)
                {
                    output.Add(npc.Name);
                }
            }
            return output;
        }

        // get names
        private void GetMaxHeartSuitors(ref List<NPC> suitors)
        {
            //Globals.Monitor.Log($"MaxHeartSuitors Token: GetMaxHeartSuitors() called", LogLevel.Debug);

            Farmer farmer = Game1.player;
            foreach (string name in farmer.friendshipData.Keys)
            {
                NPC npc = Game1.getCharacterFromName(name);
                if (npc == null)
                {
                    continue;
                }

                Friendship friendship = farmer.friendshipData[name];
                if (npc.isMarried() || !friendship.IsDating())
                {
                    //Globals.Monitor.Log($"{{npc.Name}} is not dating {Game1.player.Name}", LogLevel.Debug);
                    continue;
                }

                int hearts = friendship.Points / 250;
                if (hearts >= 10)
                {
                    suitors.Add(npc);
                }
            }
        }
    }

    internal class RivalSuitorsToken : BaseToken
    {
        /// <summary>The list of suitors as of the last context update.</summary>
        internal List<NPC> cachedSuitors = new List<NPC>();

        public RivalSuitorsToken()
        {
        }
        internal void Debug()
        {
            string sep = ",";
            var allNames = string.Join(sep, cachedSuitors);

            Globals.Monitor.Log($"10 heart suitors (all): {allNames}, count = {cachedSuitors.Count}", LogLevel.Debug);
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
		public override bool CanHaveMultipleValues(string input = null)
        {
            return (cachedSuitors.Count > 1);
        }

        public override bool RequiresInput()
        {
            return true;
        }

        protected override bool DidDataChange()
        {
            //Globals.Monitor.Log($"MaxHeartSuitorsToken: DidDataChange()", LogLevel.Debug);

            bool hasChanged = false;
            List<NPC> suitors = new();

            GetSuitors(ref suitors);

            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);

            if (suitors.Count != cachedSuitors.Count)
            {
                hasChanged = true;
            }

            if (!hasChanged)
            {
                foreach (NPC npc in suitors)
                {
                    if (!cachedSuitors.Contains(npc))
                    {
                        hasChanged = true;
                        break;
                    }
                }
            }

            if (hasChanged)
            {
                cachedSuitors.Clear();
                cachedSuitors = suitors;
            }
            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);
            //Globals.Monitor.Log($"hasChanged: {hasChanged}", LogLevel.Debug);
            return hasChanged;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            string[] args = input.ToLower().Trim().Split('|');

            if (args.Length == 1)
            {
                if (!args[0].Contains("for="))
                {
                    error += "Named argument 'for' not provided. Must be a string consisting of alphanumeric characters. ";
                    return false;
                }
                else if (args[0].IndexOf('=') == args[0].Length - 1)
                {
                    error += "Named argument 'for' must be provided a value. ";
                }
                else if (cachedSuitors.Count <= 1)
                {
                    error += "There aren't enough suitors for there to be rivals.";
                }
                else
                {
                    args = input.Trim().Split('|'); //reload args, without changing string case

                    //Check that the input name is valid with the game
                    string name = args[0].Substring(args[0].IndexOf('=') + 1).Trim().Replace(" ", "");
                    NPC npc = Game1.getCharacterFromName(name);
                    if (npc == null)
                    {
                        error += "There is no NPC with the name '" + name + "'";
                    }
                    else
                    {
                        //TODO: Do I care about this? If the player isn't dating the person, the return list will still be the same.
                        Friendship friendship = Game1.player.friendshipData[npc.Name];
                        if (friendship == null || !friendship.IsDating())
                        {
                            error += Game1.player.Name + " isn't dating " + npc.Name;
                        }
                    }
                }
            }
            else
            {
                error += "Incorrect number of arguments provided. An npc name is required. ";
            }

            if (!error.Equals(""))
            {
                Globals.Monitor.Log($"error: {error}", LogLevel.Debug);
            }
            return error.Equals("");
        }

        /// <summary>Get the current values.</summary>
        public override IEnumerable<string> GetValues(string input)
        {
            List<string> output = new();

            string[] args = input.Split('|');

            string suitorName = args[0].Substring(args[0].IndexOf('=') + 1).Trim().Replace(" ", "");
            output = TryFilterNames(suitorName);

            if (output.Count == 0)
            {
                output.Add("null");
            }
            return output;
        }

        private List<string> TryFilterNames(string suitorName)
        {
            List<string> output = new();

            foreach (NPC npc in cachedSuitors)
            {
                if (npc.Name != suitorName)
                {
                    output.Add(npc.Name);
                }
            }
            return output;
        }

        // get names
        private void GetSuitors(ref List<NPC> suitors)
        {
            //Globals.Monitor.Log($"RivalSuitors Token: GetSuitors() called", LogLevel.Debug);

            Farmer farmer = Game1.player;
            foreach (string name in farmer.friendshipData.Keys)
            {
                NPC npc = Game1.getCharacterFromName(name);
                if (npc == null)
                {
                    continue;
                }

                Friendship friendship = farmer.friendshipData[name];
                if (npc.isMarried() || !friendship.IsDating())
                {
                    //Globals.Monitor.Log($"{{npc.Name}} is not dating {Game1.player.Name}", LogLevel.Debug);
                    continue;
                }

                suitors.Add(npc);
            }
        }
    }

    //Returns the name of the current player's partner (spouse or fiancee)
    internal class PartnerToken : BaseToken
    {
        /// <summary>The list of suitors as of the last context update.</summary>
        internal string partnerName = "";

        public PartnerToken()
        {
        }
        internal void Debug()
        {
            Globals.Monitor.Log($"Partner: {partnerName}", LogLevel.Debug);
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
		public override bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public override bool RequiresInput()
        {
            return false;
        }

        protected override bool DidDataChange()
        {
            //Globals.Monitor.Log($"MaxHeartSuitorsToken: DidDataChange()", LogLevel.Debug);

            bool hasChanged = false;
            string partner = "";
            GetPartner(ref partner);

            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);

            if (partner != partnerName)
            {
                hasChanged = true;
                partnerName = partner;
            }

            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);
            //Globals.Monitor.Log($"hasChanged: {hasChanged}", LogLevel.Debug);
            return hasChanged;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            return error.Equals("");
        }

        /// <summary>Get the current values.</summary>
        public override IEnumerable<string> GetValues(string input)
        {
            bool found = (partnerName != "");
            if (!found)
            {
                yield break;
            }

            yield return partnerName;
        }

        // get names
        private void GetPartner(ref string partner)
        {
            //Globals.Monitor.Log($"RivalSuitors Token: GetSuitors() called", LogLevel.Debug);

            Farmer farmer = Game1.player;
            foreach (string name in farmer.friendshipData.Keys)
            {
                NPC npc = Game1.getCharacterFromName(name);
                if (npc == null)
                {
                    continue;
                }

                Friendship friendship = farmer.friendshipData[name];
                if (friendship.IsEngaged() || friendship.IsMarried())
                {
                    //Globals.Monitor.Log($"{{npc.Name}} is not dating {Game1.player.Name}", LogLevel.Debug);
                    partner = npc.Name;
                    break;
                }
            }
        }
    }

    //Returns the name of the current player's Fiancee
    internal class FianceeToken : BaseToken
    {
        /// <summary>The list of suitors as of the last context update.</summary>
        internal string partnerName = "";

        public FianceeToken()
        {
        }
        internal void Debug()
        {
            Globals.Monitor.Log($"Fiancee: {partnerName}", LogLevel.Debug);
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
		public override bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public override bool RequiresInput()
        {
            return false;
        }

        protected override bool DidDataChange()
        {
            //Globals.Monitor.Log($"MaxHeartSuitorsToken: DidDataChange()", LogLevel.Debug);

            bool hasChanged = false;
            string partner = "";
            GetFiancee(ref partner);

            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);

            if (partner != partnerName)
            {
                hasChanged = true;
                partnerName = partner;
            }

            //Globals.Monitor.Log($"suitors.Count: {suitors.Count}, cachedSuitors.Count: {cachedSuitors.Count}", LogLevel.Debug);
            //Globals.Monitor.Log($"hasChanged: {hasChanged}", LogLevel.Debug);
            return hasChanged;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            return error.Equals("");
        }

        /// <summary>Get the current values.</summary>
        public override IEnumerable<string> GetValues(string input)
        {
            bool found = (partnerName != "");
            if (!found)
            {
                yield break;
            }

            yield return partnerName;
        }

        // get names
        private void GetFiancee(ref string partner)
        {
            //Globals.Monitor.Log($"RivalSuitors Token: GetSuitors() called", LogLevel.Debug);

            Farmer farmer = Game1.player;
            foreach (string name in farmer.friendshipData.Keys)
            {
                NPC npc = Game1.getCharacterFromName(name);
                if (npc == null)
                {
                    continue;
                }

                Friendship friendship = farmer.friendshipData[name];
                if (friendship.IsEngaged() )
                {
                    //Globals.Monitor.Log($"{{npc.Name}} is not dating {Game1.player.Name}", LogLevel.Debug);
                    partner = npc.Name;
                    break;
                }
            }
        }
    }
}