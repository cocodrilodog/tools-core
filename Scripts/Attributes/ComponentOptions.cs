namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class ComponentOptions : PropertyAttribute {

		public string[] ExludeTypes => m_ExcludeTypes;

		public ComponentOptions() { }

		public ComponentOptions(params string[] excludeTypes) => m_ExcludeTypes = excludeTypes;

		private string[] m_ExcludeTypes;
	
	}

}