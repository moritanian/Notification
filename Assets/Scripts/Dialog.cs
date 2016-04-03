﻿using UnityEngine;
using System.Collections;

public delegate void YesCallBack(); 
public class Dialog : Token {

	TextObj text;
	// Use this for initialization
	void Start () {
		text = MyCanvas.Find<TextObj>("DialogText");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public YesCallBack _yesCallBack;

	public void OnClickYes(){
		_yesCallBack();
		Vanish();
	}
	public void OnClickCancel(){
		Vanish();
	}
}