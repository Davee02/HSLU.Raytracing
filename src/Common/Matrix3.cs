namespace Common
{
    public readonly struct Matrix3x3
    {
        public Matrix3x3(params float[] values)
        {
            if (values.Length != 9)
            {
                throw new ArgumentException("Matrix3x3 must have 9 values");
            }

            A11 = values[0];
            A12 = values[1];
            A13 = values[2];
            A21 = values[3];
            A22 = values[4];
            A23 = values[5];
            A31 = values[6];
            A32 = values[7];
            A33 = values[8];
        }

        public float A11 { get; }

        public float A12 { get; }

        public float A13 { get; }

        public float A21 { get; }

        public float A22 { get; }

        public float A23 { get; }

        public float A31 { get; }

        public float A32 { get; }

        public float A33 { get; }

        public readonly Vector3 Multiply(Vector3 vector)
        {
            var x = (A11 * vector.X) + (A12 * vector.Y) + (A13 * vector.Z);
            var y = (A21 * vector.X) + (A22 * vector.Y) + (A23 * vector.Z);
            var z = (A31 * vector.X) + (A32 * vector.Y) + (A33 * vector.Z);
            return new Vector3(x, y, z);
        }

        public readonly bool TryInverse(out Matrix3x3 inverse)
        {
            var determinant = (A11 * A22 * A33) + (A12 * A23 * A31) + (A13 * A21 * A32) - (A13 * A22 * A31) - (A11 * A23 * A32) - (A12 * A21 * A33);
            if (MathF.Abs(determinant) < 1e-10)
            {
                inverse = default;
                return false;
            }

            var invDet = 1.0f / determinant;

            var a11 = (A22 * A33 - A23 * A32) * invDet;
            var a12 = (A13 * A32 - A12 * A33) * invDet;
            var a13 = (A12 * A23 - A13 * A22) * invDet;
            var a21 = (A23 * A31 - A21 * A33) * invDet;
            var a22 = (A11 * A33 - A13 * A31) * invDet;
            var a23 = (A13 * A21 - A11 * A23) * invDet;
            var a31 = (A21 * A32 - A22 * A31) * invDet;
            var a32 = (A12 * A31 - A11 * A32) * invDet;
            var a33 = (A11 * A22 - A12 * A21) * invDet;

            inverse = new Matrix3x3(a11, a12, a13, a21, a22, a23, a31, a32, a33);
            return true;
        }
    }
}
