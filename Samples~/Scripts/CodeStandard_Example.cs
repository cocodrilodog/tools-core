
// This script has the coding standard for classes created at Cocodrilo Dog Games SAS
// The following lines are how the classes should be organized by the use of regions.
// 
// Below that, there is an example class on how to implement most of the members and
// Some additional explanations.
// 
// The logic is that the top-most members are the most visible. For that reason the order
// is: public, unity methods, event handlers, protected and private. Inside each visibility 
// group of regions the order is: fields, properties, methods and events. Regarding static
// members, they always go before instance members in the same order and within the 
// corresponding visibility order. Constants are static, but not explicitly so the go 
// between static and instance members.

// This would be ordered regions by member type:

//	Small Types
//
//		Interfaces
//		Structs
//		Enums
//		Delegates
//
//	Public
//
// 		Static Fields
//		Static Properties
//		Static Methods
//		Static Events
//		
//		Constants
//
//		Fields
//		Properties
//		Constructors
//		Methods
//		Events
//
//	Unity Methods
//
//	Event Handlers
//
//	Protected
//
// 		Static Fields
//		Static Properties
//		Static Methods
//
//		Constants
//
//		Fields
//		Properties
//		Methods
//
//	Private
//
// 		Static Fields
//		Static Properties
//		Static Methods
//
//		Constants
//
//		Fields
//		Properties
//		Methods
//
// Support Classes

namespace CocodriloDog.Core.Examples {

	// Using statements go inside the namespace
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
						// <- Two spaces between regions and others
						//
	#region Small Types

	// Put here any small type like interfaces, structs, enums and delegates

	/// <summary>
	/// Interfaces start with I
	/// </summary>
	public interface ISomeInterface {
						
		void MethodA();
						// <- One space between class members
		void MethodB();

	}

	public struct SomeStruct {

		public float Number;

		public float OtherNumber;

	}

	public enum SomeEnum {
		Value1,
		Value2
	}
	
	public delegate void SomeDelegate();

	#endregion


	/// <summary>
	/// Alternatively, if an interface is more complex and the developer still wants to keep it in the
	/// same file, it won't be inside a Small types region, but rather have its own standard regions:
	/// Properties, Methods, Events, etc.
	/// </summary>
	public interface IBiggerInterface {


		#region Properties

		float Property1 { get; }

		float Property2 { get; }

		#endregion


		#region Methods

		void MethodA();
		
		void MethodB();

		#endregion


		#region Events

		event Action OnSomething;

		#endregion


	}


	public class CodeStandard_Example : MonoBehaviour {


		#region Public Static Fields
						// <- One space at beginning of region
		/// <summary>
		/// Use PascalCase for public static fields and properties.
		/// </summary>
		public static string SomeName;
						// <- One space at end of region
		#endregion


		#region Public Static Methods

		/// <summary>
		/// Use PascalCase for all methods.
		/// 
		/// One exception: When a singleton has a public static method <c>SomeMethod</c>,
		/// its instance private counterpart should be named <c>_SomeMethod</c>
		/// </summary>
		public static void DoSomeStaticAction() {

			// Always use braces

			// Correct:
			if(UnityEngine.Random.Range(0, 3) < 4) {
				Debug.LogFormat("True!");
			}

			// Incorrect:
			if (UnityEngine.Random.Range(0, 3) < 4)
				Debug.LogFormat("True!");

		}

		#endregion


		#region Public Fields

		// Optionally, use headers to differentiate type of fields. In this example
		// we use "References", "Values" and "Subcomponents"
		[Header("References")]

		/// <summary>
		/// Use PascalCase for public fields
		/// </summary>
		[SerializeField]
		public GameObject SomeReference;

		/// <summary>
		/// In this example, public fields under "References" are objects that are outside
		/// of this object as opposed to contained or owned by this object
		/// </summary>
		[SerializeField]
		public Renderer SomeOtherReference;

		[Header("Values")]

		/// <summary>
		/// In this example public fields of value type are grouped under "Values" category
		/// </summary>
		[SerializeField]
		public float SomeValue;

		/// <summary>
		/// All fields must have <c>SerializeField</c> or <c>NonSerialized</c>. 
		/// This prevents unintended behaviour when changing from public to private 
		/// and viceversa and gives the code clarity of intention.
		/// </summary>
		[SerializeField]
		public string SomeOtherValue;

		#endregion


		#region Public Properties

		/// <summary>
		/// Use PascalCase in all properties. Auto-properties are convinient some 
		/// times.
		/// </summary>
		/// <value>Some property.</value>
		public GameObject SomeProperty { get; set; }

		/// <summary>
		/// For subcomponents it is very common the need to expose them publicly for 
		/// read only.
		/// </summary>
		/// <value>Some subcomponent.</value>
		public Transform SomeSubcomponent { 
			get { return m_SomeSubcomponent; }
		}

		#endregion


		#region Public Constructors

		// Place here any constructor
		// They may be either public, protected or private

		#endregion


		#region Public Methods

		/// <summary>
		/// Visibility keywords: <c>public</c>, <c>protected</c>, <c>private</c>
		/// must be present always.
		/// </summary>
		public void SomeMethod(int someParam) {
			// Use camelCase for local variables and parameters
			int someVar = 0;
			someVar++;
		}

		#endregion


		#region Public Events

		/// <summary>
		/// Use PascalCase for all events
		/// </summary>
		public event SomeDelegate SomeEvent;

		#endregion


		#region Unity Methods

		private void Awake() { // Openning braces go in the same line

		}

		/// <summary>
		/// Correct formatting.
		/// </summary>
		private void OnEnable() {
			SomeMethod(0);
			SomeMethod(1);
		}

		/// <summary>
		/// Incorrect formatting.
		/// </summary>
		private void OnDisable() {
							// <- Don't add space when there is only one group of statements
			SomeMethod(0);
			SomeMethod(1);

		}

		/// <summary>
		/// Correct formatting.
		/// </summary>
		private void Start() {
							// <- Add space at the beginning
			SomeMethod(0);
			SomeMethod(1);
							// <- Add space when there is more than one group
			SomeMethod(2);
			SomeMethod(3);
							// <- Add space at the end
		}

		/// <summary>
		/// Incorrect formatting
		/// </summary>
		private void OnDestroy() {

			SomeMethod(0);
			SomeMethod(1);

			SomeMethod(2);
			SomeMethod(3);  // Don't break consistency. In this case by not adding
		}                   // space in the end

		private void Update() {
			Debug.LogFormat(
				"For long and or multiple parameters, the parentesis can be organized " +
				"this way {0}",
				SomeConstant
			);
		}

		#endregion


		#region Event Handlers

		/// <summary>
		/// For event handlers, use PascalCase and separate the name of the event
		/// dispatcher from the name of the event with an "_".
		/// </summary>
		public void Button_OnClick() {

		}

		private void Timer_OnComplete() {

		}

		#endregion


		#region Protected Fields

		/// <summary>
		/// All non-public instance fields use "m_" before the name in PascalCase.
		/// </summary>
		[NonSerialized]
		protected SomeEnum m_SomeProtcetedField;

		#endregion


		#region Protected Methods

		protected virtual void OverridableMethod() {

		}

		#endregion


		#region Private Static Fields

		/// <summary>
		/// All non-public static fields use "s_" before the name in PascalCase.
		/// </summary>
		private static SomeEnum s_SomeStaticField;

		#endregion


		#region Private Constants

		/// <summary>
		/// Use PascalCase in constants.
		/// </summary>
		private const float SomeConstant = 3.14f;

		#endregion


		#region Private Fields - Serialized

		// In this example, subcomponents are components that are children of this 
		// object.
		[Header("Subcomponents")]

		/// <summary>
		/// Create private serialized fields for Unity object references.
		/// </summary>
		[SerializeField]
		private Transform m_SomeSubcomponent;

		/// <summary>
		/// It is a good practice to declare the subcomponents as serializable variables
		/// rather than obtaining them via <see cref="Component.GetComponentInChildren{T}"/>
		/// because the user will know what is it there and what is missing.
		/// </summary>
		[SerializeField]
		private Rigidbody m_SomeOtherSubcomponent;

		#endregion


		#region Private Fields - Non Serialized

		/// <summary>
		/// When obtaining a sibling component via <see cref="Component.GetComponent{T}"/>
		/// it can be cached in a non - serialized field and obtained with a counterpart
		/// property like <see cref="CachedComponent"/>
		/// </summary>
		[NonSerialized]
		private Transform m_CachedComponent;

		/// <summary>
		/// When we need a list of subcomponents of certain type it is way more efficient
		/// to obtain them via <see cref="Component.GetComponentInChildren{T}"/> as in
		/// <see cref="SubcomponentsList"/> than serializing the list.
		/// 
		/// Though it is encouraged to implement a Helpbox in the inspector to inform 
		/// the user about the expected subcomponents collection.
		/// </summary>
		[NonSerialized]
		private Rigidbody[] m_SubcomponentsList;

		#endregion


		#region Private Properties

		/// <summary>
		/// Use this pattern to obtain components via <see cref="Component.GetComponent{T}"/>,
		/// <see cref="Component.GetComponentInChildren{T}"/>, etc.
		/// </summary>
		/// <value>The cached component.</value>
		private Transform CachedComponent {
			get {
				if(m_CachedComponent == null) {
					m_CachedComponent = GetComponent<Transform>();
				}
				return m_CachedComponent;
			}
		}

		private Rigidbody[] SubcomponentsList {
			get {
				if(m_SubcomponentsList == null) {
					m_SubcomponentsList = GetComponentsInChildren<Rigidbody>(true);
				}
				return m_SubcomponentsList;
			}
		}

		#endregion


		#region Support Classes

		/// <summary>
		/// Support classes, nested or not nested, can go in the last part of the file.
		/// </summary>
		[Serializable]
		public class SomeNestedClass {

			public int Value;

			public SomeNestedClass() {
				Value = UnityEngine.Random.Range(0, 100);
			}

		}

		#endregion


	}


	#region Support Classes

	/// <summary>
	/// Support classes, nested or not nested, can go in the last part of the file.
	/// </summary>
	[Serializable]
	public class SomeSupportClass {

		public int Value;

		public SomeSupportClass() {
			Value = UnityEngine.Random.Range(0, 100);
		}

	}

	#endregion


	/// <summary>
	/// If a support class is more complex, instead of writing it inside a Support Classes region, it
	/// can have its own standard regions: Public Fields, Public Methods, etc.
	/// </summary>
	[Serializable]
	public class SomeBiggerSupportClass {
		
		
		#region Public Fields
		
		public int Value;

		public int Value1;

		#endregion


		#region Constructors

		public SomeBiggerSupportClass() {
			Value = UnityEngine.Random.Range(0, 100);
		}

		#endregion


		#region Public Methods

		public void DoSomething() { }

		#endregion


	}

}