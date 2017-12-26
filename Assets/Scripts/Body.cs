﻿using UnityEngine;
using System.Collections;

enum ScreenMode{
		Main,
		Setting,
		Text,
	} 
// main,text両方をまとめる
public class Body : Token {

	static PanelSlider _body;

	
	static ScreenMode screenMode;
	// いったんボタンが押された場合、次にボタンが押されたと判定するのはボタンが上がってから
	bool Is_Button = false;


	// Use this for initialization
	void Awake(){
		_body = GetComponent<PanelSlider>();
	}

	void Start () {
		screenMode = ScreenMode.Main;
	}

	void Update(){
		if(Util.BackButton() && !Is_Button){
			Is_Button = true;
			switch(screenMode){
				case ScreenMode.Main:
					Debug.Log("ApplicationFinish");
					Application.Quit();
					break;
				case ScreenMode.Setting:
					
					MyCanvas.Find<Setting>("BoardSetting").OnClickGoMain();
					break;
				case ScreenMode.Text:
					TodoText._todoText.GoBack(true);
					//MyCanvas.Find<TodoText>("BoardText").OnClickGoBack();
					break;
				default:
					break;
			}
		}else{
			if(Is_Button){
				StartCoroutine(Is_Button_True());
			}
		}
			
	}

	IEnumerator Is_Button_True(){
		yield return new WaitForSeconds(1.0f);
		Is_Button = false;
	}

	
	public static void GoBoardText(){
		_body.SlideIn();
		screenMode = ScreenMode.Text;
	}

	public static void GoBoardMain(){
		_body.SlideOut();
		screenMode = ScreenMode.Main;
	}
	public static void GoBoardSetting(){
		_body.SlideIn(1);
		screenMode = ScreenMode.Setting;
	}
}
