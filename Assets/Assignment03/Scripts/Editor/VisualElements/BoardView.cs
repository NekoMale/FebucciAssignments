using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment03
{
    public class BoardView : VisualElement, IDraggable, ISelectable
    {
        // Add this VisualElement to UIBuilder
        public new class UxmlFactory : UxmlFactory<BoardView, UxmlTraits> { }

        public Vector2 StartPosition { get; set; }
        public HashSet<IDraggable> LinkedDraggables { get; set; } = new HashSet<IDraggable>();
        /// <summary>
        /// Current grid offset
        /// </summary>
        Vector2 gridOffset;

        public Board activeBoard { get; private set; }

        List<NodeView> activeNodes = new List<NodeView>();
        List<NodeLinkView> activeLinks = new List<NodeLinkView>();
        Vector2 localMousePosition;

        public BoardView() {
            this.AddManipulator(new Dragger(this));
            gridOffset = transform.position;
            generateVisualContent += DrawGrid;
            activeBoard = null;

            focusable = true;

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            this.AddManipulator(new Selector(this)); // this manipulator is added in order to remove focus on node views when user clicks on background

            RegisterCallback<KeyDownEvent>(CreateNodeFromShortcut, TrickleDown.TrickleDown);

            RegisterCallback<MouseMoveEvent>(evt =>
            {
                localMousePosition = evt.localMousePosition;
            });

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        public void Disable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        private void CreateNodeFromShortcut(KeyDownEvent evt)
        {
            if(evt.keyCode == KeyCode.S)
            {
                CreateNode(typeof(SingleNode));
            }
            else if(evt.keyCode == KeyCode.B)
            {
                CreateNode(typeof(BinaryNode));
            }
        }

        /// <summary>
        /// Update board view everytime an undo-redo action is perfomed
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            AssetDatabase.SaveAssets();

            CreateBoardView(activeBoard);
        }

        /// <summary>
        /// Add node to board view
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeView AddNode(Node node)
        {
            NodeView nodeView = new NodeView(node);
            Add(nodeView);
            activeNodes.Add(nodeView);
            return nodeView;
        }

        /// <summary>
        /// Find a node view starting from asset node
        /// </summary>
        /// <param name="node">Node to look for</param>
        /// <returns>Node if found, null otherwise</returns>
        NodeView FindNodeView(Node node)
        {
            return activeNodes.FirstOrDefault(x => x.viewDataKey == node.Guid);
        }

        /// <summary>
        /// Create the board view with all node if a root has been set
        /// </summary>
        /// <param name="board">Board to show</param>
        public void CreateBoardView(Board board)
        {
            activeBoard = board;

            foreach (VisualElement element in activeNodes)
            {
                Remove(element);
                element.RemoveFromHierarchy();
            }
            activeNodes.Clear();

            foreach (VisualElement element in activeLinks)
            {
                Remove(element);
                element.RemoveFromHierarchy();
            }
            activeLinks.Clear();

            if (activeBoard.Root == null)
            {
                return;
            }

            foreach (Node node in activeBoard.Nodes)
            {
                AddNode(node);
            }
            FindNodeView(activeBoard.Root).SetAsRoot();

            foreach (Node parent in activeBoard.Nodes)
            {
                int childIndex = -1;
                foreach (Node child in parent.GetChildren())
                {
                    childIndex++;
                    if (child == null) continue;

                    CreateNodeLink(FindNodeView(parent), FindNodeView(child), childIndex);
                }
            }
        }

        /// <summary>
        /// Draw grid background
        /// </summary>
        /// <param name="mgc"></param>
        private void DrawGrid(MeshGenerationContext mgc)
        {
            Painter2D painter2D = mgc.painter2D;

            painter2D.lineWidth = 0.2f;
            painter2D.strokeColor = Color.white;


            for (int x = (int)gridOffset.x; x < resolvedStyle.width; x+=25)
            {
                painter2D.BeginPath();

                painter2D.MoveTo(new Vector2(x, 0));
                painter2D.LineTo(new Vector2(x, resolvedStyle.height));
                
                painter2D.Stroke();
            }

            for (int y = (int)gridOffset.y; y < resolvedStyle.height; y+=25)
            {
                painter2D.BeginPath();
                
                painter2D.MoveTo(new Vector2(0, y));
                painter2D.LineTo(new Vector2(resolvedStyle.width, y));
             
                painter2D.Stroke();
            }
        }

        public void OnDragStart(Vector2 transformStartPosition)
        {
            StartPosition = gridOffset;
        }

        public void OnDragMove(Vector2 deltaPosition)
        {
            gridOffset = StartPosition + deltaPosition;
            gridOffset.x %= 25;
            gridOffset.y %= 25;
        }

        public void OnDragEnd(Vector2 transformEndPosition)
        {
            transform.position = Vector2.zero;
        }

        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is not BoardView)
            {
                return;
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<Node>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
                }
            }
        }

        /// <summary>
        /// Create new asset node and its visual element on this board
        /// </summary>
        /// <param name="type">Type of node to create</param>
        private void CreateNode(Type type)
        {
            Node newNode = activeBoard.CreateNode(type);
            newNode.position = localMousePosition;

            AddNode(newNode);

            CreateBoardView(activeBoard);
        }

        /// <summary>
        /// Link two node on port index setting one as child of the other 
        /// </summary>
        /// <param name="fromNodeView">The node who is parent</param>
        /// <param name="toNodeView">The node who is child</param>
        /// <param name="portIndex">Parent port index where link happens</param>
        public void CreateNodeLink(NodeView fromNodeView, NodeView toNodeView, int portIndex)
        {
            activeBoard.CreateLink(fromNodeView.Node, toNodeView.Node, portIndex);
            
            NodeLinkView linkView = fromNodeView.LinkTo(toNodeView, portIndex);
            activeLinks.Add(linkView);
            Add(linkView);
            
            foreach(NodeView nodeView in activeNodes)
            {
                nodeView.BringToFront();
            }
        }

        /// <summary>
        /// Delete the node view and its node asset from this board
        /// </summary>
        /// <param name="nodeView"></param>
        public void DeleteNode(NodeView nodeView)
        {
            activeBoard.DeleteNode(nodeView.Node);

            activeNodes.Remove(nodeView);
            Remove(nodeView);
            nodeView.RemoveFromHierarchy();

            CreateBoardView(activeBoard);
        }

        /// <summary>
        /// Delete the link from this board
        /// </summary>
        /// <param name="nodeLinkView"></param>
        public void DeleteNodeLink(NodeLinkView nodeLinkView)
        {
            activeBoard.DeleteLink(nodeLinkView.FromNode.Node, nodeLinkView.ToNode.Node);

            activeLinks.Remove(nodeLinkView);
            Remove(nodeLinkView);
            nodeLinkView.RemoveFromHierarchy();
        }

        public void Selected()
        {

        }

        public void Unselected()
        {

        }
    }
}
