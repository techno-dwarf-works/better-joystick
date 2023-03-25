using UnityEngine;

namespace BetterJoystick.Runtime.JoystickRect.Interfaces
{
    public interface IJoystickRect
    {
        public Vector2 Center { get; }
        public float Radius { get; }
        public bool InRange(Vector2 point);
        public Vector2 GetPointOnEdge(Vector2 point);
    }
}