/*
 THE VERSION OF THIS UNITY IS 2019.4!
 IF YOU USE A DIFFERENT UNITY VERSION, DO NOT COLLAB THE PROJECT VERSION FILE!
 
Changelog:
--------------------------------------------------------------------------
-Fixed the enemy spawn count resetting to the wrong number after starting a new run without closing the game
-Fixed the manager performing a useless check if enemies exist even though it already checked if enemies exist
-Fixed the issue where enemies would not have randomized rotations when spawned
-Enemy spawn count at the start is 20 instead of 10
-Cleaned up manager script in general to get rid of redundant code
-Boss added to the game
-The chances of the boss spawning becomes more likely each time an enemy dies. Value resets after death.
-New bullet added
-Camera zooms out when close enough to the boss
-Boss health scales with the current power of the player at the time it spawns. It will not scale afterward
-The size of the asteroids are now randomized
-Most sprites have now been given a new material and shader
-Enemies now flash when damage is dealt to them
-Sounds that play a certain distance away from the camera will not be heard.
-Enemy's bullets now gets destroyed the moment they die
-Added a boss health bar
-Enemies give different amount of points instead of all being the same
-Magnet for minerals activates if the player has stopped shooting for 1.5 seconds. The magnet's range is small
-Tutorial text no longer disappears when the camera is a certain distance away
-Added extra tutorial text for level 2 and 3

 


To-do: 
--------------------------GAMEPLAY SHET------------------------------------
-Fix Splitter_Fighter so that the trailing parts collision still affects the front
-Add a pointer in the radar when the boss spawns so the player can find it more easily
-Bullets need velocity data (forgot why)
-Change the tiled background to just an image since that would actually make sense


-----------------------------UI SHET--------------------------------------
-Change title screen a little to make it look cooler (yeah idk what to do here)
-Change the UI for inputting your name into the leaderboard (Slightly changed but not really)
-Add the ability to adjust resolution
-Create UI for it in the options menu
-Tutorial text detects if you start using controller


---------------------------AUDIO SHET-------------------------------------
-Pitch the sound of the engine based on speed.
-Give enemies movement sound effects
-Add sound effects for a level change
-Add sound effects for when the leaderboard comes up
-Add sound effects for an asteroid getting hit
-Fix audio slider so it is logarithmic and not linear

--------------------------PARTICLE SHET------------------------------------
-Add a particle effect manager
-Add particle effect for asteroid
-Add particle effect for boss dying
-Particle effect for taking damage? (unsure if I should actually do that)

Issues:
-Occassionally when firing the bullets wont actually appear (They get warped to a random position??)
-If you click on the menu it will remove the keyboard's access to it until you go to a new menu
-Boss appears to be no longer able to spawn after it spawns once
 
 */ 