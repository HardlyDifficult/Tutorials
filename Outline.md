1 A game with enemies spawning

2D project config, sprites, editor script, collider, rigidbody, MonoBehaviour, prefab, Destroy, Instantiate, Coroutines, Collision Matrix.

1.1) Start a 2D project and import assets

 - Can I Unity 5.* or Monodevelop instead?
 - Whats the difference between 2D and 3D?
 - What's a scene?
 - Can I name folders differently?
 - What's a sprite / sprite sheet?
 - Who made the art for this tutorial?
 - Can I use my own art?

1.4) Configure sprites

 - What is mesh type?
 - What is filter mode?

1.6) Disable Anti-Aliasing

 - What is Anti-Aliasing and why disable it?
 - Why do we need to change this setting multiple times?
 - Why not update the camera instead?

1.7) Select an aspect ratio

 - Why use a fixed aspect ratio

1.8) Configure camera

 - Why change 'Size' and not camera position?

1.11) Add an auto save script

 - What's an editor script / why is the folder name important?
 - What's InitializeOnLoad?
 - What's a C# attribute?
 - What's a C# static constructor?
 - What's a C# delegate?
 - What about performance?

1.12) Add a platform to the scene

 - What's a GameObject, Transform, and Component?
 - What's a SpriteRenderer?
 - What's tiling do and why not use Transform scale?

1.14) Create platform with rounded edges

 - Why create a parent GameObject?
 - How is the sprite position calculated when it's a child?

1.16) Create two connected platforms

 - Why not use a single GameObject for this bottom platform?
 - Why extend the platform beyond the edge of the screen?

1.17) Complete level 1 platform layout

1.18) Add colliders and effectors

 - What is a Collider?
 - Why not place colliders on the child GameObjects instead?
 - What are Effectors?
 - What does Surface Arc do and why not use a value of 1?

1.19) Create a SpikeBall

 - Why use a parent here?
 - What's a Rigidbody2D?
 - Why shrink the collider?

1.23) Add invisible bumpers

1.24) Add starting momentum to the ball

 - Does the filename matter?
 - What is MonoBehaviour / how is Start() called?
 - What's a Vector2 and how's it differ from Vector3?
 - What's GetComponent do / what's C# generics?
 - What's RequireComponent do?
 - What's velocity and angularVelocity?
 - What's SerializeField and why not use public instead?
 - Why use protected on the Unity event?
 - Why confirm the values in the Inspector match the defaults in code?
 - Why not use a "SpikeBall" component instead?
 - What does Debug.Assert do?

1.25) Destroy balls that roll off

 - Why hardcode the outOfBoundsYPosition?
 - Why bother, the GameObject is already off screen?
 - What is Destroy and why not Destroy(this)?
 - What about an object pool?

1.26) Spawn balls from an EvilCloud

 - What's a prefab?
 - What does changing scale Z do?
 - What is StartCoroutine / WaitForSeconds?
 - What does Instantiate do?
 - What's Random.Range do / what other options do we have for RNG?
 - What's a Quaternion?

1.30) Disable collisions between enemies

 - What does the collision matrix impact?
 - What's a Layer and how's it different from a Tag?

<hr>
2 Walk, jump, and die

2.2) Create an animated character

 - What's Pivot do?
 - What's the difference between Animation and Animator?
 - How do I know what size to make the collider?
 - Why not use a collider that outlines the character?
 - Why freeze rotation and does freezing mean it can never change?

2.5) Add a script to move left & right

 - What's a controller? Why not read input here?
 - Why set velocity instead of using AddForce?
 - Why FixedUpdate instead of Update?
 - Why multiply by Time.fixedDeltaTime?

2.6) Add a player controller

 - What is an Input 'Axis' and how are they configured?
 - Why not use a bool or Enum for Left/Right instead of a float?

2.7) Jump

 - Why AddForce here instead of velocity and what's 'Impulse'?
 - How do you know when to use Update vs FixedUpdate for Input and rigidbodies?
 - Why is AudioSource on a GameObject vs just playing clips?
 - Would two separate player controllers be a better component based solution?

4.1) Flip entities when they walk the other way

 - What's a C# smart property?
 - Why not compare to 0 when checking if there is no movement?

3.10) Fade in

 - What does StopAllCoroutines do?

2.11) Create a pattern for death effects

 - Why not just play effects OnDestroy()?
 - Why is there a public method that 'should not be called directly'?
 - Why are you using Mathf and not System.Math?
 - What does GetComponentsInChildren do?
 - What's C# abstract do and how's it different from an interface?

2.12) Kill the player when he hits a ball

 - Why check the layer instead of using the Collision Matrix?
 - What is this '& 1 <<' black magic?
 - What is a C# extension method?
 - Why is there an empty Start method and why check if enabled?

2.13) Create an explosion prefab with sound effect

 - What's a particle / particle system?
 - Could you RNG select the audio clip to play?

2.14) Spawn and destroy explosion

 - What's bounds represent?
 - Why not spawn the explosion at transform.position instead of bounds.center?
 - Why bother destroying, the explosion is not visible after a few seconds?

2.17) Animate characters death

 - What does yield return null do?
 - Why not use an animation instead?
 - Why use Mathf.Sin?
 - How might we disable movement when the character is dying?

3.4) Spawn in a hovering enemy

 - How do you know what size to make the second collider?
 - Why use a child GameObject instead of two colliders on the parent?

3.5) Make the HoverGuy walk

 - Why use timeBeforeFirstWander instead of RNG from the start?
 - Why not set desiredWalkDirection to a random value instead of 1 or -1?

<hr>
3 Triggers, collisions, and raycasts

Triggers, singleton game controller, OverlapCollider, Raycast

3.1) Create a Hammer

 - Why use pivot bottom?
 - Why use a polygon collider and not a box or capsule?
 - Why use Is Trigger?
 - Why not simply sum the time used in WaitForSeconds instead of max with deltaTime?
 - Why use GetComponentsInChildren instead of a single sprite?

3.2) Equip the hammer

 - Could we reset the timer instead of preventing a second pickup?
 - Why serialize the rotation as Vector3 instead of Quaternion?
 - What's localPosition / localRotation and how do they differ from position / rotation?

3.11) Create a GameController

 - What does DontDestroyOnLoad do?
 - What's a singleton and why use it?

3.8) Restrict movement to stay on screen

 - Does setting the position cause the entity to pop on-screen?
 - Why use bounds for these checks?
 - What's the difference between setting transform.position and using myBody.MovePosition?
 - Why use an event when another component could check screen bounds again?

3.9) HoverGuy turns around when reaching the edge

3.13) Clear and restart the level on death

 - Why does position before saving the prefab matter?
 - Why not use an interface instead of abstract?
 - What does FindObjectsOfType do?
 - Why not have all objects subscribe to life count changes instead or this new pattern?

3.15) Prevent enemies spawning on top of the character

 - What does OverlapCollider do?
 - Why use a contact filter instead of a tag or a layermask?
 - Does the Collision Matrix impact anything when using OverlapCollider?
 - Why use a temp collider list?

3.16) Add points for jumping over enemies

 - What's Raycast do?
 - Why Trigger AND Raycast?
 - Why add another Rigidbody2D / why check the collision layer manually?
 - Do we need a cooldown?
 - Why FixedUpdate instead of Update here?

<hr>
4 Ground detection & Ladders


4.2) Detect floors

 - What's a C# Nullable type / what's the question mark after 'float'?
 - What's C# 'is' do and how's it differ from 'as'?
 - What's Dot product do?
 - When do you use OverlapCollider vs Raycast vs Distance vs Trigger*?
 - Why add the edge radius to bounds max when calculating the floor's position?

4.3) Prevent double jump

 - Why not use a cooldown instead?

4.4) Update wander to prefer traveling up hill

 - Why take the Dot product with Vector2.right?

4.5) Rotate so feet are flat on the floor

 - What's 'Lerp' and how's it compare to 'Slerp'?

4.6) Add ladders to the world

 - Why use sqrMagnitude instead of magnitude?

4.9) Change layers while climbing

 - What's a C# List?
 - What's rigidbody gravityScale do?
 - Why store the impacted collider list?

4.10) Random climb controller

 - If I set both odds to 50%, why does it go up more often then down?

4.11) Stop walking and rolling off ladders

 - Why not stop the WalkMovement component instead?
 - Why not deregister events here?
 - Why not stop and restart the coroutine instead?

4.13) Move towards the center of the ladder

 - Why not use velocity to move?

<hr>

5) Animations

5.1) Hammer animation

 - Why use a 1:00, what if I want to speed up the animation?
 - How do keyframes work / what happens between keyframes?

5.3) Start swinging hammer on equip

 - How does animator.Play work?

5.4) Character animation parameters

 - When do you use Animator Controller parameters vs Play(state) to change animations?
 - Why LateUpdate instead of Update or FixedUpdate?
 - What unit/scale is speed defined in?
 - How does the Multiplier Parameter work?

5.7) Jump animation

 - Why transition from Any State instead of from CharacterWalk?
 - How do animation conditions work?
 - What does the Transition Duration impact?
 - What does the Can Transition to Self impact?
 - What does Exit Time do and how does it relate to conditions?

5.8) Additional character animations

5.12) Add an intro timeline

 - What is a Unity Timeline / Activation Track?
 - How might we do this without using the Timeline Editor?

5.13) Disable spawners till the intro is complete

 - What is a BasicPlayableBehaviour / when is OnBehaviourPlay called?
 - What's a C# 'enum'?

5.14) Rotate platforms during intro

 - What's C# yield break do?

5.15) Add screen shake during intro

 - What is the script doing with percent complete?
 - What does Random.insideUnitCircle do?
 - What else could we add to the shake effect?

Add a win condition

Win animation

 - Why switch the Playable when editing Timelines?

Stop everything when the level is over

 - Why not just set timeScale to 0?
 - Why not just destroy all the components instead?
 - What's rigidbody simulated=false do?
 - What's the lock symbol do?

 <hr>
 6 UI and Scene transitions

 - Why not use just one scene for the game?
 - What's SceneManager.LoadScene do?

UI for points

 - What's a canvas do and why is our level so small in comparison?
 - Why size the font too large and then scale it down?
 - What is a RectTransform, how does it differ from a Transform?
 - Why use ceiling here?
 - What does setting the anchor point / pivot on UI do?
 - What's C# ToString("N0") do?

UI for lives

 - How does the HorizontalLayoutGroup work?
 - Why an Image and not a Sprite?
 - What does Child Force Expand Width?

Main menu

 - Does order matter for scenes in the Build Settings?
 - Why Remove Component instead of disable it?
 - How does the Canvas Scaler / Scale with Screen Size work?
 - How do UI events / button OnClick events work?

Level 2

 - What does it mean to 'Break the prefab instance'?
 - When freezing the rigidbody a warning appears, why do it this way?

Level 2 breakaway sequence

GG screen

 - Why a TextMesh instead of UI Text?