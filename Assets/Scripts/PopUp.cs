using UnityEngine;
using System.Collections;
using UnityEngine.Events;

// お知らせをonGuiで表示
public class PopUp : MonoBehaviour {
	
	enum PopUpStat{
		Off,
		Start,
		Finished
	} 
	PopUpStat _popupstat;

	public static PopUp _popup;
	float timer;
	float MaxTime = 1.0f;
	string _title = "";

	// instance　は一つだけを前提
	void Awake(){
		_popup = this;
	}

	// Use this for initialization
	void Start () {
		_popupstat = PopUpStat.Off;
	}
	
	// Update is called once per frame
	void Update () {
		if(_popupstat == PopUpStat.Start){
			timer += Time.deltaTime;
			if(timer > MaxTime){
				_popupstat = PopUpStat.Off;
			}
		}
	}

	void OnGUI(){
		if(_popupstat == PopUpStat.Start){
			int font_size;
			Rect drawArea;
			if(Util.isRunningOnAndroid()){
				drawArea = new Rect(100,500, 500f, 200f);
				font_size = 40;
			} else {
 				drawArea = new Rect(60,100, 100f, 50f);
				font_size = 15;
			} 
			GUI.Box(drawArea,"");
			GUILayout.BeginArea(drawArea);
			{
				GUIStyle style = new GUIStyle();
				style.wordWrap = true;
				style.fontSize = font_size;
				GUILayout.Label(_title, style);
			}
			GUILayout.EndArea();
		}
	}
	
	public static void PopUpStart(string title, float max_time ){
		PopUp._popup._popupStart(title, max_time);
	}

	public void _popupStart(string title, float max_time){
		MaxTime =  max_time;
		_title = title;
		_popupstat = PopUpStat.Start;
		timer = 0;

	}

}
