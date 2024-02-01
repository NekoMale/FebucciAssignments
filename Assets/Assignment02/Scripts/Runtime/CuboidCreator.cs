using UnityEngine;

namespace Core.Assignment02
{
    /// <summary>
    /// Class to add to a GameObject in order to manage creation of a cuboid
    /// </summary>
    public class CuboidCreator : MonoBehaviour
    {
        /// <summary>
        /// Cuboid start position, rotation will be applied on this point
        /// </summary>
        [SerializeField] Vector3 start = new Vector3(0f, -1f, 0f);
        /// <summary>
        /// Cuboid end position
        /// </summary>
        [SerializeField] Vector3 end = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// Cuboid rotation
        /// </summary>
        [SerializeField] Quaternion rotation = Quaternion.identity;

        /// <summary>
        /// Cuboid start position property, rotation will be applied on this point
        /// </summary>
        public Vector3 Start
        {
            get => start;
            set => start = value;
        }

        /// <summary>
        /// Cuboid end position property
        /// </summary>
        public Vector3 End
        {
            get => end;
            set => end = value;
        }

        /// <summary>
        /// Cuboid property
        /// </summary>
        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }
    }
}