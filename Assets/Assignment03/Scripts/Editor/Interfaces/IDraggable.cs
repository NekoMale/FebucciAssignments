using System.Collections.Generic;
using UnityEngine;

namespace Core.Assignment03
{
    /// <summary>
    /// Interface used from Dragger manipulator in order to manage draggable elements
    /// </summary>
    public interface IDraggable
    {
        /// <summary>
        /// Position of interest when drag starts
        /// </summary>
        public Vector2 StartPosition { get; set; }
        /// <summary>
        /// Other elements to drag when shift-modified drag performs
        /// </summary>
        public HashSet<IDraggable> LinkedDraggables { get; set; }

        /// <summary>
        /// Method called when drag starts
        /// </summary>
        /// <param name="transformStartPosition">Element position when drag starts</param>
        public void OnDragStart(Vector2 transformStartPosition);
        /// <summary>
        /// Method called every time mouse moves while drag performing
        /// </summary>
        /// <param name="deltaPosition">Delta position between start and current position</param>
        public void OnDragMove(Vector2 deltaPosition);
        /// <summary>
        /// Method called when drag end
        /// </summary>
        /// <param name="transformEndPosition">Element position when drag ends</param>
        public void OnDragEnd(Vector2 transformEndPosition); 
    }
}
