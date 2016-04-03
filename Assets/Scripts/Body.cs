using UnityEngine;
using System.Collections;


// main,text両方をまとめる
public class Body : Token {

	static PanelSlider _body;
	// Use this for initialization
	void Start () {
		_body = GetComponent<PanelSlider>();
	}
	
	public static void GoBoardText(){
		_body.SlideIn();
	}

	public static void GoBoardMain(){
		_body.SlideOut();
	}
	public static void GoBoardSetting(){
		_body.SlideIn(1);
	}
}
