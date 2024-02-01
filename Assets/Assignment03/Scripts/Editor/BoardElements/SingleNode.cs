using UnityEngine;

namespace Core.Assignment03
{
    public class SingleNode : Node
    {
        [SerializeField] public Node child;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void AddChild(Node child, int childIndex)
        {
            this.child = child;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Node[] GetChildren()
        {
            return new Node[] { child };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void RemoveChild(Node child)
        {
            if(child != this.child)
            {
                return;
            }

            this.child = null;
        }
    }
}