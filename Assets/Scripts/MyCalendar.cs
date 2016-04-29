using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

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
	// 
	public DateTime MyDateTime;
	[SerializeField]
	public Color TodayColor;
	public Color MyDayColor;

	TextObj _dateText = null;

	public TimeInput _inputTime;


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
	

	// カレンダーの日にちを決定したときに処理される内容
	public delegate void CallBack(DateTime _time);
	CallBack _callBack;

	// Use this for initialization
	void Start () {
		_panelSlider = GetComponent<PanelSlider>();
		_showDateTime = DateTime.Now;
		_dateText = MyCanvas.Find<TextObj>("Date");

		InitCalendar();
		//SetCalendar();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoCal(DateTime PointedTime ,CallBack callBack){
		_panelSlider.SlideIn();
		_callBack = callBack;
		MyDateTime = PointedTime;
		_showDateTime = PointedTime;
		SetCalendar();
	}
	
	public void ExitCalWithCallBack(DateTime _celTime){
		_panelSlider.SlideOut();
		_callBack(_celTime);
	}
	public void ExitCal(){
		_panelSlider.SlideOut();
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
	void SetCalendar(){
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
		_dateText.Label = _showDateTime.Year.ToString() + "年" + _showDateTime.Month.ToString() + "月";

	}
	int _days_in_month(DateTime dt){
		return DateTime.DaysInMonth(dt.Year, dt.Month);
	}
	

	public void OnClickPreMonth(){
		_showDateTime = _showDateTime.AddMonths(-1);
		SetCalendar();
	}
	public void OnClickNextMonth(){
		_showDateTime = _showDateTime.AddMonths(1);
		SetCalendar();
	}

	// calendarell のceltime にinputboxの時間、分を入れる
	public DateTime AddTime(DateTime _celTime){
		int hour = _inputTime.GetHour();
		int min = _inputTime.GetMin();
		return new DateTime(_celTime.Year, _celTime.Month, _celTime.Day, hour, min, 0); 
	}
}
