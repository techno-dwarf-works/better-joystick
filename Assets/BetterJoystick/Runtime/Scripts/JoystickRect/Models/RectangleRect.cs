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

        public Vector2 Center => _root.layout.center;

        public bool InRange(Vector2 point)
        {
            return _root.layout.Contains(point);
        }

        public Vector2 GetPointOnEdge(Vector2 point)
        {
            return GetClosestPointOnRectEdge(_root.layout, point);
        }

        private Vector2 GetClosestPointOnRectEdge(Rect rect, Vector2 point)
        {
            // Get the center point of the rectangle
            var center = rect.center;
    
            // Calculate the half-width and half-height of the rectangle
            var halfWidth = rect.width / 2;
            var halfHeight = rect.height / 2;
    
            // Calculate the distance between the center point and the outer point
            var direction = point - center;
    
            // Calculate the distance between the center point and the closest point on the edge of the rectangle
            var deltaX = Mathf.Clamp(direction.x, -halfWidth, halfWidth);
            var deltaY = Mathf.Clamp(direction.y, -halfHeight, halfHeight);
            var closestPoint = center + new Vector2(deltaX, deltaY);
    
            return closestPoint;
        }

    }
}