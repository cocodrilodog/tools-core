namespace CocodriloDog.Utility {

	using System;
	using UnityEngine;

	/// <summary>
	/// Utility class for all tasks related to the <see cref="UnityEngine"/> package.
	/// </summary>
	public static class Utility {


		#region Public Static Methods - Transform

		/// <summary>
		/// Traverses the transform hierarchy from child to parent.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <param name="action">Action to perform on each transform.</param>
		public static void TraverseTransform(Transform transform, Action<Transform> action) {
			foreach (Transform t in transform) {
				if (t.childCount > 0) {
					TraverseTransform(t, action);
				}
				action(t);
			}
		}

		#endregion


		#region Public Static Methods - Bounds

		/// <summary>
		/// Gets the combined bounds of the renderers of this game object.
		/// </summary>
		/// <returns>The combined bounds.</returns>
		/// <param name="gameObject">Game object.</param>
		public static Bounds GetCombinedBounds(GameObject gameObject) {
			return GetCombinedBounds(gameObject.GetComponentsInChildren<Renderer>(true));
		}

		/// <summary>
		/// Gets the combined bounds of the provided renderers.
		/// </summary>
		/// <returns>The combined bounds.</returns>
		/// <param name="renderers">Renderers.</param>
		public static Bounds GetCombinedBounds(Renderer[] renderers) {
			if (renderers != null && renderers.Length != 0) {
				var combinedBounds = renderers[0].bounds;
				for (int i = 1; i < renderers.Length; ++i) {
					combinedBounds.Encapsulate(renderers[i].bounds);
				}
				return combinedBounds;
			}
			return default(Bounds);
		}

		#endregion


		#region Public Static Methods - Screen/World

		/// <summary>
		/// Is the provided <paramref name="screenPosition"/> inside this gameObject?
		/// </summary>
		/// 
		/// <returns>
		/// <c>true</c>, if screen position is inside the <paramref name="gameObject"/>,
		/// <c>false</c> otherwise.
		/// </returns>
		/// 
		/// <param name="screenPosition">Screen position.</param>
		/// <param name="gameObject">The game object.</param>
		/// <param name="camera">The camera.</param>
		public static bool IsScreenPositionInside(
			Vector3 screenPosition, GameObject gameObject, Camera camera
		) {

			Vector3 distanceFromCamera = gameObject.transform.position - camera.transform.position;

			Vector3 worldTouch = camera.ScreenToWorldPoint(
				new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera.z)
			);

			Bounds bounds = GetCombinedBounds(gameObject);

			bool isTouchInside =
				worldTouch.x > bounds.min.x && worldTouch.x < bounds.max.x &&
				worldTouch.y > bounds.min.y && worldTouch.y < bounds.max.y;

			return isTouchInside;

		}

		/// <summary>
		/// Transform a screen position into world coordinates in the z position of the
		/// <paramref name="gameObject"/> as seen through the <paramref name="camera"/>
		/// </summary>
		/// <returns>The position to world.</returns>
		/// <param name="screenPosition">Screen position.</param>
		/// <param name="gameObject">Game object.</param>
		/// <param name="camera">Camera.</param>
		public static Vector3 ScreenPositionToWorld(
			Vector3 screenPosition, GameObject gameObject, Camera camera
		) {

			Vector3 distanceFromCamera = gameObject.transform.position - camera.transform.position;

			return camera.ScreenToWorldPoint(
				new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera.z)
			);

		}

		#endregion


	}
}