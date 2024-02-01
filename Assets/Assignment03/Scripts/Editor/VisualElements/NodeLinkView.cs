using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment03
{
    public class NodeLinkView : VisualElement, ISelectable
    {
        VisualElement filler;

        VisualElement startingPortView;
        public NodeView FromNode { get; private set; }
        public NodeView ToNode { get; private set; }

        public NodeLinkView(VisualElement startingPortView)
        {
            VisualTreeAsset visualTreeAsset = EditorGUIUtility.Load("Assets/Assignment03/Styles/NodeLinkView.uxml") as VisualTreeAsset;
            visualTreeAsset.CloneTree(this);
            focusable = true;

            filler = this.Q("node-link-filler");
            AddToClassList("node-link-container");

            this.startingPortView = startingPortView;

            this.AddManipulator(new Selector(this));
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
        }

        public void SetLink(NodeView fromNode, NodeView toNode)
        {
            FromNode = fromNode; 
            ToNode = toNode;

            startingPortView.AddToClassList("connected");

            RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
        }

        public void SetNewToNode(NodeView toNode)
        {
            ToNode = toNode;

            Update();
        }

        private void GeometryChangedCallback(GeometryChangedEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
            Update();
        }

        public void Update()
        {
            float xDistance = ToNode.worldBound.min.x - startingPortView.worldBound.center.x;
            float yDistance = ToNode.worldBound.center.y - startingPortView.worldBound.center.y + filler.resolvedStyle.height*0.5f;

            style.position = Position.Absolute;
            transform.position = new Vector3(startingPortView.worldBound.center.x - parent.worldBound.min.x,
                startingPortView.worldBound.center.y - filler.resolvedStyle.height * 0.5f - parent.worldBound.min.y, 0f) ;

            filler.style.width = MathF.Sqrt(MathF.Pow(xDistance, 2) + MathF.Pow(yDistance, 2));
            filler.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Rad2Deg * Mathf.Atan2(yDistance, xDistance), AngleUnit.Degree)));
            style.width = 0; 
            style.height = 0;
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            {
                evt.menu.AppendAction("Delete selected link", (a) => Delete());
            }
        }

        public void Selected()
        {
            RegisterCallback<KeyDownEvent>(Delete, TrickleDown.TrickleDown);
        }

        public void Unselected()
        {
            UnregisterCallback<KeyDownEvent>(Delete, TrickleDown.TrickleDown);
        }

        private void Delete(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Delete) return;

            Delete();
        }

        /// <summary>
        /// Delete action if delete happens when this node is selected or
        /// when a link from this starting port has changed
        /// [IMPROVEMENT] TO DO: change this connection instead create a new one
        /// </summary>
        public void Delete()
        {
            ToNode.RemoveLink(this);
            FromNode.RemoveLink(this);
            startingPortView.RemoveFromClassList("connected");

            (parent as BoardView).DeleteNodeLink(this);
        }

        /// <summary>
        /// Delete this link when one attached node has been deleted
        /// </summary>
        /// <param name="deletedNode"></param>
        public void DeleteByNode(NodeView deletedNode)
        {
            if (deletedNode == FromNode)
            {
                ToNode.RemoveLink(this);
            }
            else
            {
                FromNode.RemoveLink(this);
                startingPortView.RemoveFromClassList("connected");
            }

            (parent as BoardView).DeleteNodeLink(this);
        }
    }
}
