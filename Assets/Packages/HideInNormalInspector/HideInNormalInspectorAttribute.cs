/// @creator: Slipp Douglas Thompson
/// @license: WTFPL
/// @purpose: HideInNormalInspector attribute, to hide fields in the normal Inspector but let them show in the debug Inspector.
/// @why: Because this functionality should be built-into Unity.
/// @usage: Add `[HideInNormalInspector]` as an attribute to public fields you'd like hidden when Unity's Inspector is in “Normal” mode, but visible when in “Debug” mode.
/// @intended project path: Assets/Plugins/EditorUtils/HideInNormalInspectorAttribute.cs
/// @interwebsouce: https://gist.github.com/capnslipp/8138106

using UnityEngine;



public class HideInNormalInspectorAttribute : PropertyAttribute {}
