/*
 THE VERSION OF THIS UNITY IS 2019.4!
 IF YOU USE A DIFFERENT UNITY VERSION, DO NOT COLLAB THE PROJECT VERSION FILE!
 
Changelog:
--------------------------------------------------------------------------
Changed the background to be an image and not a tilemap
Fixed an issue where the player would not die from touching the boss even though the enemies take damage from touching boss
Fixed an issue where the boss would spawn way too quickly (I was doing this for debugging, it is back to normal)
Fixed an issue where an error might occur due to an enemy being able to upgrade before it had access to the manager
Added a particle effect for when an asteroid gets destroyed
Added a sound effect for when an asteroid gets destroyed
The pitch of the player ship's engine changes based on speed
When picking up a mineral the player now gets a light blue outline for a brief time
Got rid of some bad hardcode
Removed some redundant code
Changed the shape of the player's radar to a triangle
Objects now accurately detect the edges of the level
The tutorial text will now change if you connect a gamepad or change the controls for gamepad (probably) ((Note: It will not change if you just press something))
Bullet damage is now completely handled by the manager and not the player
Removed some pixel artifacts from one of the asteroid sprites
There is a 20% chance that an asteroid will be a white asteroid
White asteroids have 4% chance of spawning a white mineral each time it takes damage
White minerals give the player a shield that can take 1 extra hit
Shield health stacks, a white outline around the player shows how strong their shield currently is. The less health the shield has, the less visible it is
Player now flashes red when receiving damage.
Asteroids now have a 16% chance to drop a mineral in general
Adjusted how some scripts initialize
The boss is now able to pick up minerals as well. It doesn't effect it, it just holds them just like any other enemy would.
Enemies that pick up white minerals will get a white outline and receive a health boost equivalent to the player's current bullet damage.
Enemies with a white outline have a 25% chance of dropping a white mineral on death
Removed Unity's default wrapper from the WebGL version

 


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
-When a white enemy upgrades, the upgraded object does not have its material set yet and it tries to use it which causes an error
-Sometimes the start function just doesn't update soon enough
 
 */ 