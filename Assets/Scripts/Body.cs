using UnityEngine;
using System.Collections;

enum ScreenMode{
		Main,
		Setting,
		Text,
	} 
// main,text両方をまとめる
public class Body : Token {

	[SerializeField]
	MyCalendar calender;

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
					Setting.Instance.OnClickGoMain();
					break;
				case ScreenMode.Text:
					TodoText.Instance.GoBack(true);
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
		TodoText.Instance.gameObject.SetActive (true);
		_body.SlideIn(0, ()=>{
			Main.Instance.gameObject.SetActive (false);
			TodoText.GetInstance().InputField.SetVerticalScrollOffset(0);
		});
		screenMode = ScreenMode.Text;
	}

	public static void GoBoardMain(){
		Main.Instance.gameObject.SetActive (true);
		_body.SlideOut( () => {
			Setting.Instance.gameObject.SetActive (false);
			TodoText.Instance.gameObject.SetActive(false);
		});
		screenMode = ScreenMode.Main;
	}
	public static void GoBoardSetting(){
		Setting.Instance.gameObject.SetActive (true);
		_body.SlideIn(1, () => {
			Main.Instance.gameObject.SetActive (false);
		});
		screenMode = ScreenMode.Setting;
	}

	public void OnSwipeRight(){
		if (screenMode == ScreenMode.Main) {
			calender.OnClickPreMonth ();
		}
	}

	public void OnSwipeLeft(){
		if (screenMode == ScreenMode.Main) {
			calender.OnClickNextMonth ();
		}
	}
}
