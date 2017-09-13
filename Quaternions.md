[HardlyDifficult.com/Tutorials/Quaternions](https://HardlyDifficult.com/Tutorials/Quaternions)

# Intro to Quaternion Rotations (with Unity 2017)

[View on YouTube](TODO) 

Goal: This tutorial aims to introduce working with rotations in Unity, with a focus on Quaternions.  Some math that goes into Quaternions is included. It may help to explain what these numbers represent, but it's not necessary to know when working in Unity. By the end, you should feel comfortable using Quaternions in Unity.  

 - 1.) Euler Rotations
   - 1.1) [About Euler](#11-about-euler)
   - 1.2) [Gimbal lock](#12-gimbal-lock)
   - 1.3) [Working with Euler](#13-working-with-euler)
 - 2.) Axis-Angle Rotations
   - 2.1) [About Axis-Angle](#21-about-axis-angle)
   - 2.2) [Working with Axis-Angle](#22-working-with-axis-angle)
 - 3.) Quaternion Rotations
   - 3.1) [About Quaternions](#31-about-quaternions)
   - 3.2) Creating Quaternions  
     - 3.2.1) [Quaternion Constructors](#321-quaternion-constructors)
     - 3.2.2) [Quaternion.LookRotation](#322-quaternionlookrotation)
     - 3.2.3) [Quaternion.FromToRotation](#323-quaternionfromtorotation)
     - 3.2.4) [Math for Creating Quaternions](#324-math-for-creating-quaternions)
   - 3.3) Interpolating Rotations
     - 3.3.1) [Quaternion.Lerp](#331-quaternionlerp)
     - 3.3.2) [Quaternion.Slerp](#332-quaternionslerp)
     - 3.3.3) [Quaternion.RotateTowards](#333-quaternionrotatetowards)
     - 3.3.4) [Math for Quaternion Lerp](#334-math-for-quaternion-lerp)
   - 3.4) Combining Rotations
     - 3.4.1) [Quaternion * Quaternion](#341-quaternion--quaternion)
     - 3.4.2) [Math for Quaternion/Quaternion Multiplication](#342-math-for-quaternionquaternion-multiplication)
   - 3.5) Inverse Rotation
     - 3.5.1) [Quaternion.Inverse](#351-quaternioninverse)
     - 3.5.2) [Math for Quaternion Inverse](#352-math-for-quaternion-inverse)
   - 3.6) Rotating Vectors
     - 3.6.1) [Quaternion * Vector3 (or Vector2)](#361-quaternion--vector3-or-vector2)
     - 3.6.2) [Math for Quaternion/Vector3 Multiplication](#362-math-for-quaternionvector3-multiplication)
   - 3.7) Comparing Rotations
     - 3.7.1) [Dot Product / Quaternion.Dot](#371-dot-product--quaterniondot)
     - 3.7.2) [Quaternion.Angle](#372-quaternionangle)
     - 3.7.3) [Quaternion == Quaternion](#373-quaternion--quaternion)
     - 3.7.4) [Math for Quaternion Dot](#374-math-for-quaternion-dot)

## 1) Euler Rotations

### 1.1) About Euler

When we think of rotations, we typically think in terms of 'Euler' (pronounced oi-ler).  Euler rotations are degrees of rotation around each axis; e.g., (0, 0, 30) means "rotate the object by 30 degrees around the Z axis."

In the Inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quaternions directly, or use Euler (or other representation) and then convert it back to Quaternion for storage.

### 1.2) Gimbal lock

The main reason that Euler is not the primary way of storing and manipulating rotations in a game is because of issues which arise from "Gimbal lock".

Gimbal lock is a situation in which 2 of the rotation axes collapse, effectively representing the same movement.  This means instead of the usual 3 degrees of freedom (x, y, and z), you only have two.

Here is an example.  Once an object reaches 90 degrees on the X axis, the Y and Z axes collapse, and modifying either produces the same results (where a change to Y is the same as a negative change to Z).

<img src=https://i.imgur.com/pWILGUW.gif width=500px>

[View source for this example](https://github.com/hardlydifficult/EduQuaternions/blob/master/Assets/GimbalLockAnimation.cs).

Gimbal lock is not an all-or-nothing situation. As you approach certain angles, the impact of changing axes may not offer the full range of motion that you might expect.

Note that Euler can represent any possible rotation.  Gimbal lock is only a concern when modifying or combining rotations.

For a lot more detail - see [Wikipedia's article on Gimbal Lock](https://en.wikipedia.org/wiki/Gimbal_lock) or [GuerrillaCG's video on Gimbal Lock](https://www.youtube.com/watch?v=zc8b2Jo7mno&feature=youtu.be&t=176).

### 1.3) Working with Euler

Given a Quaternion, you can calculate the Euler value like so:

```csharp
Quaternion myRotationQuat = transform.rotation;
Vector3 eulerRotation = myRotationQuat.eulerAngles;
```

Euler rotations are stored as a Vector3.  You can perform any of the operations you might use on a position Vector3 such as +, *, and Lerp.  Then, given an Euler value, you can calculate the Quaternion:

```csharp
Quaternion z30Degrees = Quaternion.Euler(eulerRotation);
```

## 2) Axis-Angle Rotations

### 2.1) About Axis-Angle

Another way of representing rotations is Axis-Angle.  This approach defines both an axis and the angle defining how much to rotate around that axis.

Here is a simple example where we are rotating around the X axis only.  When the axis is one of the world axes like this, the angle is equivalent to an Euler angle.

<img src=https://i.imgur.com/YPelrfF.gif width=500px>

[View source for this example](https://github.com/hardlydifficult/EduQuaternions/blob/master/Assets/AxisAngleAnimation.cs) and the one below.

The following example shows a more complex rotation where the axis is not aligned with a world axis. 

 - It's hard to see with this render, but in the perspective on the right the red axis line is not just straight up and down, but also angled from front to back.
 - The bottom two perspectives show the same rotation ,but with a straight-on view of the axis itself.

<img src=https://i.imgur.com/5zCrTdn.gif width=500px>

Axis-Angle and other rotation approaches, including Quaternions and Matrices, are not impacted by Gimbal Lock. 

### 2.2) Working with Axis-Angle

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

## 3) Quaternion Rotations

### 3.1) About Quaternions

A Quaternion is an axis-angle representation scaled in a way which optimizes common calculations, such as combining multiple rotations and interpolating between different rotation values.

The default rotation for an object known as 'identity' is (0, 0, 0) in Euler and (0, 0, 0, 1) in Quaternion.  If you multiply a rotation by identity, the rotation does not change.

### 3.2) Properties of a Quaternion

Quaternion rotations must be normalized, meaning:

```csharp
x * x + y * y + z * z + w * w == 1;
```

Knowing the Quaternion rotations are normalized simplifies some of the math for using and manipulating Quaternions.

Quaternions are composed of 4 floats, like an Axis-Angle.  The first three (x, y, z) are logically grouped into a vector component of the Quaternion and the last value (w) is a scalar component.  Some of the math below shows how these parts may be considered separately.

The performance Quaternions offer comes with a small cost in terms of storage.  A rotation technically has 3 degrees of freedom, which means that it may be represented with 3 floats (like an Euler); however, a Quaternion requires 4 floats.  This tradeoff has been deemed worthwhile by the industry for the performance when a game is running.  If size matters, such as for network communication, quaternions may be compressed as well as an Euler could be.

### 3.2) Creating Quaternions

#### 3.2.1) Quaternion Constructors

In Unity, rotations are stored as Quaternions. You can construct a Quaternion from the calculated components.

```csharp
Quaternion identity = new Quaternion(0, 0, 0, 1);
```

Generally, you would not use the Quaternion constructor. Selecting the values for x, y, z, w to create the rotation you are looking for is difficult for people to do.

Often, rotations are created as Euler and then converted to Quaternion.  Then, Quaternions are used to modify other Quaternions using the techniques covered later in this tutorial. See the Euler and Axis-Angle sections above for examples on how-to convert rotation formats.

For the 'identity' rotation, instead of using the Quaternion constructor, you should use the Quaternion.identity variable:

```csharp
Quaternion rotation = Quaternion.identity;
```

Note that the 'default' Quaternion is not a valid rotation, and may not be used with any rotation method:

```csharp
Quaternion invalidQuaternion = default(Quaternion);
// invalidQuaternion == new Quaternion(0, 0, 0, 0) 
// This is not normalized, therefore not a valid quaternion
```

#### 3.2.2) Quaternion.LookRotation

LookRotation creates a rotation which will orient an object so that its forward will face the target forward direction and its up will face the target up direction.  The up direction defaults to the world's positive Y direction, but you could change this; for example, making it the negative Y direction to rotate an object upside down.

In the following example (code followed by gif), an object is rotated so that it's always facing away from the camera (since the camera defaults to a negative Z position in the world, it is behind objects by default).

```csharp
Vector3 directionToCamera
  = Camera.main.transform.position - transform.position;
transform.rotation
  = Quaternion.LookRotation(-directionToCamera, Vector3.up);
```

Note that the input directions do not need to be normalized.

<img src=https://i.imgur.com/nK9ijDJ.gif width=300px>

[View source for this example](https://github.com/hardlydifficult/EduQuaternions/blob/master/Assets/LookRotation.cs).

#### 3.2.3) Quaternion.FromToRotation

FromToRotation creates a rotation which would modify a Vector's direction so that after the rotation the Vector is facing the given target direction.  In the following example, we rotate an object so that its 'back' direction faces the camera (creating the same effect as the example above).

```csharp
Vector3 directionToCamera
  = Camera.main.transform.position - transform.position;
transform.rotation = Quaternion.FromToRotation(
  Vector3.back, directionToCamera);
```

Note that the input directions do not need to be normalized.  

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

### 3.3) Interpolating Rotations

#### 3.3.2) Quaternion.Slerp

Slerp, or **s**pherical **l**inear int**erp**olation, is a fancy term for a simple concept.  If you were to smoothly/evenly rotate from rotation A to B, slerp is the formula that calculates the interim rotation given a percent progress from 0 to 1, named 't'.  For example:

```csharp
transform.rotation = Quaternion.Slerp(
    transform.rotation, 
    targetRotation, 
    percentComplete);
```

Another way of leveraging the Slerp method is by using it in an update loop and providing the same constant for 't' each frame instead of using a percent complete.  Each frame it will close a percent of the remaining gap, this will create a motion that slows the closer it is to the target rotation.

```csharp
transform.rotation = Quaternion.Slerp(
  transform.rotation, 
  Quaternion.identity, 
  speed * Time.deltaTime);
```

The following is an example of the two different ways of leveraging 't' in slerp:

<img src=https://i.imgur.com/Mlaxbvo.gif width=500>

[View source for this example](https://github.com/hardlydifficult/EduQuaternions/blob/master/Assets/Lerp.cs) and the Slerp example below.

The performance of Slerp is almost on-par with Lerp.  We tested running Slerp or Lerp 10k times per frame in Unity and there was no measurable difference between them.

#### 3.3.1) Quaternion.Lerp

Lerp, or **l**inear int**erp**olation, for rotations is very similar to Slerp.  It follows a straight line between rotations instead of curving to ensure a constant angular velocity like Slerp does.

You can use Slerp the exact same way you use Lerp.  For example:

```csharp
transform.rotation = Quaternion.Lerp(
    transform.rotation, 
    targetRotation, 
    percentComplete);
```

The following example shows two objects, one which is rotating with Lerp (blue) and the other with Slerp (red).  Note that they exactly the same at the start, middle, and end; and there is very little different in between.

<img src=https://i.imgur.com/hfmmzoh.gif width=500>

See also [Higeneko's Slerp vs Lerp visualization]( https://www.youtube.com/watch?v=uNHIPVOnt-Y).

#### 3.3.3) Quaternion.RotateTowards

RotateTowards is an alternative to Slerp/Lerp for selecting a rotation between two other rotations.  RotateTowards uses a fixed rotation speed instead of rotating by percent (like Slerp and Lerp).

You can use RotateTowards like you use Slerp and Lerp; however, instead of specifying 't' you are providing a speed which is equal to the max degrees the object may rotate this frame.

```csharp
transform.rotation = Quaternion.RotateTowards(
    transform.rotation, 
    targetRotation, 
    speed);
```

To help clarify some use case differences between each of these interpolation options:

 - Use RotateTowards when you want to rotate with a fixed angular velocity.  
 - Use Slerp with t = percentComplete when you want the rotation to complete in a fixed amount of time.
 - Use Slerp with t = constant when you want the rotation to start fast and slow down as it approaches the target rotation.
 - Consider using Lerp over Slerp when you need some acceleration and deceleration at the start/end to smooth the experience Slerp offers.   

#### 3.3.4) Math for Quaternion Lerp

In Unity, you should use the method above.  However, for the interested, below is how the lerp may be calculated.

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

```csharp
Quaternion rotation = parentRotation * childRotation;
```

You can use multiplication to combine any number of rotations (e.g., grandparent * parent * child).

When combining rotations, a parent GameObject may rotate the parent and a child, and then the child could add an additional rotation of its own. With Quaternions, you write the multiplication such that the parent comes before the child.  Order matters, as shown in this example:

<img src=https://i.imgur.com/dO5omUB.gif width=500px>

[View source for this example](https://github.com/hardlydifficult/EduQuaternions/blob/master/Assets/MirrorRotation.cs) and the next.

#### 3.4.2) Math for Quaternion/Quaternion Multiplication

In Unity, you should use the method above.  However, for the interested, below is how multiplication may be calculated.

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

### 3.5) Inverse Rotation

#### 3.5.1) Quaternion.Inverse

The inverse of a rotation is the opposite rotation; if you apply a rotation and then apply the inverse of that rotation, it results in no change.

<img src=https://i.imgur.com/F6kNDmJ.gif width=300px>

```csharp
Quaternion inverseRotation = Quaternion.Inverse(rotation);
```

#### 3.5.2) Math for Quaternion Inverse 

In Unity, you should use the method above.  However, for the interested, below is how the inverse may be calculated.

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

Given a vector, you can calculate its position after a rotation has been applied.  For example, given an offset from the center, you can rotate to orbit around that center point.

In Unity, you can simply use the multiplication symbol, for example:

```csharp
Quaternion rotation = ...;
Vector3 offsetPosition = ...; 
transform.position = rotation * offsetPosition;
```

You must have the Quaternion before the Vector for multiplication (i.e., offsetPosition * rotation does not work).

<img src=https://i.imgur.com/SjxHgY1.gif width=300px>

[View source for this example](https://github.com/hardlydifficult/EduQuaternions/blob/master/Assets/RotateVertex.cs).
 

#### 3.6.2) Math for Quaternion/Vector3 Multiplication

In Unity, you should use the method above.  However, for the interested, below is how multiplication may be calculated.

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

Dot product is a fast operation which informs you how well-aligned two rotations are to each other.  A dot product of 1 means the two rotations are identical, and -1 means they are oriented in opposite directions.  

The dot product does not include direction.  For example, a value of .9 tells you that you are nearly facing the same direction, but does not give you enough information to rotate closer to 1.

```csharp
float dot = Quaternion.Dot(a, b);
```

#### 3.7.2) Quaternion.Angle

Angle returns the difference between two rotations in degrees.  This is very similar to the information you get from the Dot product, but returned in degrees, which may be useful for some scenarios.

```csharp
float angle = Quaternion.Angle(a, b);
```

#### 3.7.3) Quaternion == Quaternion

The equals operator (operator==) uses the dot product to test if two rotations are nearly identical.  

```csharp
if(transform.rotation == Quaternion.identity) 
...
```

Note that in general, using "==" is not recommended when floats are involved as tiny rounding issues may result in differences which have no impact on the game.  Unity has addressed this concern in a custom operator== method for Quaternions, so that "==" is safe to use.

#### 3.7.4) Math for Quaternion Dot

In Unity, you should use the method above.  However, for the interested, below is how the dot product may be calculated.

```csharp
float dot = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
```

# GG

That's all for now.  Questions, issues, or suggestions?  Please use the [YouTube comments](TODO).

Support on [Patreon](https://www.patreon.com/HardlyDifficult), with [Paypal](https://u.muxy.io/tip/HardlyDifficult), or by subscribing on [Twitch](https://www.twitch.tv/HardlyDifficult/subscribe) (free with Amazon Prime).
 
[License](https://creativecommons.org/licenses/by/4.0/). Created live at [twitch.tv/HardlyDifficult](https://www.twitch.tv/HardlyDifficult) August 2017.  