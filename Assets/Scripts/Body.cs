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
	// いったんボタンが押された場合、次にボタンが押されたと判定するのはボタンが上がってから
	bool Is_Button = false;


	// Use this for initialization
	void Awake(){
		_body = GetComponent<PanelSlider>();
	}

	void Start () {
		screen = Screen.Main;
	}

	void Update(){
		if(Util.BackButton() && !Is_Button){
			Is_Button = true;
			switch(screen){
				case Screen.Main:
					Debug.Log("ApplicationFinish");
					Application.Quit();
					break;
				case Screen.Setting:
					
					MyCanvas.Find<Setting>("BoardSetting").OnClickGoMain();
					break;
				case Screen.Text:
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
