namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;


	#region Small Types

	// TODO: This should be implemented by all objects that use tags, like CollisionTrigger,
	// DragObject
	public interface ITaggedObject {
		List<string> Tags { get; set; }
	}

	#endregion


	public class TaggedObject : MonoBehaviour, ITaggedObject {


		#region Public Properties

		public List<string> Tags {
			get => m_Tags;
			set => m_Tags = value;
		}

		#endregion


		#region Private Fields

		[CreateAsset]
		[SerializeField]
		private StringOptions m_TagOptions;

		[StringOptions(nameof(m_TagOptions))]
		[SerializeField]
		private List<string> m_Tags;

		#endregion


	}

}