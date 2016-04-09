using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
/*
時間記録には System.DateTime を使用
datetime.ToSing("yyyy/MM/dd HH:mm:ss") // 指定した形式で取得
datetime = DateTime.Now; //現在時刻
datetime = new DateTime("2015", "12","31");
datetime = new DateTime("2016","2","21","12","9","22");

datetime = DateTime.Parse()


*/

// タイトル
public class TodoField : Token {

	// 管理オブジェクト
	public static TokenMgr<TodoField> parent = null;
	static string Title = "";
	public InputField _inputField;

	public Text text;
	TextObj _timeText;

	public int id; // todo のid 本文との紐づけにも
	// todo idのわりあて方法考える
	enum Status{
		Inactive,	//利用なし
		Active,	//利用している
		Protected,	//利用しているが、表示に制限あり
	};
	
	Toggle _toggle = null;
	public bool IsNotify{
		set { _toggle.isOn = value;}
		get {return _toggle.isOn;}
	}

	public static TodoField Add(float x,float y,int id){
		//TodoField obj = CreateInstanceEasy<TodoField>("TodoField",x,y);
		TodoField obj = parent.Add(0,0);
		if(obj == null){
			Debug.Log("null TodoField作成できず");
			return null;
		}
		obj.id = id;
		return obj;
	}
	
	Status status = Status.Inactive;

	// Use this for initialization
	void Awake(){
		_timeText = transform.FindChild("Time/TimeText").gameObject.GetComponent<TextObj>();
		_timeText.Label = "";
		_toggle = transform.FindChild("Toggle").gameObject.GetComponent<Toggle>();
	}

	void Start(){

	}

	void Update(){
	}
	// 作成時の処理
	public void Create(int _id){
		string create_time = DateTime.Now.ToString();
		id = _id;
		status = Status.Active;
		Util.SaveData(TodoData._get_data_key(DataKeys.MaxId),id.ToString());
		
		Util.DoneSave();
		//再利用している場合、値が入っていることがあるため消去
		SetText("");
	}
	

	public string GetText(){
		return _inputField.text;
	}
	public void SetText(string text){
		_inputField.text = text;
	}

	public void OnclickEdit(){
		_edit();
	}

	void _edit(){
		Title = GetText();
		TodoText.TitleSet(Title);
		TodoText._todoText.Id = id;
		TodoText._todoText.SetUp();
		Body.GoBoardText();

	}

	// タイトル編集完了ボタン
	public void Modified(){
		TodoData.TitleModify(id,GetText());
	}

	public string TimeText{
		set { _timeText.Label = value;}
		get{ return _timeText.Label;}
	}
	// DateTime型からset
	public void SetTimeText(DateTime Dt){
		//TimeText = Dt.Year.ToString() + "\n" + Dt.Month.ToString() + "/" + Dt.Day;
		TimeText = Dt.ToString("yy/MM/dd") + "\n" + Dt.ToString("  HH:mm");

	}

	public void SetTime(DateTime Dt){
		Util.SaveData(TodoData._get_data_key(DataKeys.TodoTime,id),Dt.ToString());
		// 表示の更新
		SetTimeText(Dt);
	}
	// 通知On/Off
	public void OnClickIsNotify(){
		Util.SaveData(TodoData._get_data_key(DataKeys.IsNotify, id),IsNotify.ToString());
	}
	// 時間ボタン
	public void OnClickTime(){
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(_celTime => SetTimeText(_celTime));
	}
	
}
