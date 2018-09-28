using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class InputFieldDialog : Dialog {

	[SerializeField]
	NativeEditBox nativeEditBox;
	//InputField inputField;

	[SerializeField]
	Button yesButton;

	[SerializeField]
	Button cancelButton;

	Action<string> _textCallback;

	public void Set(string title, Action<string> _textCallBack){
		if(!this.gameObject.activeInHierarchy){
			Revive();
		}
		this._textCallback = _textCallBack;
		Text = title;
		SetCancelAction (null);

		yesButton.gameObject.SetActive( false );
		cancelButton.gameObject.SetActive( true );

		nativeEditBox.SetFocus (true);
	}

	public void OnTextValueChanged(){
		Debug.Log ("textValueCganged");
		yesButton.gameObject.SetActive( true );
		cancelButton.gameObject.SetActive( false );
	}

	public override void OnClickYes(){
		_textCallback(Text);
		Vanish();
	}

	public string Text{
		//get{return inputField.text;}
		get{return nativeEditBox.text;}
		set{nativeEditBox.text = value;}
		//set{inputField.text = value;}
	}
}
