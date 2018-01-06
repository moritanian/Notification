using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void DialogAction(); 
public class Dialog : Token {

	TextObj text;
	DialogAction cancelAction;

	// Use this for initialization
	void Start () {
	}
	void Awake(){
		
	}
	public override void Revive(){
		base.Revive();
		//text = transform.FindChild("Dialog/DialogText").gameObject.GetComponent<TextObj>();
		text = FindDescendant<TextObj>("DialogText");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public DialogAction _yesCallBack;

	public void Set(string title, DialogAction _yesCallBack){
		if(!this.gameObject.activeInHierarchy){
			Revive();
		}
		this._yesCallBack = _yesCallBack;
		Text = title;
		SetCancelAction (null);
	}

	public void SetCancelAction(DialogAction cancelAction){
		this.cancelAction = cancelAction;
	}

	public void OnClickYes(){
		_yesCallBack();
		Vanish();
	}
	public void OnClickCancel(){
		if (cancelAction != null) {
			cancelAction ();
		}
		Vanish();
	}
	public string Text{
		get{return text.Label;}
		set{text.Label = value;}
	}
}
