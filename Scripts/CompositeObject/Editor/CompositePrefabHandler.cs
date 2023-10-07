namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;

	[InitializeOnLoad]
	public static class CompositePrefabHandler {
		
		static CompositePrefabHandler() {
			//ObjectChangeEvents.changesPublished += ObjectChangeEvents_changesPublished;
			Undo.postprocessModifications += OnPostprocessModifications;
		}

		private static UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications) {
			for(int i = 0; i < modifications.Length; i++) {

				//Debug.Log("PostprocessModifications");

				//Debug.Log($"\tkeepPrefabOverride: {modifications[i].keepPrefabOverride}");

				//Debug.Log("\tPREVIOUS");
				//Debug.Log($"\tpreviousValue.target: {modifications[i].previousValue.target}");
				//Debug.Log($"\tpreviousValue.propertyPath: {modifications[i].previousValue.propertyPath}");
				//Debug.Log($"\tpreviousValue.value: {modifications[i].previousValue.value}");
				//Debug.Log($"\tpreviousValue.objectReference: {modifications[i].previousValue.objectReference}");

				//Debug.Log("\tCURRENT");
				//Debug.Log($"\tcurrentValue.target: {modifications[i].currentValue.target}");
				//Debug.Log($"\tcurrentValue.propertyPath: {modifications[i].currentValue.propertyPath}");
				//Debug.Log($"\tcurrentValue.value: {modifications[i].currentValue.value}");
				//Debug.Log($"\tcurrentValue.objectReference: {modifications[i].currentValue.objectReference}");

				var serializedObject = new SerializedObject(modifications[i].currentValue.target);
				serializedObject.Update();
				var property = serializedObject.FindProperty(modifications[i].currentValue.propertyPath);

				if(property != null) { // The property may be null when the object no longer exists
					var propertyType = CDEditorUtility.GetPropertyType(property);
					if (typeof(CompositeObject).IsAssignableFrom(propertyType)) {
						//Debug.Log($"+++{modifications[i].currentValue.target}: {modifications[i].currentValue.propertyPath}");
						//Debug.Log($"property.managedReferenceValue: {property.managedReferenceValue}");
						var component = modifications[i].currentValue.target as Component;
						if(component != null) {
							//Debug.Log($"GetPrefabAssetType(); {PrefabUtility.GetPrefabAssetType(component.gameObject)}");
							//Debug.Log($"IsPartOfAnyPrefab(); {PrefabUtility.IsPartOfAnyPrefab(component.gameObject)}");
							//Debug.Log($"IsAnyPrefabInstanceRoot(); {PrefabUtility.IsAnyPrefabInstanceRoot(component.gameObject)}");
							//Debug.Log($"GetPrefabInstanceStatus(); {PrefabUtility.GetPrefabInstanceStatus(component.gameObject)}");
							//Debug.Log($"GetCorrespondingObjectFromSource(); {PrefabUtility.GetCorrespondingObjectFromSource(component.gameObject)}");
							//Debug.Log($"GetCorrespondingObjectFromOriginalSource(); {PrefabUtility.GetCorrespondingObjectFromOriginalSource(component.gameObject)}");
							//Debug.Log($"GetCurrentPrefabStage(); {PrefabStageUtility.GetCurrentPrefabStage()}");
							//var prefabStage = PrefabStageUtility.GetPrefabStage(component.gameObject);
							//Debug.Log($"GetPrefabStage(); {prefabStage}");
							//if (prefabStage != null) {
							//	Debug.Log($"prefabStage.prefabContentsRoot; {prefabStage.prefabContentsRoot}");
							//}
							GameObject prefab = null;

							// This can be a prefab instance of the prefab itself, but in both cases this will find the prefab asset
							var prefabAssetType = PrefabUtility.GetPrefabAssetType(component.gameObject);
							if (prefabAssetType == PrefabAssetType.Regular) {
								prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(component.gameObject);
								if (prefab == component.gameObject) {
									//Debug.Log("Target is prefab");
								} else {
									//Debug.Log("Target is prefab instance");
								}
							}

							// If we are in prefab edit mode, the previous code won't find the prefab asset
							if (prefab == null) {
								var prefabStage = PrefabStageUtility.GetPrefabStage(component.gameObject);
								if(prefabStage != null) {
									var targetIsPrefabStageRoot = component.gameObject == prefabStage.prefabContentsRoot;
									if (targetIsPrefabStageRoot) {
										//Debug.Log($"Target is prefab stage root");
										prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
									}
								}
							}

							//Debug.Log($"Prefab: {prefab}");
							//Debug.Log($"Prefab instances:");

							if (prefab != null) {
								var prefabInstances = PrefabUtility.FindAllInstancesOfPrefab(prefab);
								foreach(var instance in prefabInstances) {
									//Debug.Log($"\t{instance}");
								}
							}

						}
					}
				}

				serializedObject.ApplyModifiedProperties();

			}
			return modifications;
		}

		private static void ObjectChangeEvents_changesPublished(ref ObjectChangeEventStream stream) {
			for(int i = 0; i < stream.length; i++) {

				Debug.Log(stream.GetEventType(i));
				
				switch (stream.GetEventType(i)) {

					case ObjectChangeKind.ChangeGameObjectOrComponentProperties: {
							stream.GetChangeGameObjectOrComponentPropertiesEvent(i, out var data);
							var obj = EditorUtility.InstanceIDToObject(data.instanceId);
							Debug.Log($"\t{obj.GetType()}");
							Debug.Log($"\tinstanceId: {data.instanceId}");
							Debug.Log($"\tscene: {data.scene.name}");
							break;
						}

					case ObjectChangeKind.UpdatePrefabInstances: {
							stream.GetUpdatePrefabInstancesEvent(i, out var data);
							var obj = EditorUtility.InstanceIDToObject(data.instanceIds[0]);
							Debug.Log($"\t{obj.GetType()}");
							Debug.Log($"\tinstanceId: {data.instanceIds.Length}");
							Debug.Log($"\tscene: {data.scene.name}");
							break;
						}

				}

			}
		}

	}

}