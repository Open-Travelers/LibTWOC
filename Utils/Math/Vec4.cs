namespace LibTWOC.Utils.Math
{
    // This doesn't need to be serializable, but I wanted it to be parallel to the Vec3 and Vec2 classes
    public struct Vec4 : ISerializableData
    {
        // Convenience constants
        public static Vec4 Zero = new Vec4(0, 0, 0, 0);

        // Fields
        public float X, Y, Z, W;
        public float Length { get { return (float)System.Math.Sqrt(this.DotProduct(this)); } }

        // Constructors
        public Vec4(Vec4 copy) =>
            (X, Y, Z, W) = (copy.X, copy.Y, copy.Z, copy.W);
        public Vec4(float x, float y, float z, float w) =>
            (X, Y, Z, W) = (x, y, z, w);
        public Vec4(float x, float y, float z) =>
            (X, Y, Z, W) = (x, y, z, 1);
        public Vec4(Vec3 source) =>
            (X, Y, Z, W) = (source.X, source.Y, source.Z, 1);

        // Overrides
        public override string ToString() => $"Vec4 {{ {X}, {Y}, {Z}, {W} }}";
        public override bool Equals(object obj)
        {
            if (obj is Vec4 p)
            {
                return (X, Y, Z, W) == (p.X, p.Y, p.Z, W);
            }
            else return false;
        }
        public override int GetHashCode() => (4, X, Y, Z, W).GetHashCode();
        
        // Operators
        public static Vec4 operator +(Vec4 a, Vec4 b)
        {
            return new Vec4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }
        public static Vec4 operator -(Vec4 a, Vec4 b)
        {
            return new Vec4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }
        public static Vec4 operator *(Vec4 a, float b)
        {
            return new Vec4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }
        public static Vec4 operator /(Vec4 a, float b)
        {
            return new Vec4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }
        
        // Static math methods
        public static float DotProduct(Vec4 a, Vec4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }
        public static Vec4 Normalize(Vec4 vec)
        {
            return vec / vec.Length;
        }
        public static Vec4 Transform(Vec4 source, Mat4x4 mat)
        {
            return new Vec4(
                mat[0] * source.X + mat[1] * source.Y + mat[2] * source.Z + mat[3] * source.W,
                mat[4] * source.X + mat[5] * source.Y + mat[6] * source.Z + mat[7] * source.W,
                mat[8] * source.X + mat[9] * source.Y + mat[10] * source.Z + mat[11] * source.W,
                mat[12] * source.X + mat[13] * source.Y + mat[14] * source.Z + mat[15] * source.W
            );
        }
        
        // Static math member convenience methods
        public float DotProduct(Vec4 b)
        {
            return DotProduct(this, b);
        }
        public Vec4 Normalize()
        {
            return Normalize(this);
        }
        public Vec4 Transform(Mat4x4 mat)
        {
            return Transform(this, mat);
        }

        public void Read(BinaryReaderExt br)
        {
            X = br.ReadF32();
            Y = br.ReadF32();
            Z = br.ReadF32();
            W = br.ReadF32();
        }

        public void Write(BinaryWriterExt bw)
        {
            bw.WriteF32(X);
            bw.WriteF32(Y);
            bw.WriteF32(Z);
            bw.WriteF32(W);
        }
    }
}
