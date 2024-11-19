namespace CocodriloDog.Core {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class SystemUtility {


		#region Public Static Methods

		/// <summary>
		/// Checks whether an object is array or list.
		/// </summary>
		/// <param name="obj">The object</param>
		/// <returns>Whether the object is array or list.</returns>
		public static bool IsArrayOrList(object obj) {
			if (obj == null) {
				return false;
			}
			return IsArrayOrList(obj.GetType());
			
		}

		/// <summary>
		/// Checks whether a type is array or list.
		/// </summary>
		/// <param name="type">The type</param>
		/// <returns>Whether the type is array or list.</returns>
		public static bool IsArrayOrList(Type type) {
			// Check if it's an array
			if (type.IsArray) {
				return true;
			}
			// Check if it's a List<>
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the element type of the provided <paramref name="arrayOrListType"/>
		/// </summary>
		/// <param name="arrayOrListType">An Array or List type.</param>
		/// <returns>The type of the elements of the array or list.</returns>
		public static Type GetElementType(Type arrayOrListType) {

			if (arrayOrListType == null) {
				throw new ArgumentNullException(nameof(arrayOrListType));
			}

			// If it's an array, get its element type
			if (arrayOrListType.IsArray) {
				return arrayOrListType.GetElementType();
			}

			// If it's a List
			if (arrayOrListType.IsGenericType) {

				// Get the generic type definition
				var genericTypeDef = arrayOrListType.GetGenericTypeDefinition();

				if (genericTypeDef == typeof(List<>)) {
					// Return the type argument of the generic type
					return arrayOrListType.GetGenericArguments()[0];
				}

			}

			// If it's not an array or a List, return null
			return null;

		}

		/// <summary>
		/// Checks whether a type is subclass of a generic type
		/// </summary>
		/// <param name="type">The type to check, for example <c>FileList</c></param>
		/// <param name="genericType">A generic type, for example <c>CompositeList&lt;&gt;</c></param>
		/// <returns><c>true</c> a <paramref name="type"/> is subclass of <paramref name="genericType"/></returns>
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

		/// <summary>
		/// Gets the non-abstract types derived from the provided <paramref name="type"/>. If the 
		/// <paramref name="type"/> is concrete, it will be included in the list.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>The list of concrete derived types.</returns>
		public static List<Type> GetConcreteDerivedTypes(Type type) {

			// Create the list
			var allConcreteSubtypes = new List<Type>();

			// Add the current type, if not abstract
			if (!type.IsAbstract) {
				allConcreteSubtypes.Add(type);
			}

			// Get all loaded types
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var allTypes = new List<Type>();
			foreach(var assembly in assemblies) {
				allTypes.AddRange(assembly.GetTypes());
			}

			// Find all subtypes that are concrete. This will include grand children, great grand
			// children, etc., because it is approving all that are assignable to the concreteType
			var concreteSubtypes = allTypes
				.Where(t => type.IsAssignableFrom(t) && t != type && !t.IsAbstract)
				.ToList();

			// Add them to the list
			allConcreteSubtypes.AddRange(concreteSubtypes);
			return allConcreteSubtypes;

		}

		#endregion


	}
}