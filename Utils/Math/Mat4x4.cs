
namespace LibTWOC.Utils.Math
{
    public struct Mat4x4 : ISerializableData
    {
        // Convenience constants
        public static Mat4x4 Identity = new Mat4x4(new float[16] { 
            1, 0, 0, 0, 
            0, 1, 0, 0, 
            0, 0, 1, 0, 
            0, 0, 0, 1 
        });
        public static Mat4x4 LookAt(Vec3 from, Vec3 to, Vec3 up)
        {
            var z = (to - from).Normalize() * -1;
            var x = up.CrossProduct(z).Normalize();
            var y = z.CrossProduct(x);

            return new Mat4x4( 
                new float[16] {
                    x.X, x.Y, x.Z, -from.DotProduct(x),
                    y.X, y.Y, y.Z, -from.DotProduct(y),
                    z.X, z.Y, z.Z, -from.DotProduct(z),
                    0, 0, 0, 1
                }
            );
        }
        public static Mat4x4 RotateX(double angle) => new Mat4x4(new float[16] {
            1, 0, 0, 0,
            0, (float)System.Math.Cos(angle), (float)-System.Math.Sin(angle), 0,
            0, (float)System.Math.Sin(angle), (float)System.Math.Cos(angle), 0,
            0, 0, 0, 1
        });
        public static Mat4x4 RotateY(double angle) => new Mat4x4(new float[16] {
            (float)System.Math.Cos(angle), 0, (float)System.Math.Sin(angle), 0,
            0, 1, 0, 0,
            (float)-System.Math.Sin(angle), 0, (float)System.Math.Cos(angle), 0,
            0, 0, 0, 1
        });
        public static Mat4x4 RotateZ(double angle) => new Mat4x4(new float[16] {
            (float)System.Math.Cos(angle), (float)-System.Math.Sin(angle), 0, 0,
            (float)System.Math.Sin(angle), (float)System.Math.Cos(angle), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        });
        
        // Fields
        public float[] Data;
        
        // Constructors
        public Mat4x4(Mat4x4 copy)
        {
            Data = new float[16];
            for (var i = 0; i < 16; i++)
                Data[i] = copy.Data[i];
        }
        public Mat4x4(float[] values) {
            Data = new float[16];
            for (var i = 0; i < 16; i++)
                Data[i] = values[i];
        }

        // Overrides
        public override string ToString() => $"Mat4x4 {{ {string.Join(", ", Data)} }}";
        public override bool Equals(object obj)
        {
            if (obj is Mat4x4)
            {
                for (var i = 0; i < 16; i++)
                {
                    if (Data[i] != ((Mat4x4)obj)[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode() => (4, 4, Data).GetHashCode();
        
        // Operators
        public static Mat4x4 operator *(Mat4x4 a, Mat4x4 b)
        {
            return new Mat4x4(new float[16] {
                a.GetRow(0).DotProduct(b.GetColumn(0)), a.GetRow(0).DotProduct(b.GetColumn(1)), a.GetRow(0).DotProduct(b.GetColumn(2)), a.GetRow(0).DotProduct(b.GetColumn(3)),
                a.GetRow(1).DotProduct(b.GetColumn(0)), a.GetRow(1).DotProduct(b.GetColumn(1)), a.GetRow(1).DotProduct(b.GetColumn(2)), a.GetRow(1).DotProduct(b.GetColumn(3)),
                a.GetRow(2).DotProduct(b.GetColumn(0)), a.GetRow(2).DotProduct(b.GetColumn(1)), a.GetRow(2).DotProduct(b.GetColumn(2)), a.GetRow(2).DotProduct(b.GetColumn(3)),
                a.GetRow(3).DotProduct(b.GetColumn(0)), a.GetRow(3).DotProduct(b.GetColumn(1)), a.GetRow(3).DotProduct(b.GetColumn(2)), a.GetRow(3).DotProduct(b.GetColumn(3))
            });
        }

        // Index operators
        public float this[int i] => this.Data[i];
        public float this[int x, int y] => this.Data[x + y * 4];

        // Convenience methods
        public Vec4 GetRow(int row)
        {
            return new Vec4(this[4 * row], this[4 * row + 1], this[4 * row + 2], this[4 * row + 3]);
        }
        public Vec4 GetColumn(int column)
        {
            return new Vec4(this[4 * column], this[4 * (column + 1)], this[4 * (column + 2)], this[4 * (column + 3)]);
        }

        // Serialization methods
        public void Read(BinaryReaderExt br)
        {
            Data = new float[16];
            for (var i = 0; i < 16; i++)
            {
                Data[i] = br.ReadF32();
            }
        }

        public void Write(BinaryWriterExt bw)
        {
            for (var i = 0; i < 16; i++)
            {
                bw.WriteF32(Data[i]);
            }
        }
    }
}
