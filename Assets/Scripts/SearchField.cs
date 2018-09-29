using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;


public class SearchField : MonoBehaviour {

	[SerializeField]
	Button clearButton;

	[SerializeField]
	UnityEvent OnSearch;

	NativeEditBox editBox;

	public string text
	{
		get { return editBox.text; }
		set
		{
			editBox.text = value;
		}
	}

	// Use this for initialization
	void Start () {
		clearButton.onClick.AddListener (OnClickClearButton);
		editBox = GetComponent<NativeEditBox> ();

		InputField inputField = GetComponent<InputField> ();
		inputField.onValueChange.AddListener (
			delegate{ OnChangeText();}
		);
		inputField.onEndEdit.AddListener (
			delegate { OnEndEdit(); }
		);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnChangeText(){
		clearButton.gameObject.SetActive (
			editBox.text.Length > 0
		);
	}

	void OnEndEdit(){
		OnSearch.Invoke ();
	}

	void OnClickClearButton(){
		editBox.text = "";
		clearButton.gameObject.SetActive (false);
		OnEndEdit ();
	}
}
