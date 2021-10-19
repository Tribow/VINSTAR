/*
 THE VERSION OF THIS UNITY IS 2019.4!
 IF YOU USE A DIFFERENT UNITY VERSION, DO NOT COLLAB THE PROJECT VERSION FILE!
 
Changelog:
--------------------------------------------------------------------------
Fixed boss health bar not displaying correctly
Fixed an issue where enemies were not properly getting the manager script causing them to throw an error
White minerals will get cleared when level changes
Probably fixed an issue where player bullets would spawn in incorrect locations
I looked into it, I have genuinely no idea why the tutorial text fails to appear in the WebGL version, I don't get it
Camera now follows you a bit more closely
Camera is even smoother (It still acts a little strange on the sides but it's better)
Changed the speed of camera movement during level transition

 


To-do: 
--------------------------GAMEPLAY SHET------------------------------------
-Bullets need velocity data (forgot why)
-Add a new controls option where the ship will turn towards the direction the stick is angled
-Camera lerps the slowing down when getting close to the player
-Camera follows a little more closely
-Add new asteroid type the holds a powerup when destroyed
-Add new asteroid type where the minerals have a chance to be white minerals
-White minerals will boost a player's hp by 1 and will boost an enemy's hp by the amount of current player bullet damage


-----------------------------UI SHET--------------------------------------
-Change title screen a little to make it look cooler (yeah idk what to do here)
-Change the UI for inputting your name into the leaderboard (Slightly changed but not really)
-Add the ability to adjust resolution
-Create UI for it in the options menu
-Tutorial text detects if you start using controller (sorta works)


---------------------------AUDIO SHET-------------------------------------
-Add sound effects for a level change
-Add sound effects for when the leaderboard comes up

--------------------------PARTICLE SHET------------------------------------
-Particle effect for taking damage? (unsure if I should actually do that)

Issues:
-Occassionally when firing the bullets wont actually appear (They get warped to a random position??) (maybe fixed?????)
-If you click on the menu it will remove the keyboard's access to it until you go to a new menu
-Tutorial does not exist in the WebGL version now
 
 */ 