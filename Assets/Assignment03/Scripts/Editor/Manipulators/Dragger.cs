using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Assignment03
{
    public class Dragger : PointerManipulator
    {
        IDraggable dragTarget;
        bool wasShiftPressed;

        Vector3 pointerStartPosition;
        bool enabled;

        public Dragger(VisualElement target)
        {
            if (target is not IDraggable dragTarget)
            {
                Debug.LogError("You can not assign Dragger Manipulator to a Visual Element who doesn't implement Draggable interface");
                return;
            }

            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse,
                modifiers = EventModifiers.Shift
            });
            
            this.dragTarget = dragTarget;
            this.target = target;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        }


        // This method stores the starting position of the pointer,
        // makes target capture the pointer, denotes that a drag is now in progress
        // and notify to target that drag has started
        private void PointerDownHandler(PointerDownEvent evt)
        {
            if (evt.button != 0) return;

            evt.StopPropagation(); // added for future draggable grid
            target.CapturePointer(evt.pointerId);

            pointerStartPosition = evt.position;
            wasShiftPressed = false;
            enabled = true;

            dragTarget.OnDragStart(target.transform.position);
        }

        // This method checks whether a drag is in progress and whether target has captured the pointer.
        // If both are true, calculates a new position for target within the bounds of the window.
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (!enabled || !target.HasPointerCapture(evt.pointerId))
            {
                return;
            }

            Vector3 pointerDelta = evt.position - pointerStartPosition;

            // if shift-modifier is pressed
            if (evt.modifiers == EventModifiers.Shift)
            {
                if (!wasShiftPressed) // if shift modified was not pressed last frame
                {
                    // notify to every target linked draggable to start drag
                    foreach (IDraggable linkedDraggable in dragTarget.LinkedDraggables)
                    {
                        linkedDraggable.OnDragStart((linkedDraggable as VisualElement).transform.position);
                    }
                }
                // notify to every target linked draggable to drag
                foreach (IDraggable linkedDraggable in dragTarget.LinkedDraggables)
                {
                    linkedDraggable.OnDragMove(pointerDelta);
                }
                wasShiftPressed = true; // set that shift-modified has been pressed
            }
            else if (wasShiftPressed) // if is not pressed and was pressed last frame
            {
                // notify to every target linked draggable to end drag
                foreach (IDraggable linkedDraggable in dragTarget.LinkedDraggables)
                {
                    linkedDraggable.OnDragEnd(pointerDelta);
                }
                wasShiftPressed = false; // set that shift-modified has not been pressed
            }

            dragTarget.OnDragMove(pointerDelta);

            target.MarkDirtyRepaint();
        }

        // This method checks whether a drag is in progress and whether target has captured the pointer.
        // If both are true, makes target release the pointer.
        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (!enabled || !target.HasPointerCapture(evt.pointerId))
            {
                return;
            }
                
            target.ReleasePointer(evt.pointerId);
            dragTarget.OnDragEnd(target.transform.position);
            enabled = false;
        }
    }
}