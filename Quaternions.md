# Intro to Quaternions 

[View on YouTube](TODO) | [Source code of the examples below](TODO)

Goal: This tutorial aims to introduce working with rotations in Unity, with a focus on Quaternions.  By the end you should feel comfortable working with Quaternions in Unity and we will introduce some of the math that goes into them so that it does not feel like black magic anymore.  

 - 1.) [Euler](#1-euler)
   - 1.1) [About Euler Rotations](#11-about-euler-rotations)
   - 1.1) [Gimbal lock](#11-gimbal-lock)
   - 1.2) [Working with Euler in Unity](#12-working-with-euler-in-unity)
 - 2.) [Axis-Angle](#2-axis-angle)
   - 2.1) [About Axis-Angle](#21-about-axis-angle)
   - 2.2) [Working with Axis-Angle in Unity](#21-working-with-axis-angle-in-unity)
 - 3.) [Quaternion](#3-quaternion)
   - 3.1) [About Quaternion Rotations](#31-about-quaternion-rotations)
   - 3.2) [Creating Quaternions](#33-creating-quaternions)   
     - 3.2.1) [Creating Quaternions in Unity](#31-working-with-quaternions-in-unity)
     - 3.2.2) [Math for Creating Quaternions](#32-math-for-creating-quaternions)
   - 3.3) [Lerp](#33-lerp)
     - 3.3.1) [About Lerp]()
     - 3.3.2) [Lerp in Unity]()
     - 3.3.3) [Math for Quaternion Lerp]()
   - 3.3) [Slerp](#33-lerp)
     - 3.3.1) [About Slerp]()
     - 3.3.2) [Slerp in Unity]()
     - 3.3.3) [Math for Quaternion Slerp]()
   - 3.4) [Combining Rotations (Quaternion Multiplication)](#34-combining-rotations-quaternion-multiplication)
   - 3.5) [Inverse](#35-inverse)
   - 3.6) [Rotating Vectors](#36-rotating-vectors)
   - 3.7) [Dot Product](#37-dot-product)

## 1) Euler

### 1.1) About Euler Rotations

When we think of rotations, we typically think in terms of 'Euler' (pronounced oi-ler).  Euler rotations are degrees of rotation around each axis; e.g., (0, 0, 30) means "rotate the object by 30 degrees around the Z axis."

In the Inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quaternions directly, or use Euler (or other representation) and then convert it back to Quaternion for storage.

### 1.2) Gimbal lock

The main reason that Euler is not the primary way of storing and manipulating rotations in a game is because of issues which arise from "Gimbal lock".

Gimbal lock is a situation when 2 of the rotation axes collapse, effectively representing the same movement.  This means instead of the usual 3 degrees of freedom (x, y, and z) you only have two.

Here is an example.  Once an object reaches 90 degrees on the X axis, the Y and Z axes collapse and modifying either produces the same results (where a change to Y is the same as a negative change to Z).

<img src=https://i.imgur.com/uUxzimi.gif width=500px>

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

<img src=https://i.imgur.com/NhEjxZd.gif width=500px>

The following example shows a more complex rotation where the axis is not aligned with a world axis. 

 - It's hard to see with this render, but in the perspective on the right the red axis line is not just straight up and down but also angled from front to back.
 - The bottom two perspectives show the same rotation but with a straight on view of the axis itself.

<img src=https://i.imgur.com/9jPicRb.gif width=500px>

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

Quaternions are composed of 4 floats, like an Axis-Angle.  The first three (x, y, z) are logically grouped into a vector component of the Quaternion and the last value (w) is the scalar component.

### 3.1) Working with Quaternions in Unity

In Unity, all rotations are stored as Quaternions.  You may prefer working with another rotation format in code and then need to convert to/from Quaternions.  See the Euler and Axis-Angle sections above for examples.

You may also construct a Quaternion from the calculated components.

```csharp
Quaternion rotation = new Quaternion(0, 0, 0, 1);
```

Generally you would not use the Quaternion constructor as selecting the values for x, y, z, w to create the rotation you are looking for is difficult for people to do.  

Often rotations are created as Euler and then converted to Quaternion.  Then Quaternions are used to modify other Quaternions using the techniques covered later in this tutorial.  You can also create Quaternions using:

 - [FromToRotation](https://docs.unity3d.com/ScriptReference/Quaternion.FromToRotation.html) creates a rotation which rotates from one direction to another direction.
 - [LookRotation](https://docs.unity3d.com/ScriptReference/Quaternion.LookRotation.html) creates a rotation which will orient an object to have the given forward and up directions.  

The default rotation for an object, known as 'identity', is (0, 0, 0) in Euler and (0, 0, 0, 1) in Quaternion.  

```csharp
Quaternion rotation = Quaternion.identity;
```

The performance Quaternions offer come with a small cost in terms of storage.  A rotation technically has 3 degrees of freedom which means that it may be represented with 3 floats (like an Euler) however a Quaternion requires 4 floats.  This tradeoff has been deemed worthwhile by the industry.  If size matters, such as for network communication, quaternions may be compressed as well as an Euler could be.

### 3.2) Math for Creating Quaternions

Here is the formula for Quaternion, given an axis-angle rotation.  You don't need to know this when working in Unity.

```csharp
// Given an Axis-Angle rotation
Vector3 axis;
float angle;
transform.rotation.ToAngleAxis(out angle, out axis);
angle *= Mathf.Deg2Rad;

// Create a Quaternion
Quaternion rotation = new Quaternion(
  axis.x * Mathf.Sin(angle / 2),
  axis.y * Mathf.Sin(angle / 2),
  axis.z * Mathf.Sin(angle / 2),
  Mathf.Cos(angle / 2));
```

### 3.3) Lerp

Lerp, or **l**inear int**erp**olation, is a fancy term for a simple concept.  If you were to smoothly/evenly rotate from rotation A to B, lerp is the formula that calculates the interim rotation given a percent progress from 0 to 1.  For example:

```csharp
transform.rotation = Quaternion.Lerp(
    transform.rotation, 
    targetRotation, 
    percentComplete);
```

In Unity, you should use the method above.  However for the interested, below is how the lerp may be calculated.

TODO do we use the Vector parts?

```csharp
// Define terms
Quaternion a = transform.rotation;
Quaternion b = targetRotation;
// Calculate target quaternion values
float x = (1 - percentComplete) * a.x + percentComplete * b.x;
float y = (1 - percentComplete) * a.y + percentComplete * b.y;
float z = (1 - percentComplete) * a.z + percentComplete * b.z;
float w = (1 - percentComplete) * a.w + percentComplete * b.w;
// Normalize results
float factor = Mathf.Sqrt(x * x + y * y + z * z + w * w);
x /= factor;
y /= factor;
z /= factor;
w /= factor;
// Update the rotation to the lerped value
transform.rotation = new Quaternion(x, y, z, w);
```

Quaternions are normalized, meaning:

```csharp
x * x + y * y + z * z + w * w == 1;
```

When a lerp calculation is performed, the resulting values need to be normalized again.

[Slerp](https://docs.unity3d.com/ScriptReference/Quaternion.Slerp.html), a similar but more difficult formula, can also be calculated on Quaternions but left out from this tutorial for simplicity.

[RotateTowards](https://docs.unity3d.com/ScriptReference/Quaternion.RotateTowards.html) is an alternative to Lerp for selecting a rotation between two other rotations.  Lerp will progress based off of the percent progress and RotateTowards will progress using a fixed rotation speed.

### 3.4) Combining Rotations (Quaternion Multiplication)

Often you need to combine rotations.  With Quaternions this is done with multiplication.

When combining rotations, order matters.  For example a parent GameObject may rotate the parent and a child and then the child could add an additional rotation of its own. With Quaternions you write the multiplication such that the parent comes before the child.  

<img src=https://i.imgur.com/6ydPWQp.gif width=500px>

```csharp
Quaternion rotation = parentRotation * childRotation;
```

You can use multiplication to combine any number of rotations (e.g. grandparent * parent * child).

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
Quaternion targetRotation;
Vector3 targetVector = parentScalar * childVector
  + childScalar * parentVector
  + Vector3.Cross(parentVector, childVector);
float targetScalar = parentScalar * childScalar
  - Vector3.Dot(parentVector, childVector);
targetRotation = new Quaternion(
  targetVector.x, targetVector.y, targetVector.z, targetScalar);
```

### 3.5) Inverse 

The inverse of a rotation is the opposite rotation.  So if you apply a rotation and then apply the inverse of that rotation it results in no change.

<img src=https://i.imgur.com/O6lHMMb.gif width=500px>

```csharp
Quaternion inverseRotation = Quaternion.Inverse(rotation);
```

In Unity, you should use the method above.  However for the interested, below is how multiplication may be calculated.

```csharp
// Split the Quaternion components
Vector3 vector = new Vector3(
    rotation.x, rotation.y, rotation.z);
float scalar = rotation.w;

// Calculate inverse
vector = -vector;

// Return results
Quaternion inverseRotation = new Quaternion(vector.x, vector.y, vector.z, scalar);
```

### 3.6) Rotating Vectors

Given a vector you can calculate its position after a rotation has been applied.  For example, given vertex of a mesh you can calculate what its position would be after a rotation.

<img src=https://i.imgur.com/0YIicjK.gif width=500px>

In Unity, you can simply use the multiplication symbol (Quaternion * Vector), for example:

```csharp
transform.position = rotation * originalPosition;
```

You must have the Quaternion before the Vector for multiplication (i.e. originalPosition * rotation does not work). 

In Unity, you should use the method above.  However for the interested, below is how multiplication may be calculated.

```csharp
// Prep for calculations
Quaternion positionQuaternion = new Quaternion(
    position.x, position.y, position.z, 0);
Quaternion inverseRotation = Quaternion.Inverse(rotation);

// Calculate new position
Quaternion newPositionQuaternion 
    = rotation * positionQuaternion * inverseRotation;
Vector3 newPosition = new Vector3(
    newPositionQuaternion.x, newPositionQuaternion.y, newPositionQuaternion.z);
```

The approach above creates a Quaternion for the position simple to enable the multiplication operations required.  Its possible to implement this algorithm without reusing the Quaternion data structure in this way.


### 3.7) Dot Product

Dot product is a fast operation which informs you how well aligned two rotations are to each other.  A dot product of 1 means the two rotations are identical and -1 means they are oriented in opposite directions.  

Note that the dot product does not include direction.  e.g. a value of .9 tells you that you are nearly facing the same direction but does not provide enough data for you to rotate closer to 1.

```csharp
float dot = Quaternion.Dot(a, b);
```

In Unity, you should use the method above.  However for the interested, below is how the dot product may be calculated.

```csharp
float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
```

 - [Angle](https://docs.unity3d.com/ScriptReference/Quaternion.Angle.html) is very similar to Dot product returning the difference between two rotations in degrees.
 - [operator==](https://docs.unity3d.com/ScriptReference/Quaternion-operator_eq.html) uses the dot product to test if two rotations are nearly identical.



<br><hr>

Questions, issues, or suggestions?  Please use the [YouTube comments](TODO).

Support on [Patreon](https://www.patreon.com/HardlyDifficult), with [Paypal](https://u.muxy.io/tip/HardlyDifficult), or by subscribing on [Twitch](https://www.twitch.tv/HardlyDifficult/subscribe) (free with Amazon Prime).
 
[License](TODO). Created live at [twitch.tv/HardlyDifficult](https://www.twitch.tv/HardlyDifficult) August 2017.  