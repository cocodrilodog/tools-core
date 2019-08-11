namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
	public class HorizontalLineDecoratorDrawer : DecoratorDrawer {


		#region Public Methods

		public override float GetHeight() {
			return 12;
		}

		public override void OnGUI(Rect position) {
			Rect rect = new Rect(position);
			rect.y += 8;
			rect.height = 2;
			GUI.Box(rect, GUIContent.none);
		}

		#endregion


	}

}