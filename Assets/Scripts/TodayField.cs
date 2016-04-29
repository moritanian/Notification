using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TodayField : Token {
// 管理オブジェクト
	public static TokenMgr<TodayField> parent = null;
	public InputField _inputField;

	// テキスト背景// 期限過ぎた場合は強調するなど
	
		[SerializeField]
	Image textImage;
		[SerializeField]
	Color NormalColor;
		[SerializeField]
	Color SelectedColor;
		[SerializeField]
	Color NormalTimeTextColor;
	//public Text text;
	TextObj _timeText;

	//public int id; // todo のid 本文との紐づけにも
	// todofield が　TodoDataを持つ
	public TodoData _todoData;

	// todo idのわりあて方法考える
	enum Status{
		Inactive,	//利用なし
		Active,	//利用している
		Protected,	//利用しているが、表示に制限あり
	};

	DateTime TodoDate;

	
	public static TodayField Add(float x,float y,TodoData todoData){
		//TodoField obj = CreateInstanceEasy<TodoField>("TodoField",x,y);
		TodayField obj = parent.Add(0,0);
		if(obj == null){
			Debug.Log("null TodayField作成できず");
			return null;
		}
		obj._todoData = todoData;
		return obj;
	}
	
	Status status = Status.Inactive;

	// Use this for initialization
	void Awake(){
		_timeText = transform.FindChild("Time/TimeText").gameObject.GetComponent<TextObj>();
		_timeText.Label = "";
	}

	public override void Revive(){
		base.Revive();
		
	}

	void Start(){

		
	}

	void Update(){
	
	}
	public string GetText(){
		return _inputField.text;
	}
	
	public void SetText(string text){
		if(_inputField == null) Debug.Log("SetText null!! ");
		if(_inputField.text  != null)_inputField.text = text;
	}

	public string TimeText{
		set { _timeText.Label = value;}
		get{ return _timeText.Label;}
	}
	// DateTime型からset
	public void SetTimeText(DateTime Dt){
		TodoDate = Dt;
		//TimeText = Dt.Year.ToString() + "\n" + Dt.Month.ToString() + "/" + Dt.Day;
		TimeText = Dt.ToString("yy/MM/dd") + "\n" + Dt.ToString("  HH:mm");
		// today ボード更新
		Main mainBoard = MyCanvas.Find<Main>("BoardMain");
		//mainBoard.ShowToday();
	}

	public void SetTime(DateTime Dt){
		_todoData.UpdateTodoTime(Dt);
		// 表示の更新
		SetTimeText(Dt);
	}
	
	// 時間ボタン
	public void OnClickTime(){
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(TodoDate,_celTime => SetTime(_celTime));
	}

	// 長押し選択
	public void TouchSelected(){
		textImage.color = SelectedColor;
	}
	public void UnTouch(){
		textImage.color = NormalColor;
	}
	
}
