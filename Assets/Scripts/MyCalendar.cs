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
	DateTime _dateTime;
	  [SerializeField]
	float CEL_WIDTH = 0; 
	  [SerializeField]
	float CEL_HEIGHT = 0;
	  [SerializeField]
	float FIRST_X = 0;
	  [SerializeField]
	public float FIRST_Y = 0;
	readonly public int DAYS_IN_WEEK = 7;
	readonly public int NUM_OF_ROW = 5;
	// 
	public DateTime MyDateTime;
	[SerializeField]
	public Color TodayColor;
	public Color MyDayColor;

	TextObj _dateText = null;


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
	int cel_month;
	public int CelMonth{
		get {return cel_month;}
	}
	int cel_year;
	public int CelYear{
		get {return cel_year;}
	}

	// カレンダーの日にちを決定したときに処理される内容
	public delegate void CallBack(DateTime _time);
	CallBack _callBack;

	// Use this for initialization
	void Start () {
		_panelSlider = GetComponent<PanelSlider>();
		_dateTime = DateTime.Now;
		_dateText = MyCanvas.Find<TextObj>("Date");

		InitCalendar();
		SetCalendar();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoCal(DateTime PointedTime ,CallBack callBack){
		_panelSlider.SlideIn();
		_callBack = callBack;
		MyDateTime = PointedTime;
		_dateTime = PointedTime;
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
		int days_in_month = DateTime.DaysInMonth(_dateTime.Year, _dateTime.Month);
		DateTime first_day = new DateTime(_dateTime.Year, _dateTime.Month,1);
		int first_week = (int)first_day.DayOfWeek; // １日の曜日

		// ここですべての要素処理するのではなく、それぞれでupdateに処理させるほうがいいかな
		/*
		for(int row=0; row<NUM_OF_ROW;row++){
			for(int col=0; col<DAYS_IN_WEEK; col++){
				;
			}
		}
		*/
		// cel に参照させるパラメータ
		first_col = first_week;
		last_col = (first_col + days_in_month) % DAYS_IN_WEEK;
		last_row = (first_col + days_in_month) / DAYS_IN_WEEK;
		last_day = days_in_month;
		//参照させるパラメータを変更してから変更するか判別するための月と年を変更する
		cel_month = _dateTime.Month;
		cel_year = _dateTime.Year;

		CalendarCel.parent.ForEachExists(cel => cel.UpdateCel());
		// 年月表示更新
		_dateText.Label = cel_year.ToString() + "年" + cel_month.ToString() + "月";

	}

	public void OnClickPreMonth(){
		_dateTime = _dateTime.AddMonths(-1);
		SetCalendar();
	}
	public void OnClickNextMonth(){
		_dateTime = _dateTime.AddMonths(1);
		SetCalendar();
	}
}
