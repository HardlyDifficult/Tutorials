# Quaternions

TODO

## Euler

When we think of rotations, we typically think in terms of 'Euler' (pronounced oi-ler) rotations.  Euler rotations are degrees of rotation around each axis; e.g., (0, 0, 30) means "rotate the object by 30 degrees around the Z axis."

In the inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quaternions directly, or use Euler and then convert it back to Quaternion for storage.

### Gimbal lock

The main reason that euler is not the primary way of storing and manipulating rotations in a game is because of issues which arise from "Gimbal lock".

Gimbal lock is a situation when 2 of the rotation axes collapse, effectively representing the same movement.  This means instead of the usual 3 degrees of freedom (x, y, and z) you only have two.

Here is an example.  Once an object reaches 90 degrees on the X axis the Y and Z axes collapse and modifying either produces the same results (where a change to Y is the same as a negative change to Z).

<img src=https://i.imgur.com/uUxzimi.gif width=500px>

Gimbal lock is not an all or nothing situation. As you approach certain angles the impact of changing axes may not offer the full range of motion you might expect.

Note that euler can represent any possible rotation.  Gimbal lock is only a concern when modifying or combining rotations.

For a lot more detail - see [Wikipedia's article on Gimbal Lock](https://en.wikipedia.org/wiki/Gimbal_lock).

For more, see [GuerrillaCG's video on Gimbal Lock](https://www.youtube.com/watch?v=zc8b2Jo7mno&feature=youtu.be&t=176).

## Axis-Angle

Another way of representing rotations is Axis-Angle.  This approach defines an axis for rotating around and the angle defining how much to rotate.

Here is a simple example where we are rotating around the X axis only.  When the axis is one of the world axes like this, the angle is equivalent to an Euler angle.

<img src=https://i.imgur.com/r90qOtA.gif width=500px>

The following example shows a more complex rotation where the axis is not aligned with a world axis. 

 - It's hard to see with this render, but in the perspective on the right the red axis line is not straight up and down.
 - The bottom two perspectives show the same rotation but with a straight on view of the axis itself.

<img src=https://i.imgur.com/zgK3H5j.gif width=500px>

Axis-Angle and other rotation approaches including Quaternions and Matrices are not impacting by Gimbal Lock.  The only downside to Axis-Angle is that it does not perform as well as Quaternions.

## Quaternion

A Quaternion is an axis-angle representation which is scaled in way which optimizes common calculations such as combining multiple rotations and interpolating between different rotation values.

This is the formula for Quaternion, given an axis-angle rotation.

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

### Lerp

Lerp, or **l**inear int**erp**olation, is a fancy term for a simple concept.  If you were to smoothly/evenly rotate from rotation A to B, lerp is the formula that calculates the interim rotation given a percent progress from 0 to 1.  For example:

```csharp
transform.rotation = Quaternion.Lerp(
    transform.rotation, 
    targetRotation, 
    percentComplete);
```

In Unity, you should use the method above.  However for the interested, below is how the lerp may be calculated.

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

Slerp, a similar but more difficult formula, can also be calculated on Quaternions but left out from this tutorial for simplicity.

### Combining Rotations (Quaternion Multiplication)

Often you need to combine rotations.  With Quaternions this is done with multiplication.

When combining rotations, order matters.  For example a parent GameObject may rotate the parent and a child and then the child could add an additional rotation of its own.  With Quaternions you write the multiplication in reverse, where the last term is the rotation which is conceptually added first.

// TODO visual for order matters if we can.

```csharp
Quaternion parentRotation;
Quaternion childRotation;

Quaternion rotation = childRotation * parentRotation;
```

```csharp
// Given two random rotations
Quaternion childRotation = Random.rotation;
Vector3 childVector = new Vector3(
  childRotation.x, childRotation.y, childRotation.z);
float childScalar = childRotation.w;

Quaternion parentRotation = Random.rotation;
Vector3 pVector = new Vector3(
  parentRotation.x, parentRotation.y, parentRotation.z);
float pScalar = parentRotation.w;

// Calculate childRotation * parentRotation
Quaternion targetRotation;
Vector3 targetVector = childScalar * pVector 
  + pScalar * childVector 
  + Vector3.Cross(childVector, pVector);
float targetScalar = childScalar * pScalar 
  - Vector3.Dot(childVector, pVector);
targetRotation = new Quaternion(
  targetVector.x, targetVector.y, targetVector.z, targetScalar);
```


### Rotating Vectors

TODO

Inverse 

TODO






The performance Quaternions offer come with a small cost in terms of storage.  A rotation technically has 3 degrees of freedom which means that it may be represented with 3 floats (like an Euler) however a Quaternion requires 4 floats.  This tradeoff has been deemed worthwhile by the industry.  If size matters, such as for network communication, quaternions may be compressed as well as an Euler could be.

