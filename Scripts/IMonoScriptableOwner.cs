namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public interface IMonoScriptableOwner {


		#region Methods

		void RecreateMonoScriptableObjects();

		void ValidateMonoScriptableArrayOrLists();

		#endregion


	}

}
