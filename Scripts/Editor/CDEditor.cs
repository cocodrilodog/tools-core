namespace CocodriloDog.Core {

	using UnityEditor;
	using UnityEngine;
	using System.Reflection;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// An editor that supports the <see cref="ButtonAttribute"/> and implements 
	/// <see cref="DrawProperty(UnityEditor.SerializedProperty)"/> so that individual
	/// properties can be overriden in subclasses.
	/// </summary>
	public abstract class CDEditor : Editor {


		#region Unity Methods

		protected virtual void OnEnable() {
			m_MethodsWithButtonByIndex = MethodsWithButtonUtility.GetMethodsWithButtonByIndex(target.GetType());
		}

		public override void OnInspectorGUI() {

			int index = 0;

			serializedObject.Update();

			var buttonIndices = new HashSet<int>();
			bool isHorizontalOpen = false;

			CDEditorUtility.IterateChildProperties(serializedObject, p => {
				if (p.propertyPath == "m_Script") {
					CDEditorUtility.DrawDisabledField(p);
				} else {
					DrawProperty(p);
				}
				if(m_MethodsWithButtonByIndex.TryGetValue(index, out var buttonMethodInfos)){

					var horizontalizeSameIndex = buttonMethodInfos.Any(bmi => bmi.HorizontalizeSameIndex);

					foreach(var buttonMethodInfo in buttonMethodInfos) {
						if (buttonIndices.Add(index) && horizontalizeSameIndex) {
							if (isHorizontalOpen) {
								EditorGUILayout.EndHorizontal();
								isHorizontalOpen = false;
							}
							EditorGUILayout.BeginHorizontal();
							isHorizontalOpen = true;
						}
						EditorGUI.BeginDisabledGroup(
							Application.isPlaying ? buttonMethodInfo.DisableInPlayMode : buttonMethodInfo.DisableInEditMode
						);
						if (GUILayout.Button(ObjectNames.NicifyVariableName(buttonMethodInfo.Method.Name))) {
							buttonMethodInfo.Method.Invoke(target, null);
						}
						EditorGUI.EndDisabledGroup();
					}
					if (isHorizontalOpen && horizontalizeSameIndex) {
						EditorGUILayout.EndHorizontal();
						isHorizontalOpen = false;
					}

				}
				index++;
			});

			serializedObject.ApplyModifiedProperties();

		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// Draws each of the properties of this object.
		/// </summary>
		/// <remarks>
		/// Override this to modify the drawing of specific properties.
		/// </remarks>
		/// <param name="property">The property</param>
		protected virtual void DrawProperty(SerializedProperty property) {
			EditorGUILayout.PropertyField(property);
		}

		#endregion


		#region Private Fields

		private Dictionary<int, List<ButtonMethodInfo>> m_MethodsWithButtonByIndex;

		#endregion


	}

}
