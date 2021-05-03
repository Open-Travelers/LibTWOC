
namespace LibTWOC.Utils.Math
{
    public struct Vec2 : ISerializableData
    {
        // Convenience constants
        public static Vec2 Zero = new Vec2(0, 0);
        public static Vec2 UnitX = new Vec2(1, 0);
        public static Vec2 UnitY = new Vec2(0, 1);

        // Fields
        public float X, Y;
        public float Length { get { return (float)System.Math.Sqrt(this.DotProduct(this)); } }

        // Constructors
        public Vec2(Vec2 copy) =>
            (X, Y) = (copy.X, copy.Y);
        public Vec2(float x, float y) => 
            (X, Y) = (x, y);

        // Overrides
        public override string ToString() => $"Vec2 {{ {X}, {Y} }}";
        public override int GetHashCode() => (2, X, Y).GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is Vec2 p)
            {
                return (X, Y) == (p.X, p.Y);
            }
            else return false;
        }

        // Operators
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }
        public static Vec2 operator *(Vec2 a, float b)
        {
            return new Vec2(a.X * b, a.Y * b);
        }
        public static Vec2 operator /(Vec2 a, float b)
        {
            return new Vec2(a.X / b, a.Y / b);
        }

        // Static math methods
        public static float DotProduct(Vec2 a, Vec2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        public static Vec2 Normalize(Vec2 vec)
        {
            return vec / vec.Length;
        }
        
        // Static math member convenience methods
        public float DotProduct(Vec2 b)
        {
            return DotProduct(this, b);
        }
        public Vec2 Normalize()
        {
            return Normalize(this);
        }

        public void Read(BinaryReaderExt br)
        {
            X = br.ReadF32();
            Y = br.ReadF32();
        }

        public void Write(BinaryWriterExt bw)
        {
            bw.WriteF32(X);
            bw.WriteF32(Y);
        }
    }
}
