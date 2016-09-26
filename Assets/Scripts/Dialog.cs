using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void YesCallBack(); 
public class Dialog : Token {

	TextObj text;
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
	public YesCallBack _yesCallBack;

	public void Set(string title, YesCallBack _yesCallBack){
		if(!this.gameObject.activeInHierarchy){
			Revive();
		}
		this._yesCallBack = _yesCallBack;
		Text = title;
	}

	public void OnClickYes(){
		_yesCallBack();
		Vanish();
	}
	public void OnClickCancel(){
		Vanish();
	}
	public string Text{
		get{return text.Label;}
		set{text.Label = value;}
	}
}
