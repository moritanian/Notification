using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
public class TodoField : InfiniteScrollItemBase<TodoData> {

	// 編集画面に飛んでいいか
	private static bool _canGoEdit = true;
	// テキスト背景// 期限過ぎた場合は強調するなど

	[SerializeField]
	Image textImage;
	[SerializeField]
	Text titleText;
	[SerializeField]
	Color NormalColor;
	[SerializeField]
	Color SelectedColor;
	[SerializeField]
	Color MemoColor;

	[SerializeField]
	Image timeTextImage;
	[SerializeField]
	Color EnphasizedColor;
	[SerializeField]
	Color NormalTimeTextColor;
	[SerializeField]
	Text timeText;

	//public int id; // todo のid 本文との紐づけにも
	// todofield が　TodoDataを持つ
	public TodoData todoData;

	TouchScreenKeyboard keyboard;

	
	Toggle _toggle = null;
	public bool IsNotify{
		set { _toggle.isOn = value;}
		get {return _toggle.isOn;}
	}

	DateTime TodoDate;
	
	// android keyboard 用
	bool isTitleEdit;

	// Use this for initialization
	void Awake(){
		timeText.text = "";
		_toggle = transform.Find("Toggle").gameObject.GetComponent<Toggle>();
	}

	void Start(){

		
	}

	void Update(){
		// 現在時刻過ぎた場合は強調する
		SetTimeTextColor();

		if (isTitleEdit && keyboard != null &&  this.keyboard.done)  // キーボードが閉じた時
        {
			OnKeyboardClosed ();	
        }
	}

    public override void UpdateItem(TodoData todoData)
    {
        this.todoData = todoData;
        SetText(todoData.Title);
        IsNotify = todoData.IsNotify;
        SetTimeText(todoData.TodoTime);
    }

    void OnKeyboardClosed(){
		isTitleEdit = false;

		if (this.keyboard.wasCanceled) {
			Debug.Log("keyboard canceled!!");
			return;
		}

		Debug.Log("updated");
		string text = this.keyboard.text;
		todoData.UpdateTitle(text);
		SetText(text);

		if(IsNotify){
			// ローカル通知変更
			todoData.setCall();
		} 
	}

	public void SetText(string text){
		titleText.text = text;
	}

	public string GetText(){
		return titleText.text;
	}

	public void OnclickEdit(){
		if(_canGoEdit){
			_edit();
		}
	}
	// 本文を編集する
	void _edit(){
		TodoText.Instance.gameObject.SetActive (true);
		TodoText.Instance.SetUp(todoData);
		Body.GoBoardText();
	}

	// タイトル編集
	public void OnClickTitleEdit(){
		Main.Instance.SetTextDialog (GetText (), (text) => Modified(text));
	}

	void ShowTitleEditDialog(){
		Main.Instance.SetTextDialog (GetText (), (text) => Modified(text));
	}

	// タイトル編集完了ボタン
	public void Modified(string text){
		SetText (text);
		todoData.UpdateTitle(text);
		if(IsNotify){
			// ローカル通知変更
			todoData.setCall();
		}
	}

	public string TimeText{
		set { timeText.text = value;}
		get{ return timeText.text;}
	}
	// DateTime型からset
	public void SetTimeText(DateTime Dt){
		TodoDate = Dt;
		if(todoData.IsMemo){
			TimeText = "Memo";
			return ;
		}
		// 今年の分は年表示しない
		if(Dt.Year == DateTime.Now.Year){
			TimeText = Dt.ToString("MM/dd") + "\n" + Dt.ToString("HH:mm");
		} else {
			TimeText = Dt.ToString("yy/MM/dd") + "\n" + Dt.ToString("HH:mm");
		}
	}

	public void SetTime(DateTime Dt){
		todoData.UpdateTodoTime(Dt);
		// 表示の更新
		SetTimeText(Dt);
	}
	// 通知On/Off
	public void OnClickIsNotify(){
		todoData.UpdateIsNotify(IsNotify);
	}
	
	// 時間変更確定したときに呼ばれる
	public void TimeModified(DateTime Dt, bool IsSet = true){

		Main.Instance.SetTodoAddImg(true);
		if(!IsSet){
			// カレンダー設定
			Main.Instance.SetDispCal(Dt);
			return ;
		}
		SetTime(Dt);
		//mainBoard.ShowToday();
		if(IsNotify) {
			
			// ローカル通知設定変更
			todoData.setCall ();
		}

		Main.Instance.Restart();
	}
	// 時間ボタン
	public void OnClickTime(){
		// Memoの場合は設定できない
		if(todoData.IsMemo) return ;

		Main.Instance.SetTodoAddImg(false);
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(TodoDate, (DateTime _celTime, bool IsSet, bool IsMemo) => TimeModified(_celTime, IsSet));
	}

	// 長押し選択
	public void OnTouchSelected(){
		ShowDeleteDialog ();
	}

	public void OnTouch(){
		textImage.color = SelectedColor;
	}

	public void OnUnTouch(){
		textImage.color = NormalColor;
	}

	// 消去するかダイアログ表示
	void ShowDeleteDialog(){
		
		ToggleDialog toggleDialog = MyCanvas.FindChild<ToggleDialog>("ToggleDialog");
		List<string> titles = new List<string>{"delete", "memo"};
		List<string> texts = new List<string>{"削除します。よろしいですか？", "メモにします。よろしいですか？"};
		List<DialogAction> callBacks = new List<DialogAction>{
			() => {
				this.Delete();
				Main.Instance.SetSearchFieldVisibility(true);
			}, 
			() => {
				this.ChangeIsMemo(true);
				Main.Instance.SetSearchFieldVisibility(true);
			}
		};

		toggleDialog.Set(titles, texts, callBacks);
		toggleDialog.SetCancelAction (() => {
			Main.Instance.SetSearchFieldVisibility(true);	
		});

		Main.Instance.SetSearchFieldVisibility (false);
	}

	// 自分を消去(データごと)
	public void Delete(){
		// ローカル通知あれば削除
		if(IsNotify){
			if(todoData.deleteCall())
				Debug.Log("Successfully delete Local Call");
		}
		// Todo Mainがもってるデータ消去、保存データ消去
		Main.Instance.DeleteDataById(todoData.Id);
		// ファイル削除
		TodoText.DeleteFile(todoData.Id);
		Main.Instance.Restart();
		
	}

	// メモに
	public void ChangeIsMemo(bool isMemo = true){
		Debug.Log(isMemo.ToString() + "memo");
		todoData.UpdateIsMemo(isMemo);
		Main.Instance.Restart();		
	}

	// テキスト背景色変更 /
	void SetTimeTextColor(){
		if(todoData.IsMemo){
			timeTextImage.color = MemoColor;
			return ;
		}
		if(todoData.TodoTime.CompareTo(DateTime.Now) < 0 && !todoData.IsMemo)timeTextImage.color = EnphasizedColor;
		else timeTextImage.color = NormalTimeTextColor;
	}
	
}
