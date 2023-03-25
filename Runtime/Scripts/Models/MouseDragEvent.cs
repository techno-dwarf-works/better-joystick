using UnityEngine;
using UnityEngine.UIElements;

namespace BetterJoystick.Runtime.Models
{
    public class MouseDragEvent
    {
        public IEventHandler Target { get; }
        public Vector2 MousePosition { get; }
        public Vector2 LocalMosePosition { get; }
        public Vector2 DeltaMousePosition { get; }
        public DragState State { get; }

        public MouseDragEvent(IMouseEvent eventBase, IEventHandler target, DragState state)
        {
            Target = target;
            State = state;
            MousePosition = eventBase.mousePosition;
            LocalMosePosition = eventBase.localMousePosition;
            DeltaMousePosition = eventBase.mouseDelta;
        }
    }
}