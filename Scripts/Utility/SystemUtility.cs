namespace CocodriloDog.Core {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class SystemUtility {


		#region Public Static Methods

		/// <summary>
		/// Check whether a type is subclass of a generic type
		/// </summary>
		/// <param name="type">The type to check, for example <c>FileList</c></param>
		/// <param name="genericType">A generic type, for example <c>CompositeList&lt;&gt;</c></param>
		/// <returns></returns>
		public static bool IsSubclassOfRawGeneric(Type type, Type genericType) {
			while (type != null && type != typeof(object)) {
				var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
				if (current == genericType) {
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		#endregion


	}
}