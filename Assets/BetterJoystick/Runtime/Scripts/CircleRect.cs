using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace BetterJoystick.Runtime
{
    public interface IJoystickRect
    {
        public Vector2 Center { get; }
        public float Radius { get; }
        public bool InRange(Vector2 point);
        public Vector2 GetPointOnEdge(Vector2 point);
    }
    
    [Serializable]
    public class CircleRect : IJoystickRect
    {
        private VisualElement _root;

        public CircleRect(VisualElement root)
        {
            _root = root;
        }

        public float Radius => _root.layout.width / 2f;

        public Vector2 Center => _root.layout.center;

        public bool InRange(Vector2 point)
        {
            var radius = Radius;
            return (point - Center).sqrMagnitude <= radius * radius;
        }

        public Vector2 GetPointOnEdge(Vector2 point)
        {
            var center = Center;
            return center + (point - center).normalized * Radius;
        }
    }
}