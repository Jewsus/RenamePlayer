# RenamePlayer
Rewrite of Scavenger's RenamePlayer plugin. Renames player if the name has any invalid (non-ASCII) characters.

####Usage:
- N/A

####Permissions:
- N/A

####Design:
-Hooks on Greet Player
-Converts copy of player name to ASCII code page
-If name is different
- Notify player
- Broadcast to all because this happens after join message
