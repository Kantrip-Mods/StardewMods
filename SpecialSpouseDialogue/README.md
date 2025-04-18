# SpecialSpouseDialogue
A framework mod that allows CP mod authors to easily overwrite spouse dialogue. This is good for wedding nights, special events, etc.

## For Mod Authors
Wedding Anniversaries creates a custom Dialogue asset that can be written to via Content Patcher. If the asset is populated at the beginning of the day, the NPC spouse will use that line instead of ANY of the other vanilla lines that could already be queued up by the game.  

If you would like to use this asset, you will need to make the following changes:

1. Add Kantrip.SpecialSpouseDialogue as a non-required dependency to your manifest.json

```json
"Dependencies": [
    {
        "UniqueID": "Kantrip.SpecialSpouseDialogue",
        "IsRequired": false,
        "MinimumVersion": "1.0",
    },
]
```

2. Create a dialogue patch with your NPC's name. This will work for vanilla NPCs as well. For example, this will patch the marriage dialogue for Sebastian when a flag is set:

```json
{
    "Action": "EditData",
    "Target": "Mods/Kantrip.SpecialSpouseDialogue/Dialogue",
    "When": {
        "HasMod": "Kantrip.SpecialSpouseDialogue",
        "HasMail" "<author.modname>_Flag_WeddingNight",
    },
    "Entries": {
        "SSD_Sebastian": "Wow. You were increible last night, @.$l",
    }
},
```

## Dialogue Keys
This mod respects the following dialogue keys, where **[NPC]** is the NPC's name (e.g. "Abigail", "Harvey", etc):
* **SSD_[NPC]** : This is the dialogue line that will show up the first time the player speaks to the NPC spouse in question.

If no dialogue key is found, SpecialSpouseDialogue will leave the vanilla marriage dialogue alone. It also wipes the asset at the end of every day, so the CP authors dont have to perform any cleanup.

## Credits

## Included Translations
