using System.Windows.Media.Media3D;

namespace Dimension
{
    /// <summary>
    /// This class contains all the procedures relating Quaternion rotations.
    /// </summary>
    class Quats
    {
        #region 3D Vector Rotation

        /// <summary>
        /// Rotates a given 3D vector by a specified angle in the specified axis.
        /// </summary>
        /// 
        /// <param name="Vector_To_Rotate">
        /// The 3D vector that needs to be rotated.
        /// </param>
        /// 
        /// <param name="Angle">
        /// Angle that specifies how much the 3D vector needs to be rotated by.
        /// </param>
        /// 
        /// <param name="Direction">
        /// The perpenficular axis from which the rotation occurs (specifies direction of the rotation).
        /// </param>
        /// 
        /// <returns>
        /// Rotated 3D vector.
        /// </returns>
        public Vector3D RotateVector3D(Vector3D Vector_To_Rotate, double Angle, Vector3D Direction)
        {
            // Stores a a 3D Identity Matrix.
            var Rotation_Matrix = Matrix3D.Identity;

            // Stores Quaternion rotation based on parameters.
            var Rotation_Quat = new Quaternion(Direction, Angle);

            // Converts Identity to Rotation matrix based on Quaternion.
            Rotation_Matrix.Rotate(Rotation_Quat);

            // Rotates given 3D vector using the Rotation Matrix.
            var Rotated_Vector = Rotation_Matrix.Transform(Vector_To_Rotate);

            // Rotated 3D vector is returned.
            return Rotated_Vector;

        }

        #endregion
    }

}
