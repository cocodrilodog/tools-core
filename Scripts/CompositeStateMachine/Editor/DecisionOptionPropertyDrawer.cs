namespace CocodriloDog.Core {
	
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DecisionOption), true)]
	public class DecisionOptionPropertyDrawer : PropertyDrawerBase {


		#region Unity Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var height = base.GetPropertyHeight(property, label);
			if (Property.isExpanded) {
				height += EditorGUI.GetPropertyHeight(m_TriggerProperty) + 2;
				height += EditorGUI.GetPropertyHeight(m_StateProperty) + 2;
			}
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			
			base.OnGUI(position, property, label);
			
			Label = EditorGUI.BeginProperty(Position, Label, Property);

			var stateReferencePropertyType = CDEditorUtility.GetPropertyType(m_StateProperty);
			var stateType = stateReferencePropertyType.GetGenericArguments()[0];
			var state = CompositeObjectMaps.GetCompositeObjectById(
				m_StateSourceProperty.objectReferenceValue, 
				stateType,
				m_StateIdProperty.stringValue
			);

			var index = CDEditorUtility.GetElementIndex(Property);
			var triggerSource = m_TriggerProperty.FindPropertyRelative("m_Source").objectReferenceValue;
			var triggerId = m_TriggerProperty.FindPropertyRelative("m_Id").stringValue;
			var trigger = CompositeObjectMaps.GetCompositeObjectById<DecisionTrigger>(triggerSource, triggerId);
			var triggerName = trigger == null ? "[No Trigger]" : trigger.Name;
			var stateName = state == null ? "[Null]" : state.Name;
			var labelText = $"[{index}] {triggerName} → {stateName}";
			Label.text = labelText;

			EditorGUI.PropertyField(Position, Property, Label, true);

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


		#region Private Properties

		private SerializedProperty m_TriggerProperty;

		private SerializedProperty m_StateProperty;

		private SerializedProperty m_StateSourceProperty;

		private SerializedProperty m_StateIdProperty;

		#endregion


		#region Private Methods

		private void InitializeProperties() {
			m_TriggerProperty = Property.FindPropertyRelative("m_Trigger");
			m_StateProperty = Property.FindPropertyRelative("m_State");
			m_StateSourceProperty = Property.FindPropertyRelative("m_State.m_Source");
			m_StateIdProperty = Property.FindPropertyRelative("m_State.m_Id");
		}

		#endregion


	}

}