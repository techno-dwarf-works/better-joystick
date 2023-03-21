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
            UxmlBoolAttributeDescription normalizeDescription =
                new UxmlBoolAttributeDescription { name = "Normalize", defaultValue = false };

            UxmlBoolAttributeDescription recenterDescription =
                new UxmlBoolAttributeDescription { name = "Recenter", defaultValue = true };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as Joystick;

                ate.Normalize = normalizeDescription.GetValueFromBag(bag, cc);
                ate.Recenter = recenterDescription.GetValueFromBag(bag, cc);
            }
        }

        // Must expose your element class to a { get; set; } property that has the same name 
        // as the name you set in your UXML attribute description with the camel case format
        public bool Normalize { get; set; }
        public bool Recenter { get; set; }

        public const string StyleClassName = "better-joystick";

        private readonly InnerJoystickImage _joystickInner;
        private IJoystickRect _joystickRect;

        public Joystick()
        {
            AddToClassList(StyleClassName);
            _joystickInner = new InnerJoystickImage();
            _joystickInner.DragEvent += OnDragEvent;
            _joystickInner.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _joystickInner.style.position = new StyleEnum<Position>(Position.Absolute);
            _joystickInner.style.height = new StyleLength(new Length(100, LengthUnit.Pixel));
            _joystickInner.style.width = new StyleLength(new Length(100, LengthUnit.Pixel));
            _joystickInner.image = new Texture2D(100, 100);
            _joystickInner.RegisterCallback<GeometryChangedEvent>(OnAttached);
            Add(_joystickInner);
            _joystickRect = new CircleRect(this);
            Value = Vector2.zero;
            this.RegisterValueChangedCallback(OnChange);
        }

        private void OnChange(ChangeEvent<Vector2> evt)
        {
            Debug.Log(evt.newValue);
        }

        private void OnAttached(GeometryChangedEvent geometryChangedEvent)
        {
            _joystickInner.UnregisterCallback<GeometryChangedEvent>(OnAttached);
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
                var mousePosition = obj.MousePosition;
                if (!_joystickRect.InRange(obj.MousePosition))
                {
                    mousePosition = _joystickRect.GetPointOnEdge(obj.MousePosition);
                }

                var panelPosition = this.WorldToLocal(mousePosition - centering);
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

            using (var pulled = ChangeEvent<Vector2>.GetPooled(prevValue, newValue))
            {
                pulled.target = this;
                panel.visualTree.SendEvent(pulled);
                Value = newValue;
            }
        }

        private Vector2 GetCentering()
        {
            return new Vector2(_joystickInner.resolvedStyle.height / 2f, _joystickInner.resolvedStyle.width / 2f);
        }

        private void PlaceJoystickAtCenter()
        {
            var centering = GetCentering();
            _joystickInner.style.top = centering.y;
            _joystickInner.style.left = centering.x;
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