using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment03
{
    public class NodeView : VisualElement, IDraggable, ISelectable
    {
        public List<NodeLinkView> attachedLinks { get; private set; }

        public Node Node { get; private set; }
        static NodeView fromNode = null;
        static int portIndex;

        Selector selector;

        public NodeView(Node node) : base()
        {
            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Assignment03/Styles/NodeView.uxml") as VisualTreeAsset;
            visualTreeAsset.CloneTree(this);

            focusable = true;

            Node = node;
            viewDataKey = node.Guid;

            style.position = Position.Absolute;
            transform.position = node.position;

            VisualElement outputContainer = this.Q("output-ports-container");

            // set node ouput ports
            for (int childIndex = 0; childIndex < node.GetChildren().Length; childIndex++)
            {
                int childIndex1 = childIndex; // caching for button lambda action
                Button childButton = new Button(() =>
                {
                    selector.Select(); // select button container to highlight the connecting element
                    StartLink(childIndex1);
                });

                childButton.AddToClassList("port");
                childButton.name = "connect-child-" + childIndex;
                childButton.text = string.Empty;
                outputContainer.Add(childButton);
            }

            // Add to container the node type as class
            if (node is SingleNode)
            {
                AddToClassList("single");
            }
            else if(node is BinaryNode) 
            {
                AddToClassList("binary");
            }

            Label nameLabel = this.Q<Label>("type-label");
            nameLabel.text = node.GetType().Name;

            Label descriptionLabel = this.Q<Label>("description-label");
            descriptionLabel.bindingPath = "Description";
            descriptionLabel.Bind(new SerializedObject(node));

            attachedLinks = new List<NodeLinkView>();
            LinkedDraggables = new HashSet<IDraggable>();

            selector = new Selector(this);
            this.AddManipulator(selector);
            this.AddManipulator(new Dragger(this));
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            {
                evt.menu.AppendAction("Delete selected node", (a) => Delete());
            }
        }

        /// <summary>
        /// Start the connection from this node
        /// </summary>
        /// <param name="childIndex">Port index where connection started</param>
        private void StartLink(int childIndex)
        {
            this.Q("connect-child-" + childIndex).AddToClassList("connecting");

            portIndex = childIndex;
            fromNode = this;
        }

        /// <summary>
        /// Interrupts the connection if existing
        /// </summary>
        private void StopLink()
        {
            if(fromNode == null)
            {
                return;
            }
            fromNode.Q("connect-child-" + portIndex).RemoveFromClassList("connecting");
            fromNode = null;
        }

        /// <summary>
        /// Set this element as board root
        /// </summary>
        public void SetAsRoot()
        {
            AddToClassList("root");

            Label nameLabel = this.Q<Label>("type-label");
            nameLabel.text = "root: " + nameLabel.text; // add "root. " prefix to node type 
        }

        public Vector2 StartPosition { get; set; }
        public HashSet<IDraggable> LinkedDraggables { get; set; }

        public void OnDragStart(Vector2 transformStartPosition)
        {
            StartPosition = Node.position;
        }

        public void OnDragMove(Vector2 deltaPosition)
        {
            Undo.RecordObject(Node, "Board (Moving Node)");
            Node.position = StartPosition + deltaPosition;
            transform.position = Node.position;

            foreach(NodeLinkView attachedLink in attachedLinks)
            {
                attachedLink.Update();
            }

            EditorUtility.SetDirty(Node);
        }

        public void OnDragEnd(Vector2 transformEndPosition)
        {

        }

        public void Selected()
        {
            // if no link action exists just register this element for delete action
            if (fromNode == null)
            {
                RegisterCallback<KeyDownEvent>(Delete, TrickleDown.TrickleDown);
                return;
            }

            // if link action exists and new selected element is the same who started connection
            if (fromNode == this)
            {
                StopLink();
                return;
            }

            // otherwise if link exists and new selected element is diferrent from starting one
            // connect fromNode to selected element
            fromNode.Q("connect-child-" + portIndex).RemoveFromClassList("connecting");
            (parent as BoardView).CreateNodeLink(fromNode, this, portIndex);
            fromNode.Q("connect-child-" + portIndex).AddToClassList("connected");
            fromNode = null;
        }

        public void Unselected()
        {
            UnregisterCallback<KeyDownEvent>(Delete, TrickleDown.TrickleDown);
            if(Selector.CurrentSelectedElement is not NodeView) // if new selected element is not a node
            {
                StopLink(); // stop link action
            }
        }

        /// <summary>
        /// Delete the link on port at received index if exists
        /// </summary>
        /// <param name="portIndex">Port index where delete the link</param>
        private void ChangeExistingLink(int portIndex)
        {
            if (Node.GetChildren()[portIndex] == null)
            {
                return;
            }

            attachedLinks.FirstOrDefault(x => x.ToNode.Node == Node.GetChildren()[portIndex]).Delete();
        }

        /// <summary>
        /// Delete event handler
        /// </summary>
        /// <param name="evt"></param>
        private void Delete(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Delete)
            {
                return;
            }
            Delete();
        }

        /// <summary>
        /// Delete this element from board view removing all existing links from and to this node
        /// </summary>
        private void Delete()
        {
            UnregisterCallback<KeyDownEvent>(Delete, TrickleDown.TrickleDown);

            foreach (NodeLinkView nodeLinkView in attachedLinks)
            {
                nodeLinkView.DeleteByNode(this);
            }

            (parent as BoardView).DeleteNode(this);
        }

        /// <summary>
        /// Add new link to this node and the other linked node to its linked draggable hashset
        /// </summary>
        /// <param name="nodeLinkView">The link to add</param>
        public void AddLink(NodeLinkView nodeLinkView)
        {
            if(attachedLinks.Contains(nodeLinkView))
            {
                return;
            }
            
            attachedLinks.Add(nodeLinkView);
            
            if (nodeLinkView.FromNode == this)
            {
                LinkedDraggables.Add(nodeLinkView.ToNode);
            }
            else
            {
                LinkedDraggables.Add(nodeLinkView.FromNode);
            }
        }

        /// <summary>
        /// Remove link from its attached links and other node from its linked draggable hashset
        /// </summary>
        /// <param name="nodeLinkView"></param>
        public void RemoveLink(NodeLinkView nodeLinkView)
        {
            attachedLinks.Remove(nodeLinkView);
            LinkedDraggables.Remove(nodeLinkView.FromNode);
            LinkedDraggables.Remove(nodeLinkView.ToNode);
        }

        /// <summary>
        /// Create a link from this node to received node starting from port at received index
        /// </summary>
        /// <param name="childView">Node to link with</param>
        /// <param name="portIndex">Port index where link starts</param>
        /// <returns></returns>
        public NodeLinkView LinkTo(NodeView childView, int portIndex)
        {
            NodeLinkView linkView = new NodeLinkView(this.Q("connect-child-" + portIndex));

            linkView.SetLink(this, childView);

            AddLink(linkView);
            childView.AddLink(linkView);

            return linkView;
        }
    }
}