# Intro to Quaternion Rotations (with Unity 2017)

TODO get a friendly URL.

[View on YouTube](TODO) | [Source code of the examples below](TODO)

Goal: This tutorial aims to introduce working with rotations in Unity, with a focus on Quaternions.  By the end you should feel comfortable working with Quaternions in Unity and we will introduce some of the math that goes into them so that it does not feel like black magic anymore.  

 - 1.) [Euler Rotations](#1-euler)
   - 1.1) [About Euler](#11-about-euler-rotations)
   - 1.1) [Gimbal lock](#11-gimbal-lock)
   - 1.2) [Working with Euler](#12-working-with-euler-in-unity)
 - 2.) [Axis-Angle Rotations](#2-axis-angle)
   - 2.1) [About Axis-Angle](#21-about-axis-angle)
   - 2.2) [Working with Axis-Angle](#21-working-with-axis-angle-in-unity)
 - 3.) [Quaternion Rotations](#3-quaternion)
   - 3.1) [About Quaternions](#31-about-quaternion-rotations)
   - 3.2) [Creating Quaternions](#33-creating-quaternions)   
     - 3.2.1) [Quaternion Constructors]()
     - 3.2.2) [Quaternion.LookRotation]()
     - 3.2.3) [Quaternion.FromToRotation]()
     - 3.2.4) [Math for Constructing Quaternions](#32-math-for-creating-quaternions)
   - 3.3) [Interpolation Rotations](#33-interpolation-lerp-slerp-movetowards)
     - 3.3.1) [Quaternion.Lerp]()
     - 3.3.2) [Quaternion.Slerp]()
     - 3.3.3) [Quaternion.RotateTowards]()
     - 3.3.4) [Math for Quaternion Lerp]()
   - 3.4) [Combining Rotations](#34-combining-rotations-quaternion-multiplication)
     - 3.4.1) [Quaternion * Quaternion]()
     - 3.4.2) [Math for Quaternion/Quaternion Multiplication]()
   - 3.5) [Inverse](#35-inverse)
     - 3.5.1) [Quaternion.Inverse]()
     - 3.5.2) [Math for Quaternion Inverse]()
   - 3.6) [Rotating Vectors](#36-rotating-vectors)
     - 3.4.1) [Quaternion * Vector3 (or Vector2)]()
     - 3.4.1) [Math for Quaternion/Vector3 Multiplication]()
   - 3.7) [Comparing Rotations]()
     - 3.7.1) [Dot Product / Quaternion.Dot](#37-dot-product)
     - 3.7.2) [Quaternion.Angle](#37)
     - 3.7.3) [Quaternion == Quaternion](#37)
     - 3.7.4) [Math for Quaternion Dot]()

## 1) Euler

### 1.1) About Euler Rotations

When we think of rotations, we typically think in terms of 'Euler' (pronounced oi-ler).  Euler rotations are degrees of rotation around each axis; e.g., (0, 0, 30) means "rotate the object by 30 degrees around the Z axis."

In the Inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quaternions directly, or use Euler (or other representation) and then convert it back to Quaternion for storage.

### 1.2) Gimbal lock

The main reason that Euler is not the primary way of storing and manipulating rotations in a game is because of issues which arise from "Gimbal lock".

Gimbal lock is a situation when 2 of the rotation axes collapse, effectively representing the same movement.  This means instead of the usual 3 degrees of freedom (x, y, and z) you only have two.

Here is an example.  Once an object reaches 90 degrees on the X axis, the Y and Z axes collapse and modifying either produces the same results (where a change to Y is the same as a negative change to Z).

<img src=https://i.imgur.com/pWILGUW.gif width=500px>

[View source for this example](TODO).

Gimbal lock is not an all or nothing situation. As you approach certain angles the impact of changing axes may not offer the full range of motion you might expect.

Note that Euler can represent any possible rotation.  Gimbal lock is only a concern when modifying or combining rotations.

For a lot more detail - see [Wikipedia's article on Gimbal Lock](https://en.wikipedia.org/wiki/Gimbal_lock) or [GuerrillaCG's video on Gimbal Lock](https://www.youtube.com/watch?v=zc8b2Jo7mno&feature=youtu.be&t=176).

### 1.3) Working with Euler in Unity

Given a Quaternion, you can calculate the Euler value like so:

```csharp
Quaternion myRotationInQuaternion = transform.rotation;
Vector3 myRotationInEuler = myRotationInQuaternion.eulerAngles;
```

Euler rotations are stored as a Vector3.  You can perform any of the operations you might use on a position Vector3 such as +, *, and Lerp.  Then given an Euler value, you can calculate the Quaternion:

```csharp
Quaternion rotationOfZ30Degrees = Quaternion.Euler(0, 0, 30);
```

## 2) Axis-Angle

### 2.1) About Axis-Angle

Another way of representing rotations is Axis-Angle.  This approach defines an axis to rotate around and the angle defining how much to rotate.

Here is a simple example where we are rotating around the X axis only.  When the axis is one of the world axes like this, the angle is equivalent to an Euler angle.

<img src=https://i.imgur.com/YPelrfF.gif width=500px>

[View source for this example](TODO) and the one below.

The following example shows a more complex rotation where the axis is not aligned with a world axis. 

 - It's hard to see with this render, but in the perspective on the right the red axis line is not just straight up and down but also angled from front to back.
 - The bottom two perspectives show the same rotation but with a straight on view of the axis itself.

<img src=https://i.imgur.com/5zCrTdn.gif width=500px>

Axis-Angle and other rotation approaches including Quaternions and Matrices are not impacting by Gimbal Lock. 

### 2.2) Working with Axis-Angle in Unity

Given a Quaternion, you can calculate the Axis-Angle value like so:

```csharp
float angle;
Vector3 axis;
transform.rotation.ToAngleAxis(out angle, out axis);
```

You could modify the angle or even the axis itself.  Then given an Axis-Angle value, you can calculate the Quaternion:

```csharp
Quaternion rotation = Quaternion.AngleAxis(angle, axis);
```

## 3) Quaternion

### 3.1) About Quaternion Rotations

A Quaternion is an axis-angle representation scaled in way which optimizes common calculations such as combining multiple rotations and interpolating between different rotation values.

Quaternions are composed of 4 floats, like an Axis-Angle.  The first three (x, y, z) are logically grouped into a vector component of the Quaternion and the last value (w) is the scalar component.  In some of the math below, you'll see the implications of logically separating the components like this.

Quaternion rotations must be normalized, meaning:

```csharp
x * x + y * y + z * z + w * w == 1;
```

Knowing the Quaternion rotations are normalized simplifies some of the math for using and manipulating Quaternions shown below.

The default rotation for an object (i.e. a rotation that would have no impact on object when applied) , known as 'identity', is (0, 0, 0) in Euler and (0, 0, 0, 1) in Quaternion.  

The performance Quaternions offer come with a small cost in terms of storage.  A rotation technically has 3 degrees of freedom which means that it may be represented with 3 floats (like an Euler) however a Quaternion requires 4 floats.  This tradeoff has been deemed worthwhile by the industry for the performance when a game is running.  If size matters, such as for network communication, quaternions may be compressed as well as an Euler could be.

### 3.2) Creating Quaternions

#### 3.2.1) Quaternion Constructor in Unity

In Unity, all rotations are stored as Quaternions.  You may prefer working with another rotation format in code and convert to or from Quaternions as needed.  See the Euler and Axis-Angle sections above for examples on converting rotation formats.

You may also construct a Quaternion from the calculated components.

```csharp
Quaternion identity = new Quaternion(0, 0, 0, 1);
```

Generally you would not use the Quaternion constructor.  Selecting the values for x, y, z, w to create the rotation you are looking for is difficult for people to do.  Often rotations are created as Euler and then converted to Quaternion.  Then, Quaternions are used to modify other Quaternions using the techniques covered later in this tutorial. 

For identity, instead of using the constructor you can use the Quaternion.identity variable:

```csharp
Quaternion rotation = Quaternion.identity;
```

Note that the 'default' Quaternion is not a valid rotation and may not be used with any rotation method:

```csharp
Quaternion invalidQuaternion = default(Quaternion);
// invalidQuaternion == new Quaternion(0, 0, 0, 0) 
// This is not normalized, therefor not a valid quaternion
```

#### 3.2.2) Quaternion.LookRotation in Unity

LookRotation creates a rotation which will orient an object so that its forward will face the target forward direction and its up will face the target up direction.  The up direction defaults to the world's positive Y direction but you could change this, for example making it the negative Y direction to rotate an object upside down.

In the following example (code followed by gif), an object is rotated so that it's always facing away from the camera (since the camera defaults to a negative Z position in the world it is behind objects by default).

```csharp
Vector3 directionToCamera
  = Camera.main.transform.position - transform.position;
transform.rotation
  = Quaternion.LookRotation(-directionToCamera, Vector3.up);
```

Note that the input directions do not need to be normalized.

<img src=https://i.imgur.com/nK9ijDJ.gif width=300px>

[View source for this example](TODO).

#### 3.2.3) Quaternion.FromToRotation in Unity

FromToRotation creates a rotation which would modify a Vector's direction so that after the rotation the Vector is facing the given target direction.  In the following example, we rotate an object so that its 'back' direction faces the camera (creating the same effect as the example above).

```csharp
Vector3 directionToCamera
  = Camera.main.transform.position - transform.position;
transform.rotation = Quaternion.FromToRotation(
  Vector3.back, directionToCamera);
```

Note that the input directions do not need to be normalized.  Later in this tutorial we cover Quaternion multiplication.


#### 3.2.4) Math for Creating Quaternions

Here is the formula for Quaternion, given an axis-angle rotation.  You don't need to know this when working in Unity.

```csharp
// Given an Axis-Angle rotation
Vector3 axis;
float angle;
transform.rotation.ToAngleAxis(out angle, out axis);
angle *= Mathf.Deg2Rad;

// Calculated the Quaternion components
Vector3 vectorComponent = axis * Mathf.Sin(angle / 2);
float scalarComponent = Mathf.Cos(angle / 2);

// Construct the result
Quaternion rotation = new Quaternion(
  vectorComponent.x,
  vectorComponent.y,
  vectorComponent.z,
  scalarComponent);
```

### 3.3) Interpolation (Lerp/Slerp/MoveTowards)

#### 3.3.1) Lerp

Lerp, or **l**inear int**erp**olation, is a fancy term for a simple concept.  If you were to smoothly/evenly rotate from rotation A to B, lerp is the formula that calculates the interim rotation given a percent progress from 0 to 1, named 't'.  For example:

```csharp
transform.rotation = Quaternion.Lerp(
    transform.rotation, 
    targetRotation, 
    percentComplete);
```

Another way of leveraging the Lerp method is by using it in an update loop and providing the same constant for 't' each frame instead of using a percent complete.  This will create a motion that slows the closer it is to the target rotation.

```csharp
transform.rotation = Quaternion.Lerp(
  transform.rotation, 
  Quaternion.identity, 
  speed * Time.deltaTime);
```

<img src=https://i.imgur.com/E5rwh3i.gif width=500>

[View source for this example](TODO).

#### 3.3.2) Slerp

Slerp, or **s**pherical **l**inear int**erp**olation, is very similar to lerp when interpolating rotations.  The following example shows two objects, one which is rotating with Lerp (blue) and the other with Slerp (red).

You can use Slerp the exact same way you use Lerp.  For example:

```csharp
transform.rotation = Quaternion.Slerp(
    transform.rotation, 
    targetRotation, 
    percentComplete);
```

<img src=https://i.imgur.com/Qu2wWvW.gif width=500>

[View source for this example](TODO).

#### 3.3.3) RotateTowards

RotateTowards is an alternative to Lerp/Slerp for selecting a rotation between two other rotations.  RotateTowards uses a fixed rotation speed instead of rotating by percent (like Lerp and Slerp).

You can use RotateTowards like you use Lerp and Slerp, however instead of specifying 't' you are providing a speed which is the max degrees the object may rotate this frame.

```csharp
transform.rotation = Quaternion.RotateTowards(
    transform.rotation, 
    targetRotation, 
    speed);
```

To help clarify some use case differences between each of these interpolation options:

 - Use RotateTowards when you want to rotate with a fixed angular velocity.  
 - Use Lerp with t = percentComplete when you want the rotation to complete in a fixed amount of time.
 - Use Lerp with t = constant when you want the rotation to start fast and slow down as it approaches the target rotation.
 - Use Slerp over Lerp when you need some acceleration and deceleration at the start/end to smooth the experience Lerp offers.  Note that Slerp is computationally much more expensive.


#### 3.3.4) Math for Quaternion Lerp

In Unity, you should use the method above.  However for the interested, below is how the lerp may be calculated.

```csharp
// Define terms
Quaternion a = transform.rotation;
Quaternion b = targetRotation;

// Split the Quaternion components
Vector3 aVector = new Vector3(a.x, a.y, a.z);
float aScalar = a.w;
Vector3 bVector = new Vector3(b.x, b.y, b.z);
float bScalar = b.w;

// Calculate target quaternion values
Vector3 targetVector = (1 - percentComplete) * aVector + percentComplete * bVector;
float targetScalar = (1 - percentComplete) * aScalar + percentComplete * bScalar;

// Normalize results
float factor = Mathf.Sqrt(targetVector.sqrMagnitude + targetScalar * targetScalar);
targetVector /= factor;
targetScalar /= factor;

// Update the rotation to the lerped value
transform.rotation = new Quaternion(
  targetVector.x, targetVector.y, targetVector.z, targetScalar);
```

When a lerp calculation is performed, the values need to be normalized so that the resulting Quaternion is normalized.

### 3.4) Combining Rotations 

#### 3.4.1) Quaternion * Quaternion

Often you need to combine rotations.  With Quaternions this is done with multiplication.

When combining rotations, a parent GameObject may rotate the parent and a child, and then the child could add an additional rotation of its own. With Quaternions you write the multiplication such that the parent comes before the child.  Order matters as shown in this example:

<img src=https://i.imgur.com/LwyP3vz.gif width=500px>

[View source for this example](TODO).

```csharp
Quaternion rotation = parentRotation * childRotation;
```

You can use multiplication to combine any number of rotations (e.g. grandparent * parent * child).

#### 3.4.2) Math for Quaternion Multiplication

In Unity, you should use the method above.  However for the interested, below is how multiplication may be calculated.

```csharp
// Split the Quaternion components
Vector3 parentVector = new Vector3(
  parentRotation.x, parentRotation.y, parentRotation.z);
float parentScalar = parentRotation.w;

Vector3 childVector = new Vector3(
  childRotation.x, childRotation.y, childRotation.z);
float childScalar = childRotation.w;

// Calculate parentRotation * childRotation
Vector3 targetVector = parentScalar * childVector
  + childScalar * parentVector
  + Vector3.Cross(parentVector, childVector);
float targetScalar = parentScalar * childScalar
  - Vector3.Dot(parentVector, childVector);

// Store result
Quaternion targetRotation = new Quaternion(
  targetVector.x, targetVector.y, targetVector.z, targetScalar);
```

### 3.5) Inverse 

#### 3.5.1) Quaternion.Inverse

The inverse of a rotation is the opposite rotation; if you apply a rotation and then apply the inverse of that rotation, it results in no change.

<img src=https://i.imgur.com/gLsG1OQ.gif width=300px>

[View source for this example](TODO).

```csharp
Quaternion inverseRotation = Quaternion.Inverse(rotation);
```

#### 3.5.2) Math for Quaternion Inverse 

In Unity, you should use the method above.  However for the interested, below is how the inverse may be calculated.

```csharp
// Split the Quaternion components
Vector3 vector = new Vector3(
    rotation.x, rotation.y, rotation.z);
float scalar = rotation.w;

// Calculate inverse
vector = -vector;

// Store results
Quaternion inverseRotation = new Quaternion(
  vector.x, vector.y, vector.z, scalar);
```

### 3.6) Rotating Vectors

#### 3.6.1) Quaternion * Vector3 (or Vector2)

Given a vector you can calculate its position after a rotation has been applied.  For example, given an offset from the center you can rotate to orbit around that center point.

<img src=https://i.imgur.com/LAV5HN8.gif width=300px>

In Unity, you can simply use the multiplication symbol, for example:

```csharp
Quaternion rotation = ...;
Vector3 offsetPosition = ...; 
transform.position = rotation * offsetPosition;
```

You must have the Quaternion before the Vector for multiplication (i.e. offsetPosition * rotation does not work). 

#### 3.6.2) Math for Quaternion/Vector3 Multiplication

In Unity, you should use the method above.  However for the interested, below is how multiplication may be calculated.

```csharp
// Prep for calculations
Quaternion positionQuaternion = new Quaternion(
    position.x, position.y, position.z, 0);
Quaternion inverseRotation = Quaternion.Inverse(rotation);

// Calculate new position
Quaternion newPositionQuat 
    = rotation * positionQuaternion * inverseRotation;

// Store result
Vector3 newPosition = new Vector3(
    newPositionQuat.x, newPositionQuat.y, newPositionQuat.z);
```

The approach above creates a Quaternion for the position simply to enable the multiplication operations required.  It's possible to implement this algorithm without reusing the Quaternion data structure in this way.


### 3.7) Comparing Rotations

#### 3.7.1) Dot Product / Quaternion.Dot

Dot product is a fast operation which informs you how well aligned two rotations are to each other.  A dot product of 1 means the two rotations are identical and -1 means they are oriented in opposite directions.  

Note that the dot product does not include direction.  e.g. a value of .9 tells you that you are nearly facing the same direction but does not give you enough information to rotate closer to 1.

```csharp
float dot = Quaternion.Dot(a, b);
```

#### 3.7.2) Quaternion.Angle

Angle returns the difference between two rotations in degrees.  This is very similar to the information you get from the Dot product, but returned in degrees which may be useful for some scenarios.

```csharp
float angle = Quaternion.Angle(a, b);
```

#### 3.7.3) Quaternion == Quaternion

The equals operator (operator==) uses the dot product to test if two rotations are nearly identical.  Any data structure which uses floats should not use exact comparisons as rounding issues may result in very tiny differences which have no impact on how the game is rendered, hence the 'nearly'.


```csharp
if(transform.rotation == Quaternion.identity) 
...
```

#### 3.7.4) Dot Product / Quaternion.Dot

In Unity, you should use the method above.  However for the interested, below is how the dot product may be calculated.

```csharp
float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
```

# GG

That's all for now.  Questions, issues, or suggestions?  Please use the [YouTube comments](TODO) or stop by [twitch.tv/HardlyDifficult].

Support on [Patreon](https://www.patreon.com/HardlyDifficult), with [Paypal](https://u.muxy.io/tip/HardlyDifficult), or by subscribing on [Twitch](https://www.twitch.tv/HardlyDifficult/subscribe) (free with Amazon Prime).
 
[License](https://creativecommons.org/licenses/by/4.0/). Created live at [twitch.tv/HardlyDifficult](https://www.twitch.tv/HardlyDifficult) August 2017.  