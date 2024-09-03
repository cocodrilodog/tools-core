namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class ComponentOptionsAttribute : PropertyAttribute {

		public string[] ExludeTypes => m_ExcludeTypes;

		public ComponentOptionsAttribute() { }

		public ComponentOptionsAttribute(params string[] excludeTypes) => m_ExcludeTypes = excludeTypes;

		private string[] m_ExcludeTypes;
	
	}

}