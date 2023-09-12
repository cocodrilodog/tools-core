namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Utility class for <see cref="UnityEngine.EventSystems"/> related objects.
	/// </summary>
	public static class EventSystemUtility {


		#region Public Static Methods - Event Triggers

		/// <summary>
		/// Adds a listener to the <paramref name="eventTrigger"/>.
		/// </summary>
		/// <param name="eventTrigger"></param>
		/// <param name="type"></param>
		/// <param name="action"></param>
		public static void AddTriggerListener(
			EventTrigger eventTrigger,
			EventTriggerType type,
			UnityAction<BaseEventData> action
		) {
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = type;
			entry.callback.AddListener(action);
			EntriesByAction[action] = entry;
			eventTrigger.triggers.Add(entry);
		}

		/// <summary>
		/// Removes a listener from the <paramref name="eventTrigger"/>.
		/// </summary>
		/// <param name="eventTrigger"></param>
		/// <param name="action"></param>
		public static void RemoveTriggerListener(
			EventTrigger eventTrigger,
			UnityAction<BaseEventData> action
		) {
			EventTrigger.Entry entry = EntriesByAction[action];
			eventTrigger.triggers.Remove(entry);
			EntriesByAction.Remove(action);
		}

		#endregion


		#region Public Static Methods - Navigation

		/// <summary>
		/// Executes the submit event in the current selected game object.
		/// </summary>
		/// <remarks>
		/// If a button is selected, the click action of the button is performed.
		/// </remarks>
		public static void Submit() {
			if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) {
				Submit(EventSystem.current.currentSelectedGameObject);
			}
		}

		/// <summary>
		/// Executes the submit event in the provided <paramref name="gameObject"/>.
		/// </summary>
		/// <remarks>
		/// Normally, this would be used to make <c>Button</c>s to perform click.
		/// </remarks>
		/// <param name="gameObject">The gameObject.</param>
		public static void Submit(GameObject gameObject) {
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
		}

		/// <summary>
		/// Executes the cancel event in the current selected game object.
		/// </summary>
		public static void Cancel() {
			if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) {
				Cancel(EventSystem.current.currentSelectedGameObject);
			}
		}

		/// <summary>
		/// Executes the cancel event in the provided <paramref name="gameObject"/>.
		/// </summary>
		/// <param name="gameObject">The gameObject.</param>
		public static void Cancel(GameObject gameObject) {
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.cancelHandler);
		}

		/// <summary>
		/// If a UI is selected, selects another UI on the left side.
		/// </summary>
		public static void Left() {
			if (EventSystem.current == null) {
				return;
			}
			Selectable currentSelectable = null;
			if (EventSystem.current.currentSelectedGameObject != null) {
				currentSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			}
			if (currentSelectable != null) {
				Selectable leftSelectable = currentSelectable.FindSelectableOnLeft();
				if (leftSelectable != null) {
					EventSystem.current.SetSelectedGameObject(leftSelectable.gameObject);
				}
			}
		}

		/// <summary>
		/// If a UI is selected, selects another UI on the right side.
		/// </summary>
		public static void Right() {
			if (EventSystem.current == null) {
				return;
			}
			Selectable currentSelectable = null;
			if (EventSystem.current.currentSelectedGameObject != null) {
				currentSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			}
			if (currentSelectable != null) {
				Selectable rightSelectable = currentSelectable.FindSelectableOnRight();
				if (rightSelectable != null) {
					EventSystem.current.SetSelectedGameObject(rightSelectable.gameObject);
				}
			}
		}

		/// <summary>
		/// If a UI is selected, selects another UI above.
		/// </summary>
		public static void Up() {
			if (EventSystem.current == null) {
				return;
			}
			Selectable currentSelectable = null;
			if (EventSystem.current.currentSelectedGameObject != null) {
				currentSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			}
			if (currentSelectable != null) {
				Selectable upSelectable = currentSelectable.FindSelectableOnUp();
				if (upSelectable != null) {
					EventSystem.current.SetSelectedGameObject(upSelectable.gameObject);
				}
			}
		}

		/// <summary>
		/// If a UI is selected, selects another UI below.
		/// </summary>
		public static void Down() {
			if (EventSystem.current == null) {
				return;
			}
			Selectable currentSelectable = null;
			if (EventSystem.current.currentSelectedGameObject != null) {
				currentSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			}
			if (currentSelectable != null) {
				Selectable downSelectable = currentSelectable.FindSelectableOnDown();
				if (downSelectable != null) {
					EventSystem.current.SetSelectedGameObject(downSelectable.gameObject);
				}
			}
		}

		#endregion


		#region Private Static Fields

		private static Dictionary<UnityAction<BaseEventData>, EventTrigger.Entry> m_EntriesByAction;

		#endregion


		#region Private Static Properties

		private static Dictionary<UnityAction<BaseEventData>, EventTrigger.Entry> EntriesByAction {
			get {
				if (m_EntriesByAction == null) {
					m_EntriesByAction = new Dictionary<UnityAction<BaseEventData>, EventTrigger.Entry>();
				}
				return m_EntriesByAction;
			}
		}

		#endregion


	}

}