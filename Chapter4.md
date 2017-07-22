Feet

<details><summary>How does Dot product work?</summary>

The Dot product is a fast operation which can be used to effeciently determine if two directions represented with Vectors are facing the same (or a similiar) way.

In the visualization below, we are rotating two ugly arrows.  These arrows are pointing in a direction and we are using Vector2.Dot to compare those two directions.  The Dot product is shown as we rotate around.

<img src="http://i.imgur.com/XrjcWQm.gif" width=200px />

A few notables about Dot products:

 - '1' means the two directions are facing the same way.
 - '-1' means the two directions are facing opposite ways.
 - '0' means the two directions are perpendicular.
 - Numbers smoothly transition between these points, so .9 means that the two directions are nearly identical.
 - When two directions are not the same, the Dot product will not tell you which direction an object should rotate in order to make them align - it only informs you about how similar they are at the moment.  

For this visualization, we are calculating the Dot product like so:

```csharp
Vector2.Dot(gameObjectAToWatch.transform.up, gameObjectBToWatch.transform.up);
```

</details>

## Prevent double jump


## Rotate to match the floor's angle
(or with jumping)

Create a Feet with isGrounded and Quaternion floorRotation. 
Create a RotateToMatchFloorWhenGrounded

## Ladders

LadderMovement, for character and spike ball.


Fly Guy too
 - Prevent walking into walls?


win condition

end of level / respawn and scene changes

## SuicideWhenPlayerDies.
(or with end of level sequence)

In game UI

ui...

points

=====

Level 2

..

DieOnBumpers?
