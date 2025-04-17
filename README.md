# Cocodrilo Dog Core

Cocodrilo Dog's core tools for general purpose.

## How to Install

To install the `Core` package in your Unity project, open the Package Manager and click the plus button, "Add package from URL..." and the use this URLs:
https://github.com/cocodrilodog/tools-core.git

<img src="https://github.com/cocodrilodog/tools-motion-kit/assets/8107813/6408c284-330b-4abb-a075-5f5452841775" height="150">

<img src="https://github.com/cocodrilodog/tools-motion-kit/assets/8107813/57a94a49-0474-4744-88d7-f0835c7a6455" height="150">

## Code Standard

A code standard guide for C# in Unity. 

> **For more details:**
> <br/>
> See the ***CodeStandard_Example*** script from the samples of this package.

This helps a team of developers to agree on the coding style and makes every script in a project to look as if it was created by the same person. This makes it very easy to understand the code of your teammates.

<img src="https://github.com/user-attachments/assets/b5755ab5-837f-4886-baba-fb0ee79ac465" alt="image" width="700"/>

## Attributes

A set of attributes that improves quality of life for Unity developers.

> **For more details:**
> <br/>
> See the ***Attributes_Example*** scene from the samples of this package.

- `[MinMaxRange]` Inspector control to set ranges with sliders.
- `[HorizontalLine]` A decorative horizontal line in the inspector.
- `[CreateAsset]` Creates an asset of the type of the property, where the property derives from `ScriptableObject`.
- `[StringOptions]` On a string field, draws a popup with options to choose from.
- `[Help]` Draws a HelpBox based on a custom method.
- `[Hide]` Hides the field, depending on a custom method.
- `[Button]` Makes an inspector button that invokes the method below the attribute.
- `[ReadOnly]` Makes the field read-only.
- `[UnityEventGroup]` Groups multiple Unity events under a toolbar.

<img src="https://github.com/user-attachments/assets/766bcd27-0235-4653-8c36-85b514f173da" alt="image" height="450"/>
<img src="https://github.com/user-attachments/assets/97f87cad-a657-41b2-9b24-95e1d6013318" alt="image" height="450"/>

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
This allows to declare a field derived from `ScriptableObject` that has a _Create_ button which makes the editor to create the correct type of asset for you. If there are more that one classes that are derived from the field class, a popup with the options will appear.

```
[CreateAsset]
public DerivedFromScriptableObject SomeScriptableObject;

[CreateAsset]
public List<DerivedFromScriptableObject> ManyScriptableObjects;
```
<img src="https://github.com/user-attachments/assets/1e718d94-ce02-4d8b-a0c3-9b30ecde4079" alt="image" width="500"/>

## `CollisionTrigger` Component
This component helps to easily identify and react to collisions.

> **For more details:**
> <br/>
> See the ***Collision_Example*** scene from the samples of this package.

Normally developers would need to create some code in `MonoBehaviour`s implementing `OnCollision...` or `OnTrigger...` functions and comparing game object tags or any other identifying system in place, in order to achieve the desired collision results.

With the `CollisionTrigger`, you can decide what to do when an object collides with another object and react according to the nature of other object. And you can set all that in the inspector! 

For example, you can use a `CollisionTrigger` in a character and make the character gain gold when the it collides with coins, and lose health when it collides with spikes, without writing a single line of code related to the collision. All that can be setup in the inspector in a matter of seconds.

<img src="https://github.com/user-attachments/assets/9adc377f-c71d-4e3b-80e9-229c5a1e34d9" alt="image" width="400"/>
<br/>
<img src="https://github.com/user-attachments/assets/70bcbf89-f4c4-415e-9061-a91661a59ca4" alt="image" width="500"/>
<br/>
<img src="https://github.com/user-attachments/assets/9b6309e5-156e-4373-9ac9-d8c71556c445" alt="image" width="500"/>

## `CompositeObject`

Base class to create `System.Object`s that use the `[SerializeReference]` attribute. This class is suitable for creating composite structures like the one shown below, hence the name `CompositeObject`.

> **For more details:**
> <br/>
> See the ***Composite_Example*** scene from the samples of this package.

Derived classes from `CompositeObject` support polymorphism, which means that any field declared as `MyCompositeObject`, for example, can serialize data of any other class that derives from `MyCompositeObject`.

In the example of the images below, I created an abstract class `FileBase`:
```
[Serializable]
public abstract class FileBase : CompositeObject { }
```
Then I created a concrete class `TextFile`:
```
[Serializable]
public class TextFile : FileBase {

  [SerializeField]
  private string m_Text;

}
```
Additionally, I created a container class `Folder`:
```
[Serializable]
public class Folder : FileBase {

  [SerializeReference] // <- Notice the SerializeReference attribute.
  private List<FileBase> m_Files;

}
```
Finally, in the `MonoBehaviour` example class, I declared a `Folder` field , like this:
```
[SerializeReference] // <- Notice the SerializeReference attribute.
private Folder m_MyDisk;
```
In this example, any field declared as `FieldBase` can hold either a `Folder` or a `TextFile` instance, and a `Folder` can contain children `FileBase` instances which again, could be either `Folder` or `TextField`. This creates a **composite** structure.

In the images sequence below, I navigate throught the composite of folders and files:

<img src="https://github.com/user-attachments/assets/543ce11c-52d1-4283-98d1-a7bcf8400d9c" alt="image" width="500"/>
<br/>
<img src="https://github.com/user-attachments/assets/3b54db8d-0537-4b20-a1a4-e22964d09e9f" alt="image" width="500"/>
<br/>
<img src="https://github.com/user-attachments/assets/35632b85-79e2-44c0-a032-64f971b4dd26" alt="image" width="500"/>
<br/>
<img src="https://github.com/user-attachments/assets/756a3d05-94b5-4ca5-aa5b-cc67c0254d07" alt="image" width="500"/>
<br/>
<br/>
This last image shows how a new entry would be created:
<br/>
<img src="https://github.com/user-attachments/assets/7f09c699-c555-4ad6-849b-90af8f5b2150" alt="image" width="500"/>

This tech, was designed to be used in the `MotionKit` initially, but it has found its place in many other tools created by Cocodrilo Dog. It incorporates a breadcrumb structure in the inspector so that complex composite structures can be easily navigated.

## Composite State Machine

Base classes `MonoCompositeStateMachine`, `ScriptableCompositeStateMachine` and concrete classes `MonoFlowStateMachine` and `ScriptableFlowStateMachine` used to create state machines that uses the `CompositeObject` technology to create polymorphic states.

> **For more details:**
> <br/>
> See the ***CompositeStateMachine_Example*** scene from the samples of this package.

Example of a class derived from `MonoCompositeStateMachine`. This rotates a circle around a distant point, and depending on the angle, it will have one of three colors:

<img src="https://github.com/user-attachments/assets/a51d8380-ff51-4c5c-9577-692630410224" alt="image" height="160"/>
<img src="https://github.com/user-attachments/assets/aa29b121-09a8-4979-92b4-45bc7dbab7f5" alt="image" height="160"/>
<br/>

Example of `MonoFlowStateMachine` used to create a simple player mechanism:

<img src="https://github.com/user-attachments/assets/b33efc63-ab04-442b-81a0-a0ee00e26ab0" alt="image" width="400"/>
<br/>
<img src="https://github.com/user-attachments/assets/53e05b83-4d01-4b11-ac9d-92f6121b8be5" alt="image" width="400"/>



## Interface Field
## `MonoBehaviour`s
## `ScriptableRefrence` and `ScriptableValue`
