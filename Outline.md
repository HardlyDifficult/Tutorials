# 1) Project Setup

Create a project, import assets, and set some configurations.

## 1.1) Start a Project

Start a 2D project and save the scene.

What are the limitations of Personal / Community editions?
Can I use Unity 5.* or Monodevelop instead?
What does 2D (vs 3D) impact in Unity?
What's a scene?
Can I name folders differently?


## 1.2) Import Assets

Import sprites and sounds into your project.

What's a sprite / sprite sheet?
Who made the art for this tutorial?
Can I use my own art?

## 1.3) Configure Sprites

Update each sprite to use a full rect mesh and point filter mode.

What's mesh type / full rect?
What's filter mode / point?

## 1.4) Select an Aspect Ratio

Change the aspect ratio to 5:4 in the Game window and build settings.

Why use a fixed aspect ratio?

## 1.5) Configure Camera

Update the camera size and background color.

What's camera 'Size'?

## 1.6) Auto Save Script

Create an editor script which automatically saves every time you hit 'play'.

How do I know it's working?
Why is the 'Editor' folder name important?
What's InitializeOnLoad?
What's a C# attribute?
What's a C# static constructor?
What's a C# delegate / event?
What about performance?

# 2) Platforms

Layout platforms for level 1.

## 2.1) Add a Platform

Add a sprite to the scene representing the middle segment of a platform.

What are GameObject, Transform, and Component?
What's a SpriteRenderer?
TODO what does pixel per unit do?
What does tiling do, and why not use Transform scale?

## 2.2) Platform Edges

Create a new parent GameObject for the platform sprite and add edge sprites.

Why create a parent GameObject?
How is the sprite position calculated when it's a child?

## 2.3) Two Connected Platforms

Our level design calls for the bottom platform to rotate halfway through. Create two Platform GameObjects and position and rotate their parent GameObjects so that they appear connected.

Why not use a single GameObject for this bottom platform?
Why extend the platform beyond the edge of the screen?

## 2.4) Complete Platform Layout

At this point we have covered everything you need to match the level 1 platform layout. You can match the layout we used or come up with your own.

## 2.5) Add Colliders

Add a BoxCollider2D to each of the Platforms. Add an edge radius and edit colliders to match the sprites.

What is a Collider?
Why not place colliders on the child GameObjects instead?

## 2.6) Reorganize Hierarchy

Organize GameObjects in the hierarchy and place the Platforms in a parent.

What does the order in the Hierarchy impact?

# 3) SpikeBall

Create a ball that spawns from a cloud and rolls down the platforms.

## 3.1) Create a Ball with Gravity

Add a GameObject for the SpikeBall, set the sprite's order in layer, and add a rigidbody.

What does Order in Layer do?
Why use a parent here?
What's a Rigidbody2D?

## 3.2) Add a Collider

Add a circle collider to the SpikeBall.

Why size the collider smaller than the ball?

## 3.3) Add Invisible Bumpers

Add additional box colliders off-screen to redirect balls back on-screen.

What did that do?

## 3.4) Spawn Balls

Add the EvilCloud and create a script for it to spawn SpikeBalls.

What does changing scale Z do?
What's a prefab?
What's MonoBehaviour / how is Start called?
What's SerializeField and why not use public instead?
Why use protected on the Unity event?
What's StartCoroutine / WaitForSeconds?
What does Instantiate do?
What's a Quaternion?
What does Random.Range do / what other options do we have for RNG?
Does the filename matter?
Why confirm the values in the Inspector match the defaults in code?
What does Debug.Assert do?
Why does ReSharper recommend changing variables to readonly?

## 3.5) Add Initial Momentum

Add a script to the SpikeBall which sets an initial velocity and angular velocity.

What are velocity and angularVelocity?
What's a Vector2 and how does it differ from Vector3?
What does GetComponent do / what's C# generics?
What does RequireComponent do?
Why not use a "SpikeBall" component instead?

## 3.6) Disable Collisions Between Enemies

Create a layer for enemies and update the collision matrix to disable enemy-to-enemy collisions.

What's a Layer and how's it different from a Tag?
What does the collision matrix impact?
Why not change the children layers too?

## 3.7) Destroy Balls that Roll Off

Add a script to the SpikeBall that destroys the GameObject after it rolls off the bottom platform and out of view.

Why hardcode the outOfBoundsYPosition?
Why bother if the GameObject is already off-screen?
What's Destroy and why not Destroy(this)?
What about an object pool?
Why FixedUpdate vs Update?

# 4) Character

Add a character to the scene that can walk and jump.

## 4.1) Create an Animated Character

Add a GameObject for the character with a walk animation.

What does Pivot do?
What's the difference between Animation and Animator?
How do I know what size to make the collider?
Why not use a collider that outlines the character?
Why freeze rotation, and does freezing mean it can never change?

## 4.2) Move Left & Right

Add a script to the character to be able to move left and right once a controller is added.

What's a controller? Why not read input here?
Why set velocity instead of using AddForce?
Why FixedUpdate instead of Update?
Why multiply by Time.fixedDeltaTime?
Why not use HideInInspector on desiredWalkDirection?
What do the Tooltip and Range attributes do?

## 4.3) Add a Player Controller

Add a script to the character to read user input and drive movement.

What is an Input 'Axis' and how is it configured?
Why does desiredWalkDirection slowly change?

## 4.4) Turn Around

Flip the entity's sprite when they switch between walking left and right.

Why not compare to 0 when checking if there is no movement?
TODO why not use flipX instead?

## 4.5) Jump

Add a script to the character to enable it to jump and update the player controller to match. Play a sound effect when an entity jumps.

Why use AddForce here instead of velocity, and what's 'Impulse'?
How do you know when to use Update vs FixedUpdate for Input and rigidbodies?
Why is AudioSource on a GameObject vs just a clip?
Would two separate player controllers be a better component-based solution?
TODO why not set the AudioClip instead?

## 4.6) Platform Effectors

Add a platform effector to each platform.

What are Effectors?
What does Surface Arc do and why not use a value of 1?

# 5) HoverGuy

Add a second enemy and spawner.

## 5.1) Spawn in a Hovering Enemy

Spawn in a new enemy which hovers above the ground.

How do you know what size to make the second collider?
Why use a child GameObject instead of two colliders on the parent?

## 5.2) Make the HoverGuy Walk

Add a script to the HoverGuy to drive random walk movement.

How might we improve on WanderWalkController?

## 5.3) Don't Spawn on Another Entity

Update the door so that it does not spawn if the character is too close.

How do I know what size to make the collider?
What does OverlapCollider do?
Why use a contact filter instead of a tag or a layermask?
Does the Collision Matrix impact anything when using OverlapCollider?
Why use a temp collider list?

## 5.4) Fade in Entities

Add a script to entities so that they fade in before moving.

What does StopAllCoroutines do?
What's Behaviour and how does it differ from MonoBehaviour?

# 6) Death Effects

Spawn an explosion and animate the character's death.

## 6.1) Death Effect Pattern

Create a pattern to use instead of destroying GameObjects directly, allowing an opportunity for objects to animate on death.

Why not just play effects OnDestroy?
Why is there a public method that 'should not be called directly'?
Why are you using Mathf and not System.Math?
What does GetComponentsInChildren do?
What's C# abstract do and how's it different from an interface?
Why not make DeathEffectManager a static class?

## 6.2) Kill the Character

When the character comes in contact with an enemy, kill him!

Why check the layer instead of using the Collision Matrix?
What is this '& 1 <<' black magic?
What is a C# extension method?
Why is there an empty Start method and why check if enabled?

## 6.3) Create an Explosion

Create an explosion particle system with a sound effect.

What's a particle / particle system?
Could you RNG select the audio clip to play?

## 6.4) Destroy Explosions

The explosion should destroy after a few the effect completes.

Why not Destroy instead?

## 6.5) Spawn Explosions

Add a script which spawns the explosion prefab when the character dies.

What's bounds represent?
Why not spawn the explosion at transform.position instead of bounds.center?
Why bother destroying, the explosion is not visible after a few seconds?

## 6.6) Animate the Character's Death

Add a scaling effect for the character dying, in addition to the explosion.

What does yield return null do?
Why not use an animation instead?
Why use Mathf.Sin?
How might we disable movement when the character is dying?
Why not make the originalScale readonly?

# 7) Game Controller

Create Game and Level controllers; tracking points, lives, and level respawn.

## 7.1) Create a GameController

Create a singleton GameController to track points, lives, and hold global data such as the world size.

What does DontDestroyOnLoad do?
What's a singleton and why use it?
Why is there an entire class for basically one line of code?

## 7.2) Stay on-screen

Create a script which ensures entities can not walk off-screen.

Why use bounds for these checks?
What's the difference between setting transform.position and using myBody.MovePosition?
Does setting the position cause the entity to pop on-screen?
Why use an event when another component could check screen bounds again?

## 7.3) Bounce Off Edges

Create a script to have the HoverGuy bounce off the edge of the screen and never stop walking.

Why not register for the event on Awake?
TODO why do we need to change the Z position? - Looks fine, but stay on screen was triggering on spawn due to a z problem. -

## 7.4) Respawn Character

Add scripts to destroy the enemies and respawn the character when he dies.

Why does position before saving the prefab matter?
TODO what does script execution order do?

## 7.5) Clear Level on Death

Why not use an interface instead of abstract?
What does FindObjectsOfType do?
Why not have all objects subscribe to life count changes instead or this new pattern?

## 7.6) Points for Jumping Over Enemies

Add a collider and script to award points anytime the character jumps over an enemy.

What's Raycast do?
Why Trigger AND Raycast?
Why add another Rigidbody2D / why check the collision layer manually?
Do we need a cooldown?
Why hold rotation on the Points child GameObject?
Why FixedUpdate instead of Update here?

# 8) Floor Detection

## 8.1) Detect Floors

Create a script to calculate the distance to and rotation of the floor under an entity.

What's a C# Nullable type / what's the question mark after 'float'?
What's C# 'is' do and how's it differ from 'as'?
What's Dot product do?
When do you use OverlapCollider vs Raycast vs Distance vs Trigger*?
Why add the edge radius to bounds max when calculating the floor's position?

## 8.2) Prevent Double Jump

Update the jump script to prevent double jump and flying (by spamming space), by leveraging the floor detector component just created.

Why not use a cooldown instead?

## 8.3) Prefer Wandering Up Hill

Update the HoverGuy walk controller so that it is more likely to walk up hill than down.

What did that do?
Why take the Dot product with Vector2.right?

## 8.4) Rotate Entities

Create a script to rotate an entity, aligning with the floor when touching one, otherwise rotating back to the default position.

What's 'Lerp' and how's it compare to 'Slerp'?

# 9) Ladders

Add ladders to level. Allow the player to climb up and down, and have enemies randomly climb as well.

## 9.1) Add Ladders

Create GameObjects and layout ladders in the world. Set their layer to Ladder.

Add a box collider to each of the ladders and size it to use for climbing and set it as a trigger collider. An entity will be able climb ladders when its bottom is above the bottom of the ladder's collider and its center is inside.

TODO why not tile instead?

## 9.2) Climb Ladders

Add a script to climb ladders, and updated the player controller to match.

Why use sqrMagnitude instead of magnitude?

## 9.3) Disable Physics While Climbing

While climbing a ladder disable physics, allowing entities to climb down / through a platform.

What's a C# List?
What's rigidbody gravityScale do?
Why store the impacted collider list?

## 9.4) Random Climb Controller

Create a script for the HoverGuy and SpikeBall to control when to climb a ladder.

If I set both odds to 50%, why does it go up more often then down?

## 9.5) Stay On Ladders

Stop WanderWalkController when climbing up or down.

Why not stop the WalkMovement component instead?
Why not deregister events here?
Why not stop and restart the coroutine instead?

## 9.6) Move Towards the Center of the Ladder

Add a script to the HoverGuy and SpikeBall to direct them towards the center of a ladder while climbing.

Why not use velocity to move?

## 10) Hammer

Add Hammers around the level which when picked up may be used to smash enemies.

## 10.1) Create a Hammer

Create a Hammer prefab and then layout several in the world.

Why use pivot bottom?
Why use a polygon collider and not a box or capsule?
Why use Is Trigger?
Why not simply sum the time used in WaitForSeconds instead of max with deltaTime?
Why use GetComponentsInChildren instead of a single sprite?
Could we reset the timer instead of preventing a second pickup?
Why serialize the rotation as Vector3 instead of Quaternion?
What's localPosition / localRotation and how do they differ from position / rotation?

## 10.2) Hammer Animation

Create an animation for the hammer swinging.

Why use a 1:00, what if I want to speed up the animation?
How do keyframes work / what happens between keyframes?

## 10.3) Start Swinging on Equip

Add a script to the hammer to start the swing animation when it's equip.

Does the name matter?
How does animator.Play work?

## 10.4) Flash

Add a script to the Hammer to make it flash before it's gone.

# 11) Character Animations

Create a few animation for the Character.

## 11.1) Character Animation Parameters

Create parameters to use in the character's Animator Controller and a script to feed the data.

When do you use Animator Controller parameters vs Play(state) to change animations?
Why LateUpdate instead of Update or FixedUpdate?
What unit/scale is speed defined in?
How does the Multiplier Parameter work?

## 11.2) Jump Animation

Add an animation to the character for jumping.

Why transition from Any State instead of from CharacterWalk?
How do animation conditions work?
What does the Transition Duration impact?
What does the Can Transition to Self impact?
What does Exit Time do and how does it relate to conditions?

## 11.3) Climb Animation

Add an animation for when climbing ladders.

## 11.4) Idle Animation

Add an animation for while idle.

## 11.5) Breakdance Animation

Add an animation for the character to dance after idling for a while.

# 12) Intro Timeline

Create an animated sequence for the intro to level 1.

## 12.1) Intro Animation

Create an animation for the cloud entrance at the start of the level.

Create a timeline which enables the LevelController and Hammers after the intro is complete.

What is a Unity Timeline / Activation Track?
How might we do this without using the Timeline Editor?

## 12.2) Start Spawners After Intro

Disable the spawners and create a script to later enable them when the level intro completes.

What is a BasicPlayableBehaviour / when is OnBehaviourPlay called?
What's a C# 'enum'?

## 12.3) Rotate Platforms

Platforms start out straight and then when the intro animation is nearly complete, shake down into position.

What's C# yield break do?

## 12.4) Screen Shake

Shake the screen when the platforms fall into place.

What is the script doing with percent complete?
What does Random.insideUnitCircle do?
What else could we add to the shake effect?

# 13) Level Won Timeline

Create an animated sequence to play when the player beats level 1.

## 13.1) Win Condition

The goal of the game is to save the beautiful mushroom. For level 1, that means getting close - but before you actually reach it the EvilCloud is going to carry the mushroom up to level 2.

Here we detect the end of the game, the cloud animation will be added later in the tutorial.

## 13.2) Win Timeline

When the character reaches the win area, play a Timeline to animate the end of the level.

Why switch the Playable when editing Timelines?

## 13.3) Stop Everything When the Level is Over

When the level is over, stop the spawners and freeze the character and enemies while the win timeline plays.

Why not just set timeScale to 0?
Why not just destroy all the components instead?
What's rigidbody simulated=false do?
What's the lock symbol do?

## 13.4) Transition Scenes

After the level ends, load level 2.

Why not use just one scene for the game?
What's SceneManager.LoadScene do?

# 14) UI

Create a menu and in game UI for points and lives. Add scene transitions so the game may play multiple times.

## 14.1) Points

Display the number of points in the top right.

What's a canvas do and why is our level so small in comparison?
Why size the font too large and then scale it down?
What is a RectTransform, how does it differ from a Transform?
Why use ceiling here?
What does setting the anchor point / pivot on UI do?
What's C# ToString("N0") do?

## 14.2) Lives

Add sprites to display how many lives remain.

How does the HorizontalLayoutGroup work?
Why an Image and not a Sprite?
What does Child Force Expand Width?

## 14.3) Main Menu

Create a main menu to show at the start of the game.

Does order matter for scenes in the Build Settings?
How does the Canvas Scaler / Scale with Screen Size work?

## 14.4) Start Level 1

Allow the player to start Level 1 from the menu.

Why Remove Component instead of disable it?
How do UI events / button OnClick events work?

## 14.5) Play Again

When the game is over, return to the menu and allow the player to play again.

# 15) To Review, Level 2

## 15.1) Design Level 2

Create level 2 reusing a lot from level 1 but change various configurations such as having the cloud spawn HoverGuy enemies.

The win condition for this level, to be added later, will be to jump over each of the breakaway platforms we add here.

What does it mean to 'Break the prefab instance'?
When freezing the rigidbody a warning appears, why do it this way?

## 15.2) Level 2 Timelines

## 15.3) Breakaway Blocks

Once the character has touched each of the Breakaway platforms, make the level collapse.

## 15.4) GG screen

Design a scene to display when the player wins the game. Here we drop a bunch of random "GG"s from the sky.

Why a TextMesh instead of UI Text?
TODO why not use component based mechanics?