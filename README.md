# Cocodrilo Dog Core

Cocodrilo Dog's core tools for general purpose.

## Attributes

The `Core` package has many attributes that improves development process in several areas. Please read below for details about the attributes.

### `[MinMaxRange]`
Creates a control that allows to set a range with sliders. It can be used in `FloatRange` and `IntRange` fields.
```
[MinMaxRange(0, 10)]
public FloatRange MinMaxRange;

[MinMaxRange(0, 10)]
public List<FloatRange> MinMaxRanges;
```
<img src="https://github.com/user-attachments/assets/4144a308-d5d0-4f97-8d3d-caa06913c8df" alt="image" width="600"/>

### `[CreateAsset]`
This allows to declare a field derived from `ScriptableObject` by adding a _Create_ button that makes the editor to create the correct type of asset for you. If there are more that one classes that are derived from the field class, a popup with the options will appear.
```
[CreateAsset]
public DerivedFromScriptableObject SomeScriptableObject;

[CreateAsset]
public List<DerivedFromScriptableObject> ManyScriptableObjects;
```
<img src="https://github.com/user-attachments/assets/1e718d94-ce02-4d8b-a0c3-9b30ecde4079" alt="image" width="600"/>
