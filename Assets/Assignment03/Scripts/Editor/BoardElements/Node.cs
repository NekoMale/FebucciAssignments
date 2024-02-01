using UnityEngine;

namespace Core.Assignment03
{
    public abstract class Node : ScriptableObject
    {
        [SerializeField] public string Guid;
        [SerializeField] public Vector2 position;
        [SerializeField, TextArea] public string Description = "Add a description for this node...";

        /// <summary>
        /// Get node children
        /// </summary>
        /// <returns>Array containing every node children, also if they are null</returns>
        public abstract Node[] GetChildren();

        /// <summary>
        /// Add a new child node at index
        /// </summary>
        /// <param name="child">Child to add as children</param>
        /// <param name="childIndex">Index where add the new child</param>
        public abstract void AddChild(Node child, int childIndex);
        
        /// <summary>
        /// Remove child node from node's children
        /// </summary>
        /// <param name="child">Child to remove</param>
        public abstract void RemoveChild(Node child);
    }
}