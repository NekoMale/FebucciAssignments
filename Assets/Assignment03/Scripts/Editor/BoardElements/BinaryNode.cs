using UnityEngine;

namespace Core.Assignment03
{
    public class BinaryNode : Node
    {
        [SerializeField] public Node[] children = new Node[2];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void AddChild(Node child, int childIndex)
        {
            children[childIndex] = child;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Node[] GetChildren()
        {
            return children;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void RemoveChild(Node child)
        {
            for(int childIndex = 0; childIndex < children.Length; childIndex++)
            {
                if (children[childIndex] != child)
                {
                    continue;
                }

                children[childIndex] = null;
                return;
            }
        }
    }
}