using UnityEngine;

namespace Core.Assignment02
{
    /// <summary>
    /// Class to add to a GameObject in order to manage creation of a cube
    /// </summary>
    public class CubeCreator : MonoBehaviour
    {
        /// <summary>
        /// Cube start position, rotation will be applied on this point
        /// </summary>
        [SerializeField] Vector3 start = new Vector3(0f, -1f, 0f);
        /// <summary>
        /// Cube end position
        /// </summary>
        [SerializeField] Vector3 end = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// Cube rotation
        /// </summary>
        [SerializeField] Quaternion rotation = Quaternion.identity;

        /// <summary>
        /// Cube start position property, rotation will be applied on this point
        /// </summary>
        public Vector3 Start
        {
            get => start;
            set => start = value;
        }

        /// <summary>
        /// Cube end position property
        /// </summary>
        public Vector3 End
        {
            get => end;
            set => end = value;
        }

        /// <summary>
        /// Cube property
        /// </summary>
        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }
    }
}