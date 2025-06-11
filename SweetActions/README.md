# SweetActions
A mod that adds a few relationship actions:
* DoDating -- sets dating status on the target NPC
* DoBreakup -- ends dating status on the target NPC
* DoEngagement -- sets up the wedding (3 days in advance, as in vanilla)

## For Mod Authors

You'll need to add Kantrip.SweetActions as a required dependency to your manifest.json

```json
"Dependencies": [
    {
        "UniqueID": "Kantrip.SweetActions",
        "IsRequired": true,
    },
]
```

### EXAMPLES:
Here's an example of how I'm using the DoBreakup trigger to end a relationship from within an event (no wilted boquet needed):
```json
"{{ModId}}_Says_No":   "/action Kantrip.SweetActions_DoBreakup Elliott
                        /friendship Elliott -1000  --I'm handling friendship loss in the event, because DoBreakup doesn't change this
                        /speak Elliott \"Oh...\"
                        /emote Elliott 28
                        /pause 4000
                        /speak Elliott \"I guess I was mistaken.$s\"
                        /pause 1000
                        /end",
```

You can also use the `$action` tag in dialogue to to do the same thing. And of course, these work perfectly well with TriggerActions.

Example of using this in dialogue (with the $action command)

## NOTE:
This isn't extensively documented yet because I'm just using SweetActions internally. If you find this code and would like to use this mod, contact me on Nexus or Discord and I'll write better documentation and publish the mod for real.
