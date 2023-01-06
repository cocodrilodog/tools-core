namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using CocodriloDog.Utility;

	[CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
	public class HorizontalLineDecoratorDrawer : DecoratorDrawer {


		#region Public Methods

		public override float GetHeight() {
			return 12;
		}

		#endregion


		#region Unity Methods

		public override void OnGUI(Rect position) {
			Rect rect = new Rect(position);
			rect.y += 8;
			CDEditorUtility.DrawHorizontalLine(rect);
		}

		#endregion


	}

}