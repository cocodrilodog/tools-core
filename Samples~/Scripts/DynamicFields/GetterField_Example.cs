namespace CocodriloDog.Core.Examples {

	using UnityEngine;

	public class GetterField_Example : MonoBehaviour {


		#region Private Fields

		[SerializeField]
		private GetterField<string> m_PositionGetter;

		[SerializeField]
		private CompositeObjectReference<FileBase> m_Test;

		[SerializeReference]
		private FileBase m_TestFile;

		#endregion


	}

}
