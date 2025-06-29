namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// Constructs and stores dictionaries containing references to <see cref="CompositeObject"/>s
	/// mapped by their corresponding paths on their root.
	/// </summary>
	[InitializeOnLoad]
	public static class CompositeObjectMaps {


		#region Static Constructor

		static CompositeObjectMaps() {
			
			ObjectChangeEvents.changesPublished -= ObjectChangeEvents_ChangesPublished;
			ObjectChangeEvents.changesPublished += ObjectChangeEvents_ChangesPublished;

			Selection.selectionChanged -= Selection_SelectionChanged;
			Selection.selectionChanged += Selection_SelectionChanged;

		}

		#endregion


		#region Public Static Methods

		/// <summary>
		/// Gets a dictionary with all the <see cref="CompositeObject"/>s of type <typeparamref name="T"/> mapped
		/// by their path in the <paramref name="root"/> object.
		/// </summary>
		/// <typeparam name="T">The type of <see cref="CompositeObject"/> we are looking for.</typeparam>
		/// <param name="root">The root object.</param>
		/// <returns>The dictionary.</returns>
		public static Dictionary<string, T> GetCompositeObjectsMap<T>(UnityEngine.Object root) where T : CompositeObject {
			var baseMap = GetCompositeObjectsMap(root, typeof(T));
			var concreteMap = new Dictionary<string, T>();
			foreach (var pathObject in baseMap) {
				concreteMap.Add(pathObject.Key, pathObject.Value as T);
			}
			return concreteMap;
		}

		/// <summary>
		/// Gets a dictionary with all the <see cref="CompositeObject"/>s of type <paramref name="subtypeFilter"/> mapped
		/// by their path in the <paramref name="root"/> object.
		/// </summary>
		/// <param name="root">The root object.</param>
		/// <param name="subtypeFilter">The subtype that we are looking for. It must be a <see cref="CompositeObject"/> or derived from it.</param>
		/// <returns>The dictionary.</returns>
		public static Dictionary<string, CompositeObject> GetCompositeObjectsMap(UnityEngine.Object root, Type subtypeFilter) {

			var completeMap = GetCompositeObjectsMap(root);
			if(completeMap == null) {
				return null;
			}

			var mapCopy = new Dictionary<string, CompositeObject>(completeMap);
			var compositeObjectsToRemove = mapCopy
				.Where(pathToCompositeObject => !subtypeFilter.IsAssignableFrom(pathToCompositeObject.Value.GetType()))
				.Select(pathToCompositeObject => pathToCompositeObject.Key)
				.ToList();

			foreach (var key in compositeObjectsToRemove) {
				mapCopy.Remove(key);
			}

			return mapCopy;

		}

		/// <summary>
		/// Gets a dictionary with all the <see cref="CompositeObject"/>s mapped by their path in the <paramref name="root"/> object.
		/// </summary>
		/// <param name="root">The root object.</param>
		/// <returns>The dictionary.</returns>
		public static Dictionary<string, CompositeObject> GetCompositeObjectsMap(UnityEngine.Object root) {

			if (root == null) {
				return null;
			}

			if (s_MapsByObject.ContainsKey(root)) {
				return s_MapsByObject[root];
			}

			// Create a new map if not available
			Dictionary<string, CompositeObject> map = new();

			// Start with the root node.
			InspectNode(root);

			void InspectNode(object compositeNode, string basePath = null) {

				// Get the type of the node
				var type = compositeNode.GetType();

				const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

				// Look for type and inherited types
				while (type != null && type != typeof(object)) {
					foreach (var field in type.GetFields(flags)) {

						bool isPublic = field.IsPublic;
						bool isSerialized = field.GetCustomAttribute<SerializeField>() != null;
						bool isSerializedReference = field.GetCustomAttribute<SerializeReference>() != null;

						if ((isPublic || isSerialized || isSerializedReference) && !field.IsStatic) {

							if (typeof(CompositeObject).IsAssignableFrom(field.FieldType)) {
								// Single instance
								var compositeObject = field.GetValue(compositeNode) as CompositeObject;
								if (compositeObject != null) {
									IncludeNode(compositeObject);
								}
							} else if (
								SystemUtility.IsSubclassOfRawGeneric(field.FieldType, typeof(CompositeList<>)) ||
								SystemUtility.IsSubclassOfRawGeneric(field.FieldType, typeof(List<>)) ||
								SystemUtility.IsArrayOfType(field.FieldType, typeof(CompositeObject))
							) {

								// CompositeList<> or List<> 
								Type elementType = null;
								if (field.FieldType.IsGenericType) {
									elementType = field.FieldType.GetGenericArguments()[0];
								}

								// Array
								if (field.FieldType.IsArray) {
									elementType = field.FieldType.GetElementType();
								}

								// Matching element type
								if (typeof(CompositeObject).IsAssignableFrom(elementType)) {
									foreach (var compositeObject in field.GetValue(compositeNode) as IEnumerable<CompositeObject>) {
										if (compositeObject != null) {
											IncludeNode(compositeObject);
										}
									}
								}

							}


						}
					}
					type = type.BaseType; // Move to parent class
				}

				void IncludeNode(CompositeObject compositeObject) {
					string path = null;
					if (string.IsNullOrEmpty(basePath)) {
						path = compositeObject.Name;
					} else {
						path = basePath + $"/{compositeObject.Name}";
					}
					map[path] = compositeObject;
					InspectNode(compositeObject, path); // Recursive
				}

			}

			return s_MapsByObject[root] = map;

		}

		/// <summary>
		/// Clear the <see cref="CompositeObject"/> map of the provided <paramref name="root"/> object.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static bool ClearCompositeObjectsMap(UnityEngine.Object root) => s_MapsByObject.Remove(root);
	
		/// <summary>
		/// Gets a <see cref="CompositeObject"/> of type <typeparamref name="T"/> in the <paramref name="root"/> with the
		/// provided <paramref name="id"/>.
		/// </summary>
		/// <typeparam name="T">The type of <see cref="CompositeObject"/></typeparam>
		/// <param name="root">The root Unity.Object where the <see cref="CompositeObject"/> was created on.</param>
		/// <param name="id">The unique id for the <see cref="CompositeObject"/></param>
		/// <returns>The <see cref="CompositeObject"/></returns>
		public static T GetCompositeObjectById<T>(UnityEngine.Object root, string id) where T : CompositeObject {
			return GetCompositeObjectById(root, typeof(T), id) as T;
		}

		/// <summary>
		/// Gets a <see cref="CompositeObject"/> in the <paramref name="root"/> with the provided <paramref name="id"/>.
		/// </summary>
		/// <param name="root">The root Unity.Object where the <see cref="CompositeObject"/> was created on.</param>
		/// <param name="subtypeFilter">The concrete type of the <see cref="CompositeObject"/> that we are looking for.</param>
		/// <param name="id">The unique id for the <see cref="CompositeObject"/></param>
		/// <returns></returns>
		public static CompositeObject GetCompositeObjectById(UnityEngine.Object root, Type subtypeFilter, string id) {
			var map = GetCompositeObjectsMap(root, subtypeFilter);
			if(map == null) {
				return null;
			}
			var compositeObject = map.FirstOrDefault(pathToCompositeObject => pathToCompositeObject.Value.Id == id).Value;
			return compositeObject;
		}

		#endregion


		#region Static Event Handlers

		private static void ObjectChangeEvents_ChangesPublished(ref ObjectChangeEventStream stream) {
			for (int i = 0; i < stream.length; ++i) {
				var type = stream.GetEventType(i);
				switch (type) {

					case ObjectChangeKind.ChangeGameObjectOrComponentProperties:
						stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var changeGameObjectOrComponent);
						var goOrComponent = EditorUtility.InstanceIDToObject(changeGameObjectOrComponent.instanceId);
						ClearCompositeObjectsMap(goOrComponent);
						break;

					case ObjectChangeKind.ChangeAssetObjectProperties:
						stream.GetChangeAssetObjectPropertiesEvent(i, out var changeAssetObjectPropertiesEvent);
						var changeAsset = EditorUtility.InstanceIDToObject(changeAssetObjectPropertiesEvent.instanceId);
						ClearCompositeObjectsMap(changeAsset);
						break;

				}
			}
		}

		private static void Selection_SelectionChanged() => s_MapsByObject.Clear();

		#endregion


		#region Private Static Fields

		private static Dictionary<UnityEngine.Object, Dictionary<string, CompositeObject>> s_MapsByObject = new();

		#endregion


	}

}