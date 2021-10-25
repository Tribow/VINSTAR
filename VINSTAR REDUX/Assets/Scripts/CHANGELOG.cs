/*
 THE VERSION OF THIS UNITY IS 2019.4!
 IF YOU USE A DIFFERENT UNITY VERSION, DO NOT COLLAB THE PROJECT VERSION FILE!
 
Changelog:
--------------------------------------------------------------------------
-The enemy spawn cap returns. It caps out at 220 enemies. You hit this cap at level 20.
-The minimum amount of enemies that can exist at once for each level now slowly scales with the level count.
-The minimum amount of enemies that can exist at once for each level caps at 180 enemies. You hit this cap at level 4,000.
-Minerals no longer check for the manager
-Asteroids have a 5% chance to be a powerup asteroid. (white asteroid can stack with powerup variant)
-Created a script for loading prefabs from Resources folder
-Created a powerup object for the speed upgrade and the acceleration upgrade
-The player is able to pick up powerups. These powerup upgrades persist between levels
-The enemies are able to pick up powerups. The most recent powerup the enemy gets affects their outline
-When an enemy upgrades with a powerup it keeps the powerup to its upgraded form
-When an enemy dies while holding a powerup it will drop it on death
-When a player dies approximately half of the powerups will be dropped (I might have to adjust the math here)
-The player now bounces a bit harder while in slow mode (might not notice it much)
-The player takes damage from touching the boss (FOR REAL THIS TIME)


To-do: 
--------------------------GAMEPLAY SHET------------------------------------
-Bullets need velocity data (forgot why) (might not actually need to do this)
-Add a new controls option where the ship will turn towards the direction the stick is angled
-Change outline shader so instead of detecting the edges of the sprite and making an outline, just draw a version solid color version of the sprite below the sprite and scale it? Should work better maybe
-Outline shader should gain the ability to be multicolored to communicate when an enemy has more than one powerup
-Add a version of a powerup that actually powers you down (especially funny on enemies)
-Add these powerups/powerdowns into the game:
    -Speed Powerup ✓
    -Acceleration Powerup ✓
    -Fire Rate Powerup
    -Bullet lifetime Powerup
    -Bullet Speed Powerup
    -Extra Bullet Powerup (Effect changes based on shot type)
    -Handling Powerup
    -Ship Size Powerup (Minimum size for powerdown)
    -Bullet Size Powerup (Minimum size for powerdown)
    -Magnet Range Powerup
    -Random Powerup (Everything above)
    -Shot Type Powerups:
        -Shot Types will change the current shot type the player has on pickup
        -The new shot type gets added to a list of shot types. The player can have two extra shot types including default
        -If the player collects a third extra shot type the player will drop their current shot for the new one. In the case of the default shot, it will choose a random one to drop
        -The player can cycle through their shot types with 2 new inputs
        -Enemies that collect the powerup will get two extra bullet powerups instead
            -Spread Shot Powerup:
                -Nerfs bullet damage by dividing it by 3
            -Burst Shot Powerup (Fires in all directions, starts out at 4 bullets):
                -Nerfs fire rate
                -Nerfs damage slightly
                -Nerfs bullet lifetime slightly
            -Shotgun Powerup:
                -Nerfs damage
                -Greatly nerfs fire rate
                -Greatly nerfs bullet lifetime
                -Bullets randomize their speed
            -Laser Shot Powerup:
                -Nerfs fire rate
                -Ignores bullet speed
                -Bullet lifetime affects range
            -Random Direction Shot Powerup:
                -Buffs fire rate slightly
                -Buffs damage slightly
            -Machinegun Powerup:
                -Greatly buffs fire rate
                -Nerfs damage
                -Bullets randomize their angle within 20 degrees
            -Sword Laser Powerup?:
                -Ignores bullet speed
                -Buffs handling
                -Greatly Buffs damage
                -Player must hold down the shoot button to keep it out
                -Takes a second to actually come out once button is pressed. Timing changes with fire rate
                -Bullet lifetime affects length
    -Temporary Powerups:
        -Has a small chance to spawn instead of the normal powerup
        -Enemies that pick up this powerup will get the same effect
        -Cannot drop on death
        -Different shader effects depending on what the powerup does
        -Separate powerups can be stacked, the bullets shape will use the most recent shot shape
            -Piercing Bullet:
                -Bullets go until they hit their lifetime
                -Bullets get a purple particle trail
            -Bomb Bullet:
                -Bullets explode into an AoE on impact
                -Bullet itself no longer does damage, only the AoE does
                -Affected by bullet's size
                -Explosions damage player and enemy
                -Bullets get a red particle trail
            -Terminal Velocity
                -Multiplies max speed by 5
                -Gives very high acceleration
                -Yellow Flash turned on
            -Focus!
                -Forces you into slow mode
                -Enemies that grab this get super reduced max speed
            -Proximity Mines
                -Player/Enemy starts to periodically drop proximity mines that take a second to activate
                -Automatically explodes after 10 seconds
                -Explosion damage player and enemy
            -Phase Powerup
                -No longer bounces off of anything
                -Object becomes semi-transparent
            -Teleport
                -Teleports you to a random location in the level
            -Time Bomb
                -Player/Enemy will explode in 15 seconds with huge AoE
                -The bomb can be passed to other enemies or the player on collision
                -When the bomb explodes it creates a huge AoE. Whoever holds the bomb dies instantly
                -The AoE does 500 damage to enemies and does 5 damage to the player
                -White Flash turned on, flashes red each second
-Adding some networking for leaderboard so it's on a server
-Add player data on server too



-----------------------------UI SHET--------------------------------------
-Change title screen a little to make it look cooler (yeah idk what to do here)
-Change the UI for inputting your name into the leaderboard (Slightly changed but not really)
-Add UI that shows the current stats the player has in powerups. Should have a bar for each stat
-Add the ability to adjust resolution
-Create UI for it in the options menu
-Tutorial text detects if you start using controller (sorta works)


---------------------------AUDIO SHET-------------------------------------
-Add sound effects for a level change
-Add sound effects for when the leaderboard comes up
-The sound of the player's bullets should not be affected by its distance from the camera
-Add sound for picking up minerals
-Add sound for picking up powerup

--------------------------PARTICLE SHET------------------------------------
-Particle effect for taking damage? (unsure if I should actually do that)
-Add particle effect for when the magnet activates
-Particle effect for bullet despawns?
-Add particle effect when collecting a powerup

Issues:
-Occassionally when firing the bullets wont actually appear (They get warped to a random position??) (maybe fixed?????) (I DON'T KNOW????)
-If you click on the menu it will remove the keyboard's access to it until you go to a new menu
-Tutorial does not exist in the WebGL version now
-You cannot enter a name on the WebGL version for some reason
-Camera has trouble following the player at the edges of the level


 */ 