# Quaternions

TODO

## Euler

When we think of rotations, we typically think in terms of 'Euler' (pronounced oi-ler) rotations.  Euler rotations are degrees of rotation around each axis; e.g., (0, 0, 30) means "rotate the object by 30 degrees around the Z axis."

In the inspector, modifying a Transform's rotation is done in Euler.  In code, you can either work with Quaternions directly, or use Euler and then convert it back to Quaternion for storage.

### Gimbal lock

The main reason that euler is not the primary way of storing and manipulating rotations in a game is because of issues which arise from "Gimbal lock".

Gimbal lock is a situation when 2 of the rotation axes collapse, effectively representing the same movement.  This means instead of the usual 3 degrees of freedom (x, y, and z) you only have two.

Here is an example.  Once an object reaches 90 degrees on the X axis the Y and Z axes collapse and modifying either produces the same results (where a change to Y is the same as a negative change to Z).

<img src=https://i.imgur.com/N155axb.gif width=500px>

Gimbal lock is not an all or nothing situation. As you approach certain angles the impact of changing axes may not offer the full range of motion you might expect.

Note that euler can represent any possible rotation.  Gimbal lock is only a concern when modifying or combining rotations.

For a lot more detail - see [Wikipedia's article on Gimbal Lock](https://en.wikipedia.org/wiki/Gimbal_lock).

The other rotation approaches below do not have a Gimbal Lock issue.

For more, see [GuerrillaCG's video on Gimbal Lock](https://www.youtube.com/watch?v=zc8b2Jo7mno&feature=youtu.be&t=176).

## Axis-Angle

TODO

## Quaternion

A Quaternion is an axis-angle representation which is scaled in way which optimizes common calculations, for example:

 - Quaternions can combine multiple rotations efficiently.
 - Linear interpolation (i.e. Lerp) between rotations is very fast.

The performance Quaternions offer come with a small cost in terms of storage.  A rotation technically has 3 degrees of freedom which means that it may be represented with 3 floats (like an Euler) however a Quaternion requires 4 floats.  This tradeoff has been deemed worthwhile by the industry.  If size matters, such as for network communication, quaternions may be compressed as well as an Euler could be.

normalized.

This is the formula for Quaternion, given an axis-angle rotation.

```csharp
Vector3 axis;
float angle;

Quaternion rotation = new Quaternion(
    axis.x * Mathf.Sin(angle/2),
    axis.y * Mathf.Sin(angle/2),
    axis.z * Mathf.Sin(angle/2),
    Mathf.Cos(angle/2)
)
```


### Lerp


```csharp
Quaternion rotation = Quaternion.Lerp(a, b, percentComplete);
```

```csharp
float x = (1 - percentComplete) * a.x + percentComplete * b.x;
float y = (1 - percentComplete) * a.y + percentComplete * b.y;
float z = (1 - percentComplete) * a.z + percentComplete * b.z;
float w = (1 - percentComplete) * a.w + percentComplete * b.w;
float factor = Mathf.Sqrt(x * x + y * y + z * z + w * w);
x /= factor;
y /= factor;
z /= factor;
w /= factor;
Quaternion rotation = new Quaternion(x, y, z, w);
```

Slerp, a similar but more difficult formula, can also be calculated on Quaternions but left out from this tutorial for simplicity.

### Combining Rotations (Quaternion Multiplication)

Often you need to combine rotations.  With Quaternions this is done with multiplication.

When combining rotations, order matters.  For example a parent GameObject may rotate the parent and a child and then the child could add an additional rotation of its own.  With Quaternions you write the multiplication in reverse, where the last term is the rotation which is conceptually added first.

```csharp
Quaternion parentRotation;
Quaternion childRotation;

Quaternion rotation = childRotation * parentRotation;
```

Math...

### Rotating Vectors

To rotate a vector:

```csharp

```

Inverse 
