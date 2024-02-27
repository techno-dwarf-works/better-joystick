using System;
using BetterJoystick.Runtime.JoystickRect.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace BetterJoystick.Runtime.JoystickRect.Models
{
    [Serializable]
    public class RectangleRect : IJoystickRect
    {
        private VisualElement _root;

        public RectangleRect(VisualElement root)
        {
            _root = root;
        }

        public float Radius => Mathf.Max(_root.layout.width, _root.layout.height) / 2f;

        public Vector2 Center => _root.layout.center - _root.layout.position;

        public bool InRange(Vector2 point)
        {
            // Check the point exists within the rectangle relative to bounds
            return new Rect(0,0, _root.layout.width, _root.layout.height).Contains(point);
        }

        public Vector2 GetPointOnEdge(Vector2 point)
        {
            return GetClosestPointOnRectEdge(point);
        }

        private Vector2 GetClosestPointOnRectEdge(Vector2 point)
        {
            var halfWidth = Center.x;
            var halfHeight = Center.y;
    
            // Calculate the distance between the center point and the outer point
            var direction = point - Center;
    
            // Calculate the distance between the center point and the closest point on the edge of the rectangle
            var deltaX = Mathf.Clamp(direction.x, -halfWidth, halfWidth);
            var deltaY = Mathf.Clamp(direction.y, -halfHeight, halfHeight);
            var closestPoint = Center + new Vector2(deltaX, deltaY);
    
            return closestPoint;
        }

    }
}