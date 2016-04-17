using UnityEngine;
using System.Collections;

enum Screen{
		Main,
		Setting,
		Text,
	} 
// main,text両方をまとめる
public class Body : Token {

	static PanelSlider _body;

	
	static Screen screen;

	// Use this for initialization
	void Awake(){
		_body = GetComponent<PanelSlider>();
	}

	void Start () {
		screen = Screen.Main;
	}

	void Update(){
		if(Util.BackButton()){
			switch(screen){
				case Screen.Main:
					Application.Quit();
					break;
				case Screen.Setting:
					MyCanvas.Find<Setting>("BoardSetting").OnClickGoMain();
					break;
				case Screen.Text:
					MyCanvas.Find<TodoText>("BoardText").OnClickGoBack();
					break;
				default:
					break;
			}
		}
			
	}

	
	public static void GoBoardText(){
		_body.SlideIn();
		screen = Screen.Text;
	}

	public static void GoBoardMain(){
		_body.SlideOut();
		screen = Screen.Main;
	}
	public static void GoBoardSetting(){
		_body.SlideIn(1);
		screen = Screen.Setting;
	}
}
