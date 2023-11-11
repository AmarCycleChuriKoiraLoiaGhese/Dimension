using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Dimension
{
    class LookCamera
    {
        public event PropertyChangedEventHandler LookDirectionChanged = (sender, e) => { };

        public Vector3D NewVector3D { get; set; } // Needs change.

        private Vector3D RotateVector3D()
        {


            return new Vector3D();
        }

    }
}
