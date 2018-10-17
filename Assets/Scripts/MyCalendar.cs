using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/*
DayOfWeek 列挙体
Sunday :0 ~ 6
*/

public class MyCalendar : Token {

	PanelSlider _panelSlider;
	//表示する月と年指定用
	DateTime _showDateTime;
	public DateTime ShowDateTime{
		get {return _showDateTime;}
	}
	//指定された日時 
	DateTime _myDateTime;
	public DateTime MyDateTime {
		get {return _myDateTime;}
		set {_myDateTime = value;}
	}
	
	Image BattenButton;
	Image CheckButton;
	ImageObj MemoButton;
	Token EditDateTime;
	SwipeSlider dateTimeTextSlider;
	Text EventText; 

	  [SerializeField]
	float CEL_WIDTH = 0; 
	  [SerializeField]
	float CEL_HEIGHT = 0;
	  [SerializeField]
	float FIRST_X = 0;
	  [SerializeField]
	public float FIRST_Y = 0;
	readonly public int DAYS_IN_WEEK = 7;
		[SerializeField]
	readonly public int NUM_OF_ROW = 6;
	
	public Color TodayColor;
	public Color MyDayColor;
	public Color TodayMyColor;
	public Color NormalDayColor;
	public Color OutofRangeColor;
	public Color FontColor = Color.black;

	public TimeInput _inputTime;

	bool isDispMemoButton;

	// CalendarCelに参照させるパラメータ 読み取り専用
	// これらは　
	int first_col; //一日の行番号 
	public int FirstCol{
		get {return first_col;}
	}
	int last_col;	// 月最終日の行番号
	public int LastCol {
		get {return last_col;}
	}
	int last_row;	// 月最終日の列番号
	public int LastRow{
		get {return last_row;}
	}
	int last_day;
	public int LastDay{
		get {return last_day;}
	}
	
	public enum CalState{
		Disp,
		SetDate
	};
	CalState _calState;
	// カレンダーの日にちを決定したときに処理される内容
	public delegate void CallBack(DateTime _time, bool IsSet = true, bool IsMemo = false);
	CallBack _callBack;

	public CalendarCel PointedCel;

	void Awake(){
		BattenButton = MyCanvas.Find<Image>("BattenButton");
		CheckButton = MyCanvas.Find<Image>("CheckButton");
		MemoButton = MyCanvas.Find<ImageObj>("MemoButton");
		EditDateTime = MyCanvas.Find<Token>("EditDateTime");
		EventText = MyCanvas.Find<Text>("EventText");
		dateTimeTextSlider = MyCanvas.Find<SwipeSlider>("DateTimeTextSlider");
	}
	// Use this for initialization
	void Start () {
		_panelSlider = GetComponent<PanelSlider>();
		_showDateTime = DateTime.Now;
		InitCalendar();
		//SetCalendar();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoCal(DateTime PointedTime ,CallBack callBack, bool IsDispMemoButton = false){
		isDispMemoButton = IsDispMemoButton;
		// textFirldに反映
		_inputTime.SetTime(PointedTime);
		_calState = CalState.SetDate;
		_callBack = callBack;
		MyDateTime = PointedTime;
		_showDateTime = PointedTime;
		SetCalendar();
		SetCalImg(true);

	}

	// カレンダーをdisplayモードで表示
	public void DispCal(DateTime PointedTime, CallBack callBack){
		_calState = CalState.Disp;
		_callBack = callBack;
		SetCalImg(false);
		MyDateTime = PointedTime;
		_showDateTime = PointedTime;
		SetCalendar();
	}

	// 日にち確定button
	public void OnClickCheck(){
		MyDateTime = SetTime(MyDateTime);
		ExitCalWithCallBack(MyDateTime);
	}

	// メモ追加ボタン
	public void OnClickMemo(){
		ExitCalWithCallBack(MyDateTime, true, true);
	}
	
	public void ExitCalWithCallBack(DateTime _celTime, bool IsSet = true, bool IsMemo = false){
		//_panelSlider.SlideOut();
		_callBack(_celTime, IsSet, IsMemo);
		SetCalImg(false);
	}
	// ×ボタン
	public void OnClickExitCal(){
		ExitCalWithCallBack(MyDateTime, false);
	}

	void SetCalImg(bool IsSet){
		CheckButton.enabled = IsSet;
		BattenButton.enabled = IsSet;
		EventText.enabled = !IsSet;
		dateTimeTextSlider.gameObject.SetActive (!IsSet);
		if(IsSet){ // editmode
			EditDateTime.Revive();
			if(isDispMemoButton)MemoButton.Revive();
		}else{ // dispmode
			EditDateTime.Vanish();
			if(MemoButton.Exists)MemoButton.Vanish();
		}
	}
		
	void InitCalendar(){
		if(CalendarCel.parent == null){
			CalendarCel.parent = new TokenMgr<CalendarCel>("CalendarCel", DAYS_IN_WEEK * NUM_OF_ROW);
		}
		for(int row=0; row<NUM_OF_ROW;row++){
			for(int col=0; col<DAYS_IN_WEEK; col++){
				// セルを生成し、それをMyCalendar 以下におく
				CalendarCel.Add(col*CEL_WIDTH + FIRST_X, FIRST_Y - row * CEL_HEIGHT, row, col).transform.SetParent(this.transform, false);
			}
		}
	}

	public void SetCalendar(){
		// 表示する月の日数
		int days_in_month = _days_in_month(_showDateTime);
		DateTime first_day = new DateTime(_showDateTime.Year, _showDateTime.Month,1);
		int first_week = (int)first_day.DayOfWeek; // １日の曜日

		// cel に参照させるパラメータ
		first_col = first_week;
		last_col = (first_col + days_in_month) % DAYS_IN_WEEK;
		last_row = (first_col + days_in_month) / DAYS_IN_WEEK;
		last_day = days_in_month;

		CalendarCel.AllUpdate();
		// 年月表示更新
		if(_calState==CalState.Disp)
			UpdateDateTimeText();

		if(_calState == CalState.SetDate) SetEditDateTimeText();
	}

	// セルが押された際の処理 (テキスト変更, dispモードの場合、コールバック実行)
	public void OnClickCel(){
		if(_calState==CalState.Disp){
			UpdateDateTimeText ();
			_callBack(MyDateTime);
		}else{
			_showDateTime = MyDateTime;
			SetEditDateTimeText();

		}

	}

	void SetEditDateTimeText(){
		EditDateTime.GetComponent<Text>().text =  _showDateTime.Year.ToString() + "/" + _showDateTime.Month.ToString() + "/" + _showDateTime.Day.ToString();
	}

	int _days_in_month(DateTime dt){
		return DateTime.DaysInMonth(dt.Year, dt.Month);
	}

	public void OnClickPreMonth(){
		moveMonth (-1);
	}

	public void OnClickNextMonth(){
		moveMonth (1);
	}

	void moveMonth(int move){
		_showDateTime = _showDateTime.AddMonths(move);
		_showDateTime = _showDateTime.AddDays(- _showDateTime.Day + 1);
		SetCalendar();

		if(Main.Instance.IsNeedUpdateShowInMonthChange()){
			Main.Instance.ApplySelectOpt();
		}
	}

	// calendarell のceltime にinputboxの時間、分を入れる
	public DateTime SetTime(DateTime _celTime){
		int hour = _inputTime.GetHour();
		int min = _inputTime.GetMin();
		return new DateTime(_celTime.Year, _celTime.Month, _celTime.Day, hour, min, 0); 
	}

	// イベントテキスト値挿入
	public void SetEventText(string event_text){
  		MyCanvas.Find<Text>("EventText").text = event_text;
  	}

	string GetDateTimeText(DateTime d){
		return d.Year.ToString () + "年" + d.Month.ToString () + "月";
	}

	void UpdateDateTimeText(){
		dateTimeTextSlider.getCurrentItem ().GetComponent<Text> ().text = GetDateTimeText (_showDateTime);
	}

	// shuld be registered in swipeSlider
	public void OnSwipeSlideStart(SwipeSliderEventObj eventObj){
		// copy
		DateTime nextDateTime = _showDateTime;
		Debug.Log (-eventObj.slideDirection);
		nextDateTime = nextDateTime.AddMonths (- eventObj.slideDirection);

		eventObj.nextItem.GetComponent<Text> ().text = GetDateTimeText (nextDateTime);

	}

	// shuld be registered in swipeSlider
	public void OnSwipeSlideNext(SwipeSliderEventObj eventObj){

		if (eventObj.isAutoSlide) {
			moveMonth (-eventObj.slideDirection);
		}

	}
}

