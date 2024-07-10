#[Deprecated] Better Joystick

> [!CAUTION]
> Package deprecated and replaced with - [Better Controls](https://github.com/techno-dwarf-works/better-controls)

[![openupm](https://img.shields.io/npm/v/com.tdw.better.joystick?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.tdw.better.joystick/)

![image](https://user-images.githubusercontent.com/22265817/227740413-aa3fafb1-9e93-48e8-9968-6d07655118fa.png)

Simple UIToolkit joystick

## Usage
```c#
public class Test : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    public void CreateJoystick()
    {
        var joystick = new Joystick();
        joystick.RegisterCallback<JoystickEvent>(OnDrag);
        joystick.Normalize = true; // Normalizing output value in JoystickEvent or you can set this in UIBuilder
        joystick.Recenter = true; // Allows to re-centering inner joystick after user release or you can set this in UIBuilder
        
        //or

        var joystickFromDocument = document.rootVisualElement.Q<Joystick>();
        joystickFromDocument.RegisterCallback<JoystickEvent>(OnDrag);
    }

    private void OnDrag(ChangeEvent<Vector2> evt)
    {
        Debug.Log(evt.newValue);
    }
}
```

To change style of joystick use uss classes:

```css
.better-inner-joystick{
}

.better-joystick{
}
```

### Note
You can alter joystick restriction (which shape joystick tends to follow) behaviour by implementing `IJoystickRect`.
From the box you have 2 already implemented `JoystickRect`'s:
1. CircleRect (set by default)
2. RectangleRect

To set new IJoystickRect follow those instractions:
```c#
 var joystick = new Joystick();
joystick.SetJoystickRect(new RectangleRect(joystick));
```

## Install
[How to install](https://github.com/uurha/BetterPluginCollection/wiki/How-to-install)
