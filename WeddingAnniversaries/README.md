# WeddingAnniversaries
A mod that allows NPC spouses to remember their anniversaries. This is version 2.0 of the mod -- a complete rework from its origins as a Content Patcher mod.

## For Mod Authors
Wedding Anniversaries creates a custom Dialogue asset that can be written to via Content Patcher. If you would like your NPC to have anniversary dialogue, you will need to make the following changes:

1. Add Kantrip.WeddingAnniversaries as a non-required dependency to your manifest.json

```json
"Dependencies": [
    {
        "UniqueID": "Kantrip.WeddingAnniversaries",
        "IsRequired": false,
        "MinimumVersion": "2.0",
    },
]
```

2. Create a dialogue patch with your NPC's name. This will work for vanilla NPCs as well. For example, this will patch the  anniversary reminder, annniversary day message, and anniversary gifts for both Sebastian and for ichortower's Hat Mouse Lacey:

```json
{
    "Action": "EditData",
    "Target": "Mods/Kantrip.WeddingAnniversaries/Dialogue",
    "When": {
        "HasMod": "Kantrip.WeddingAnniversaries",
    },
    "Entries": {
        "Reminder_Sebastian": "Hey um.... you remember that our anniversary is in a week, right?",
        "Anniversary_Sebastian": "Hey... happy anniversary, @.$h",
        "Gifts_Sebastian": "62 72 797 595",

        "Reminder_Lacey": "Our anniversary is next week!$h",
        "Anniversary_Lacey": "Happy anniversary, @.$h",
        "Gifts_Lacey": "StardropTea 221 873 525",
    }
},
```

You can, of course, use CP to randomize those lines to your heart's content. WeddingAnniversaries contains a set of five lines for each NPC by default, as well as a gift lift of unique item ids for each of the vanilla spouses.

## Credits
Special thanks to:
* ApryllForever, who helped me figure out what was required to make this work with hers (and who put in a bunch of work on my behalf)
* TheMightyAmondee, who created a new custom token to make the first version of this mod work
* Mushymato, who gently pointed me in the right direction on several occasions.