using UnityEngine.UIElements;

namespace Core.Assignment03
{
    public class Selector : MouseManipulator
    {
        public static ISelectable CurrentSelectedElement { get; private set; } = null;

        public Selector(VisualElement target)
        {
            base.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });

            this.target = target;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(SelectByEvent);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(SelectByEvent);
        }

        private void SelectByEvent(MouseDownEvent evt)
        {
            evt.StopPropagation();

            Select();
        }

        public void Select()
        {
            if (CurrentSelectedElement == target)
            {
                return;
            }

            ISelectable lastSelectedElement = CurrentSelectedElement;

            CurrentSelectedElement = target as ISelectable;
            target.AddToClassList("selected");
            CurrentSelectedElement.Selected();

            if (lastSelectedElement != null)
            {
                (lastSelectedElement as VisualElement).RemoveFromClassList("selected");
                lastSelectedElement.Unselected();
            }
        }
    }
}