# Cocodrilo Dog Core

Cocodrilo Dog's core tools for general purpose.

## Code Standard
A code standard guide for C# in Unity. 

This helps a team of developers to agree on the coding style and makes every script in a project to look as if it was created by the same person. This makes it very easy to understand the code of your teammates.

<img src="https://github.com/user-attachments/assets/b5755ab5-837f-4886-baba-fb0ee79ac465" alt="image" width="700"/>

## Attributes
A set of attributes that improves the quality of life of Unity developers.

<img src="https://github.com/user-attachments/assets/766bcd27-0235-4653-8c36-85b514f173da" alt="image" height="500"/>
<img src="https://github.com/user-attachments/assets/97f87cad-a657-41b2-9b24-95e1d6013318" alt="image" height="500"/>

## `CollisionTrigger`
<img src="https://github.com/user-attachments/assets/9adc377f-c71d-4e3b-80e9-229c5a1e34d9" alt="image" height="200"/>
<img src="https://github.com/user-attachments/assets/70bcbf89-f4c4-415e-9061-a91661a59ca4" alt="image" height="200"/>



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
<img src="https://github.com/user-attachments/assets/4144a308-d5d0-4f97-8d3d-caa06913c8df" alt="image" width="500"/>

### `[CreateAsset]`
This allows to declare a field derived from `ScriptableObject` by adding a _Create_ button that makes the editor to create the correct type of asset for you. If there are more that one classes that are derived from the field class, a popup with the options will appear.
```
[CreateAsset]
public DerivedFromScriptableObject SomeScriptableObject;

[CreateAsset]
public List<DerivedFromScriptableObject> ManyScriptableObjects;
```
<img src="https://github.com/user-attachments/assets/1e718d94-ce02-4d8b-a0c3-9b30ecde4079" alt="image" width="500"/>
