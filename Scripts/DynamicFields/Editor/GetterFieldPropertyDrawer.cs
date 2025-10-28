namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UIElements;

	[CustomPropertyDrawer(typeof(GetterField<>), true)]
	public class GetterFieldPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return EditorGUIUtility.singleLineHeight + 2;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			base.OnGUI(position, property, label);

			Label = EditorGUI.BeginProperty(Position, Label, Property);

			// Object field
			EditorGUIUtility.labelWidth *= 0.7f;
			var objectRect = Position;
			objectRect.width *= 0.5f;
			objectRect.xMax -= 1;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(objectRect, m_ObjectProperty, Label);
			EditorGUIUtility.labelWidth = 0;

			// Path field
			var pathRect = Position;
			pathRect.xMin = objectRect.xMax + 2;

			// Generate the options
			var options = new List<string>(GetGetterOptions(m_ObjectProperty.objectReferenceValue));
			options.Insert(0, NoFunctionString); // Allow the first choice to be null
			options.Insert(1, "--"); // Add a separator

			// Draw the hierarchy dropdown with the options.
			EditorGUI.BeginDisabledGroup(m_ObjectProperty.objectReferenceValue == null);
			if (m_NewPath != null) {
				m_PathProperty.stringValue = m_NewPath;
				m_NewPath = null;
			}
			if (m_ObjectProperty.objectReferenceValue == null) {
				m_PathProperty.stringValue = null;
			}
			CDEditorGUI.HierarchyDropdown(
				id: Property.propertyPath, 
				rect: pathRect, 
				currentPath: m_PathProperty.stringValue, 
				pathStyle: PathStyle.SeparatedByPoint, 
				defaultPath: NoFunctionString, 
				allPaths: options, 
				onNewPath: (id, newPath) => {
					//monoBehavioursByCompositeObjectPath.TryGetValue(newPath, out var component);
					//m_NewChoice = new CompositeObjectData(id, newPath, component);
					Debug.Log(newPath);
					m_NewPath = newPath;
				}
			);
			EditorGUI.EndDisabledGroup();

			EditorGUI.EndProperty();

		}

		#endregion


		#region Protected Methods

		protected override void InitializePropertiesForGetHeight() {
			base.InitializePropertiesForGetHeight();
			InitializeProperties();
		}

		protected override void InitializePropertiesForOnGUI() {
			base.InitializePropertiesForOnGUI();
			InitializeProperties();
		}

		#endregion


		#region Private Fields

		private SerializedProperty m_ObjectProperty;

		private SerializedProperty m_PathProperty;

		private Dictionary<UnityEngine.Object, List<string>> m_GetterOptionsByObject = new();

		private string m_NewPath;

		private Type m_GenericType;

		protected const string NoFunctionString = "No Function";

		#endregion


		#region Private Methods

		private void InitializeProperties() {
			m_ObjectProperty = Property.FindPropertyRelative("m_Object");
			m_PathProperty = Property.FindPropertyRelative("m_Path");
		}

		private List<string> GetGetterOptions (UnityEngine.Object sourceObject) {

			if(sourceObject == null) {
				return new();
			}

			if (m_GetterOptionsByObject.TryAdd(sourceObject, new())) {

				if (sourceObject != null) {

					if (sourceObject is GameObject gameObject) {
						AddGetterOptionsFromGameObject(gameObject);
					} else if (sourceObject is Component component) {
						AddGetterOptionsFromGameObject(component.gameObject);
					} else {
						AddGetterOptionsFromComponentOrScriptableObject(sourceObject);
					}

					void AddGetterOptionsFromGameObject(GameObject gameObject) {
						var components = gameObject.GetComponents(typeof(Component));
						AddGetterOptionsFromComponentOrScriptableObject(gameObject);
						foreach (var comp in components) {
							AddGetterOptionsFromComponentOrScriptableObject(comp);
						}
					}

					void AddGetterOptionsFromComponentOrScriptableObject(UnityEngine.Object componentOrScriptableObject) {
						var methods = GetMethodsBySignature(componentOrScriptableObject.GetType(), GenericType);
						foreach (var getter in methods) {
							m_GetterOptionsByObject[sourceObject].Add($"{componentOrScriptableObject.GetType().Name}/{getter.Name}");
						}
						var properties = GetPropertiesByType(componentOrScriptableObject.GetType(), GenericType);
						foreach (var property in properties) {
							m_GetterOptionsByObject[sourceObject].Add($"{componentOrScriptableObject.GetType().Name}/{property.Name}");
						}
					}

				}

			}

			return m_GetterOptionsByObject[sourceObject];

		}

		private Type GenericType {
			get {
				if (m_GenericType == null) {
					var propType = CDEditorUtility.GetPropertyType(Property);
					m_GenericType = propType.GetGenericArguments()[0];
				}
				return m_GenericType;
			}
		}

		private MethodInfo[] GetMethodsBySignature(Type ownerType, Type returnType, Type parameterType = null) {

			return ownerType.GetMethods().Where((m) => {
				if (m.ReturnType != returnType || m.IsSpecialName) {
					return false;
				}

				var parameters = m.GetParameters();
				if (parameterType != null) {
					if (parameters.Length == 1 && parameterType == parameters[0].ParameterType) {
						return true;
					}
				} else {
					if (parameters.Length == 0) {
						return true;
					}
				}

				return false;

			}).ToArray();
		}

		private PropertyInfo[] GetPropertiesByType(Type ownerType, Type propertyType) {
			return ownerType.GetProperties().Where((p) => propertyType == p.PropertyType).ToArray();
		}

		#endregion


		#region Support Classes

		/// <summary>
		/// Data about a <see cref="CompositeObject"/> that is used to set a new <see cref="CompositeObjectReference{T}"/> 
		/// value asynchronously given the fact that the 
		/// <see cref="CDEditorGUI.HierarchyDropdown(string, Rect, string, List{string}, Action{string, string})"/>
		/// is asynchronous.
		/// </summary>
		public class GetterFieldData {

			/// <summary>
			/// The property path of the <see cref="CompositeObjectReference{T}"/> used as an Id.
			/// </summary>
			public string GetterField_PropertyPath;

			/// <summary>
			/// The path of the <see cref="CompositeObject"/>, from the root to itself.
			/// </summary>
			public string GetterFieldPath;

			/// <summary>
			/// An optional <see cref="UnityEngine.MonoBehaviour"/> used when the <see cref="m_SourceProperty"/> object is a 
			/// <see cref="GameObject"/> and needs to be changed to a <see cref="UnityEngine.MonoBehaviour"/> when the user
			/// chooses a <see cref="CompositeObject"/>.
			/// </summary>
			public Component MonoBehaviour;

			public GetterFieldData(
				string getterField_propertyPath,
				string getterFieldPath,
				MonoBehaviour monoBehaviour
			) {
				GetterField_PropertyPath = getterField_propertyPath;
				GetterFieldPath = getterFieldPath;
				MonoBehaviour = monoBehaviour;
			}

		}

		#endregion


	}

}