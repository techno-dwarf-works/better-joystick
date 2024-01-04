using BetterJoystick.Runtime.JoystickRect.Interfaces;
using BetterJoystick.Runtime.JoystickRect.Models;
using BetterJoystick.Runtime.Models;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace BetterJoystick.Runtime
{
    public class Joystick : VisualElement, INotifyValueChanged<Vector2>
    {
       
        
        [Preserve]
        public new class UxmlFactory : UxmlFactory<Joystick, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _normalizeDescription =
                new UxmlBoolAttributeDescription { name = "Normalize", defaultValue = false };

            private readonly UxmlBoolAttributeDescription _recenterDescription =
                new UxmlBoolAttributeDescription { name = "Recenter", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as Joystick;

                ate.Normalize = _normalizeDescription.GetValueFromBag(bag, cc);
                ate.Recenter = _recenterDescription.GetValueFromBag(bag, cc);
            }
        }

        /// <summary>
        /// Normalizing output value in JoystickEvent or you can set this in UIBuilder
        /// </summary>
        public bool Normalize { get; set; } = false;

        /// <summary>
        /// Allows to re-centering inner joystick after user release or you can set this in UIBuilder
        /// </summary>
        public bool Recenter { get; set; } = true;

        public const string StyleClassName = "better-joystick";

        private readonly InnerJoystickImage _joystickInner;
        private IJoystickRect _joystickRect;

        public Joystick()
        {
            AddToClassList(StyleClassName);
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/JoystickStyles"));
            _joystickInner = new InnerJoystickImage();
            _joystickInner.DragEvent += OnDragEvent;
            Add(_joystickInner);
            _joystickInner.BringToFront();
            RegisterCallback<GeometryChangedEvent>(OnAttached);
            _joystickRect = new CircleRect(this);
            Value = Vector2.zero;
        }

        private void OnAttached(GeometryChangedEvent geometryChangedEvent)
        {
            PlaceJoystickAtCenter();
        }

        private void OnDragEvent(MouseDragEvent obj)
        {
            Vector2 newValue;
            Vector2 prevValue;
            if (obj.State == DragState.AtRest)
            {
                if (!Recenter) return;
                PlaceJoystickAtCenter();
                newValue = Vector2.zero;
                prevValue = Value;
            }
            else
            {
                var centering = GetCentering();
                var mousePosition = obj.MousePosition - GetJoystickRelativeAreaPosition();
                if (!_joystickRect.InRange(mousePosition))
                {
                    mousePosition = _joystickRect.GetPointOnEdge(mousePosition);
                }

                var panelPosition = mousePosition - centering;
                _joystickInner.style.top = panelPosition.y;
                _joystickInner.style.left = panelPosition.x;
                newValue = mousePosition - _joystickRect.Center;
                prevValue = Value;
            }

            if (Normalize)
            {
                var t = Mathf.InverseLerp(0, _joystickRect.Radius, newValue.magnitude);
                newValue = newValue.normalized * t;
            }

            using (var pulled = JoystickEvent.GetPooled(prevValue, newValue))
            {
                pulled.target = this;
                panel.visualTree.SendEvent(pulled);
                Value = newValue;
            }
        }

        private Vector2 GetCentering()
        {
            var innerResolvedStyle = _joystickInner.resolvedStyle;
            var styleHeight = (innerResolvedStyle.height + GetTopOffset(innerResolvedStyle)) / 2f;
            var styleWidth = (innerResolvedStyle.width + GetLeftOffset(innerResolvedStyle)) / 2f;
            return new Vector2(styleHeight, styleWidth);
        }

        private void PlaceJoystickAtCenter()
        {
            var centering = GetCentering();
            var joystickResolvedStyle = resolvedStyle;
            _joystickInner.style.top = (joystickResolvedStyle.height - GetTopOffset(joystickResolvedStyle)) / 2f - centering.y;
            _joystickInner.style.left = (joystickResolvedStyle.width - GetLeftOffset(joystickResolvedStyle)) / 2f - centering.x;
        }
        
        private Vector2 GetJoystickRelativeAreaPosition()
        {
            var joystickElementPosition = worldBound;
            return new Vector2(joystickElementPosition.x, joystickElementPosition.y);
        }

        private float GetTopOffset(IResolvedStyle resolved)
        {
            return resolved.paddingTop + resolved.marginTop + resolved.borderTopWidth;
        }

        private float GetLeftOffset(IResolvedStyle resolved)
        {
            return resolved.paddingLeft + resolved.marginLeft + resolved.borderLeftWidth;
        }

        public void SetJoystickRect(IJoystickRect joystickRect)
        {
            _joystickRect = joystickRect;
            PlaceJoystickAtCenter();
        }

        public void SetValueWithoutNotify(Vector2 newValue)
        {
            Value = newValue;
        }

        public Vector2 Value { get; private protected set; }

        Vector2 INotifyValueChanged<Vector2>.value
        {
            get => Value;
            set => Value = value;
        }
    }
}