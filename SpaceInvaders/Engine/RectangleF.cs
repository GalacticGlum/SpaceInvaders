/*
 * Author: Shon Verch
 * File Name: RectangleF.cs
 * Project Name: SpaceInvaders
 * Creation Date: 02/10/2019
 * Modified Date: 02/24/2019
 * Description: An axis-aligned, four sided, two dimensional box defined by a
 *              top-left position and a size supporting floating point precision.
 */

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace SpaceInvaders.Engine
{
    /// <summary>
    /// An axis-aligned, four sided, two dimensional box defined by a top-left position
    /// and a size supporting floating point precision.
    /// </summary>
    [DataContract]
    public struct RectangleF
    {
        /// <summary>
        /// A <see cref="RectangleF"/> with it's position and size set to 0.0.
        /// </summary>
        public static RectangleF Empty { get; } = new RectangleF();

        /// <summary>
        /// The x-coordinate of the top-left corner of this <see cref="RectangleF"/>.
        /// </summary>
        [DataMember]
        public float X;

        /// <summary>
        /// The y-coordinate of the top-left corner of this <see cref="RectangleF"/>.
        /// </summary>
        [DataMember]
        public float Y;

        /// <summary>
        /// The width of this <see cref="RectangleF"/>.
        /// </summary>
        [DataMember]
        public float Width;

        /// <summary>
        /// The height of this <see cref="RectangleF"/>.
        /// </summary>
        [DataMember]
        public float Height;

        /// <summary>
        /// The x-coordinate of the left edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Left => X;

        /// <summary>
        /// The x-coordinate of the right edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        /// The y-coordinate of the top edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Top => Y;
        
        /// <summary>
        /// The y-coordinate of the bottom edge of this <see cref="RectangleF"/>.
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        /// Gets a boolean indicating whether this <see cref="RectangleF"/> is empty;
        /// an empty <see cref="RectangleF"/> has all values equal to 0.0.
        /// </summary>
        public bool IsEmpty => Width == 0 && Height == 0 && X == 0 && Y == 0;

        /// <summary>
        /// The position of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// The size of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        /// <summary>
        /// The centre position of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 Centre => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

        /// <summary>
        /// The top-left position of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 TopLeft => new Vector2(X, Y);

        /// <summary>
        /// The top-right position of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 TopRight => new Vector2(X + Width, Y);

        /// <summary>
        /// The bottom-left position of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 BottomLeft => new Vector2(X, Y + Height);

        /// <summary>
        /// The bottom-right position of this <see cref="RectangleF"/>.
        /// </summary>
        public Vector2 BottomRight => new Vector2(X + Width, Y + Height);

        /// <summary>
        /// Initializes a new <see cref="RectangleF"/> from the specified position and size scalars.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new <see cref="RectangleF"/> from the specified position and size vectors.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        public RectangleF(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        /// <summary>
        /// Determines whether the two specified <see cref="RectangleF"/> intersect.
        /// </summary>
        /// <param name="a">The first <see cref="RectangleF"/>.</param>
        /// <param name="b">The second <see cref="RectangleF"/>.</param>
        /// <returns>
        /// A boolean value indicating whether the two rectangles intersect.
        /// Value <c>true</c> if they do intersect; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(ref RectangleF a, ref RectangleF b) => 
            a.X < b.X + b.Width && a.X + a.Width > b.X &&
            a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;

        /// <summary>
        /// Determines whether the specified <see cref="RectangleF"/> intersects with this <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">The other <see cref="RectangleF"/>.</param>
        /// <returns>
        /// A boolean value indicating whether the two rectangles intersect.
        /// Value <c>true</c> if they do intersect; otherwise, <c>false</c>.
        /// </returns>
        public bool Intersects(RectangleF other) => Intersects(ref this, ref other);

        /// <summary>
        /// Determines whether the specified <see cref="RectangleF" /> contains the specified <see cref="Vector2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        /// A boolean value indicating whether the <see cref="RectangleF"/> contains the point.
        /// Value <c>true</c> if it does; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(ref RectangleF rectangle, ref Vector2 point) => 
            rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width &&
            rectangle.Y <= point.Y && point.Y < rectangle.Y + rectangle.Height;

        /// <summary>
        /// Determines whether the specified <see cref="RectangleF" /> contains the specified <see cref="Vector2" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        /// A boolean value indicating whether the <see cref="RectangleF"/> contains the point.
        /// Value <c>true</c> if it does; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(RectangleF rectangle, Vector2 point) =>
            Contains(ref rectangle, ref point);

        /// <summary>
        /// Determines whether this <see cref="RectangleF" /> contains the specified <see cref="Vector2"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        /// A boolean value indicating whether this <see cref="RectangleF"/> contains the point.
        /// Value <c>true</c> if it does; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Vector2 point) => Contains(ref this, ref point);

        /// <summary>
        /// Determines whether the two specified <see cref="RectangleF"/> are equal.
        /// </summary>
        /// <param name="a">The first <see cref="RectangleF"/>.</param>
        /// <param name="b">The second <see cref="RectangleF"/>.</param>
        /// <returns>
        /// A boolean value indicating whether the two rectangles are equal.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(RectangleF a, RectangleF b) => a.Equals(b);

        /// <summary>
        /// Determines whether the two specified <see cref="RectangleF"/> are not equal.
        /// </summary>
        /// <param name="a">The first <see cref="RectangleF"/>.</param>
        /// <param name="b">The second <see cref="RectangleF"/>.</param>
        /// <returns>
        /// A boolean value indicating whether the two rectangles are not equal.
        /// Value <c>true</c> if they are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(RectangleF a, RectangleF b) => !(a == b);

        /// <summary>
        /// Determines whether this <see cref="RectangleF"/> is equal to another <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">The other <see cref="RectangleF"/>.</param>
        /// <returns>
        /// A boolean value indicating whether this rectangle is equal to another.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RectangleF other) => Equals(ref other);

        /// <summary>
        /// Determines whether this <see cref="RectangleF"/> is equal to another <see cref="RectangleF"/>.
        /// </summary>
        /// <param name="other">The other <see cref="RectangleF"/>.</param>
        /// <returns>
        /// A boolean value indicating whether this rectangle is equal to another.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref RectangleF other) =>
            X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

        /// <summary>
        /// Determines whether this <see cref="RectangleF"/> is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A boolean value indicating whether this rectangle is equal to the specified object.
        /// Value <c>true</c> if they are equal; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is RectangleF f && Equals(f);

        /// <summary>
        /// Returns a hash code of this <see cref="RectangleF"/>.
        /// </summary>
        /// <returns>A hash code of this <see cref="RectangleF"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Converts a <see cref="Rectangle"/> to a <see cref="RectangleF"/>. 
        /// </summary>
        /// <param name="rectangle">The rectangle to convert.</param>
        public static implicit operator RectangleF(Rectangle rectangle)
        {
            return new RectangleF
            {
                X = rectangle.X,
                Y = rectangle.Y,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }

        /// <summary>
        /// <para>
        /// Converts a <see cref="RectangleF"/> to a <see cref="Rectangle"/>.
        /// </para>
        /// A loss of precision may occur due to conversion from floating-point values to integer values.
        /// </summary>
        /// <param name="rectangle">The rectangle to convert.</param>
        public static explicit operator Rectangle(RectangleF rectangle) =>
            new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);

        /// <summary>
        /// Converts this <see cref="RectangleF"/> to a string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }
}
