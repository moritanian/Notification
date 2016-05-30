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
	public InputField _inputField;

	// テキスト背景// 期限過ぎた場合は強調するなど
	
		[SerializeField]
	Image textImage;
		[SerializeField]
	Color NormalColor;
		[SerializeField]
	Color SelectedColor;

		[SerializeField]
	Image timeTextImage;
		[SerializeField]
	Color EnphasizedColor;
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
	
	Toggle _toggle = null;
	public bool IsNotify{
		set { _toggle.isOn = value;}
		get {return _toggle.isOn;}
	}

	DateTime TodoDate;
	
	public static TodoField Add(float x,float y,TodoData todoData){
		//TodoField obj = CreateInstanceEasy<TodoField>("TodoField",x,y);
		TodoField obj = parent.Add(0,0);
		if(obj == null){
			Debug.Log("null TodoField作成できず");
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
		_toggle = transform.FindChild("Toggle").gameObject.GetComponent<Toggle>();
	}

	public override void Revive(){
		base.Revive();
		
	}

	void Start(){

		
	}

	void Update(){
		// 現在時刻過ぎた場合は強調する
		SetTimeTextColor();
	}
	// 作成時の処理
	public void Create(DateTime Dt){
		//再利用している場合、値が入っているこ
		SetTime(Dt);
		_todoData.UpdateCreate();
		status = Status.Active;
		//再利用している場合、値が入っていることがあるため消去
		Debug.Log("Created!! ");
		SetText("");
		Main mainBoard = MyCanvas.Find<Main>("BoardMain");
		mainBoard.ShowToday();
	}
	

	public string GetText(){
		return _inputField.text;
	}
	
	public void SetText(string text){
		if(_inputField == null) Debug.Log("SetText null!! ");
		if(_inputField.text  != null)_inputField.text = text;
	}

	public void OnclickEdit(){
		_edit();
	}
	// 本文を編集する
	void _edit(){
		TodoText.SetUp(this);
		Body.GoBoardText();
	}

	// タイトル編集完了ボタン
	public void Modified(){
		_todoData.UpdateTitle(GetText());
		Main mainBoard = MyCanvas.Find<Main>("BoardMain");
		mainBoard.ShowToday();
		if(IsNotify){
			// ローカル通知変更
			setCall();
		}
	}

	public string TimeText{
		set { _timeText.Label = value;}
		get{ return _timeText.Label;}
	}
	// DateTime型からset
	public void SetTimeText(DateTime Dt){
		TodoDate = Dt;
		//TimeText = Dt.Year.ToString() + "\n" + Dt.Month.ToString() + "/" + Dt.Day;
		// 今年の分は年表示しない
		if(Dt.Year == DateTime.Now.Year){
			TimeText = Dt.ToString("  MM/dd") + "\n" + Dt.ToString("  HH:mm");
		} else {
			TimeText = Dt.ToString("yy/MM/dd") + "\n" + Dt.ToString("  HH:mm");
		}
		// today ボード更新
		Main mainBoard = MyCanvas.Find<Main>("BoardMain");
	//	mainBoard.ShowToday();
	}

	public void SetTime(DateTime Dt){
		_todoData.UpdateTodoTime(Dt);
		// 表示の更新
		SetTimeText(Dt);
	}
	// 通知On/Off
	public void OnClickIsNotify(){
		bool result = _todoData.UpdateIsNotify(IsNotify);
		// ローカル通知セット or リセット　する
		if(result){
			if(IsNotify)setCall();
			else{
				if(deleteCall())
					Debug.Log("Successfully delete call");
			}
		}
		// 過去であるため通知onにできない
		if(result == false && IsNotify == true ){
			IsNotify = false;
		}
	}
	
	public void TimeModified(DateTime Dt){
		SetTime(Dt);
		Main mainBoard = MyCanvas.Find<Main>("BoardMain");
		mainBoard.ShowToday();
		if(IsNotify){
			if(_todoData.TodoTime.CompareTo(DateTime.Now) > 0){
				// ローカル通知設定変更
				setCall();
				}else{
					// 時間変更で過去になった場合、falseに
					deleteCall();
					_todoData.UpdateIsNotify(false);
				}
		}
	}
	// 時間ボタン
	public void OnClickTime(){
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(TodoDate,_celTime => TimeModified(_celTime));
	}

	// 長押し選択
	public void TouchSelected(){
		textImage.color = SelectedColor;
	}
	public void UnTouch(){
		textImage.color = NormalColor;
	}

	// 消去するかダイアログ表示
	public void AppDeleteDailog(){
		Dialog dialog = MyCanvas.FindChild<Dialog>("DialogBack");
		dialog.Revive();
		dialog.Text = "削除する";
		dialog._yesCallBack = new YesCallBack(this.Delete);
	}

	// 自分を消去(データごと)
	public void Delete(){
		Main mainBoard = MyCanvas.Find<Main>("BoardMain");
		// ローカル通知あれば削除
		if(IsNotify){
			if(deleteCall())
				Debug.Log("Successfully delete Local Call");
		}
		// Todo Mainがもってるデータ消去、保存データ消去
		mainBoard.DeleteDataById(_todoData.Id);
		// ファイル削除
		TodoText.DeleteFile(_todoData.Id);
		mainBoard.ShowToday();
		Vanish();
		
	}

	// テキスト背景色変更 /
	void SetTimeTextColor(){
		
		if(_todoData.TodoTime.CompareTo(DateTime.Now) < 0)timeTextImage.color = EnphasizedColor;
		else timeTextImage.color = NormalTimeTextColor;
	}

	// ローカル通知を登録
	bool setCall(){
		// 過去の場合
		if(_todoData.TodoTime.CompareTo(DateTime.Now) < 0){
			IsNotify = false;
			return false;
		}
		AndroidSamp.LocalCallSet(_todoData.Id, _todoData.TodoTime, "Notify"+ _todoData.Id.ToString(), "title", _todoData.Title);
		return true;
	}

	// ローカル通知を削除
	bool deleteCall(){
		return AndroidSamp.LocalCallReset(_todoData.Id);
	}
	
}
