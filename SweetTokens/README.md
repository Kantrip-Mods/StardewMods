# SweetTokens
A mod that adds a couple of new relationship tokens for Content Patcher.

* Suitors
* MaxHeartSuitors: requires a "type" input (vanilla, custom)
* RivalSuitors: requires a "for" input (an NPC name)
* IsEngaged: requires a "player" input (host/local)

## For Mod Authors

1. Add Kantrip.SweetTokens as a required dependency to your manifest.json

```json
"Dependencies": [
    {
        "UniqueID": "Kantrip.SweetTokens",
        "IsRequired": true,
    },
]
```

## Examples:

Here's I'm checking whether there are any 10-heart suitors in a patch:
```json
"HasValue: {{Kantrip.SweetTokens/MaxHeartSuitors:type=all}}": true,
```

Here I'm extracting a Rival for Elliott from among the players current suitors:
```json
"Rival": "{{Random: {{Kantrip.SweetTokens/RivalSuitors:for=Elliott }} }}",
```

Here I'm checking whether the player is currently dating anyone:
```json
"Query: {{Count:{{Kantrip.SweetTokens/Suitors}}}} = 1": true,
```

## NOTE:
This isn't extensively documented yet because I'm just using SweetTokens internally. If you find this code and would like to use this mod, contact me on Nexus or Discord and I'll write better documentation and publish it as a separate mod.

## LICENSE (and credits):
Unlike the rest of my mods which are MIT licensed, SweetTokens is being released under GPL because the BaseToken class was copied directly from Vertigon's Stats As Tokens mod.