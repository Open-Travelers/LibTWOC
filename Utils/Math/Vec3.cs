
namespace LibTWOC.Utils.Math
{
    public struct Vec3 : ISerializableData
    {
        // Convenience constants
        public static Vec3 Zero = new Vec3(0, 0, 0);
        public static Vec3 UnitX = new Vec3(1, 0, 0);
        public static Vec3 UnitY = new Vec3(0, 1, 0);
        public static Vec3 UnitZ = new Vec3(0, 0, 1);
        
        // Fields
        public float X, Y, Z;
        public float Length { get { return (float)System.Math.Sqrt(this.DotProduct(this)); } }

        // Constructors
        public Vec3(Vec3 copy) => 
            (X, Y, Z) = (copy.X, copy.Y, copy.Z);
        public Vec3(float x, float y, float z) =>
            (X, Y, Z) = (x, y, z);
        
        // Overrides
        public override string ToString() => $"Vec3 {{ {X}, {Y}, {Z} }}";
        public override bool Equals(object obj)
        {
            if (obj is Vec3 p)
            {
                return (X, Y, Z) == (p.X, p.Y, p.Z);
            }
            else return false;
        }
        public override int GetHashCode() => (3, X, Y, Z).GetHashCode();
        
        // Operators
        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vec3 operator *(Vec3 a, float b)
        {
            return new Vec3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vec3 operator /(Vec3 a, float b)
        {
            return new Vec3(a.X / b, a.Y / b, a.Z / b);
        }
       
        // Static math methods
        public static float DotProduct(Vec3 a, Vec3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static Vec3 CrossProduct(Vec3 a, Vec3 b)
        {
            return new Vec3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }
        public static Vec3 Transform(Vec3 source, Mat4x4 mat)
        {
            return new Vec3(
                mat[0] * source.X + mat[1] * source.Y + mat[2] * source.Z + mat[3],
                mat[4] * source.X + mat[5] * source.Y + mat[6] * source.Z + mat[7],
                mat[8] * source.X + mat[9] * source.Y + mat[10] * source.Z + mat[11]
            );
        }
        public static Vec3 Normalize(Vec3 vec)
        {
            return vec / vec.Length;
        }

        // Static math member convenience methods
        public float DotProduct(Vec3 b)
        {
            return DotProduct(this, b);
        }
        public Vec3 CrossProduct(Vec3 b)
        {
            return CrossProduct(this, b);
        }
        public Vec3 Transform(Mat4x4 mat)
        {
            return Transform(this, mat);
        }
        public Vec3 Normalize()
        {
            return Normalize(this);
        }

        // Serialization methods
        public void Read(BinaryReaderExt br)
        {
            X = br.ReadF32();
            Y = br.ReadF32();
            Z = br.ReadF32();
        }
        public void Write(BinaryWriterExt bw)
        {
            bw.WriteF32(X);
            bw.WriteF32(Y);
            bw.WriteF32(Z);
        }

    }
}
