using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;
using System;

// カレンダーのセルに割り当てる
public class CalendarCel : Token {

	static MyCalendar _mycalendar = null;
	 /// uGUI Button
  	Button _button = null;
  		[SerializeField]
  	Text _text;
  		[SerializeField]
  	ImageObj numImage;
  		[SerializeField]
  	Text numText;
  	
  	public string Label
  	{
  		get { return _text.text; }
  	}

  	// 祝日など
  	string event_text;
  	public string EventText{
  		get {return event_text;}
  	}

  	void SetEventText(){
  		MyCanvas.Find<Text>("EventText").text = event_text;
  	}

  	//日にちと休日の色分け
  	void SetLabel(string txt){
  		_text.text = txt;
  		string holiday_txt = Holiday.GetHoliday(_celTime);
  		
  		// event 表示
  		if(holiday_txt != ""){
  			event_text = holiday_txt;
  		} else{
  			event_text = "";
  		}

  		// 文字色変更
  		if(col == 0 || holiday_txt!= ""){	// 日曜祝日
  			_text.color = Color.red;
  		}else if(col == 6){					// 土曜
  			_text.color = Color.blue;
  		}else{								// 平日
  			_text.color = Color.black;
  		}
  	}

  	Image CelImage; 
	// 管理オブジェクト
	public static TokenMgr<CalendarCel> parent = null;

	// 存在する場所を表す
	int row{
		get;
		set;
	}
	int col{
		get;
		set;
	}

	// そのセルの状態　範囲外/ 通常 / 今日　/ 該当 / 今日であり、指し示されている
	enum Status{
		OutOfRange,
		Normal,
		Today,
		Pointed,
		TodayPointed
	}
	Status _status;
	void status(Status stat){
		_status = stat;
		switch(_status){
			case Status.OutOfRange:
				CelImage.color = new Color(255,255,255,0.5f);
				break;
			case Status.Today:
				CelImage.color = _mycalendar.TodayColor;
				break;
			case Status.Pointed:
				CelImage.color = _mycalendar.MyDayColor;
				break;
			case Status.TodayPointed:
				CelImage.color = _mycalendar.TodayMyColor;
				break;
			case Status.Normal:
			default:
				CelImage.color = new Color(255,255,255,1.0f);
				break;
		}
	}

	// これはMyCalendarの月が変更され、処理が必要か判別するため
	DateTime _celTime;
	
	public int Day{
		get {
			if(_status == Status.OutOfRange) return 0;
			return _celTime.Day;
		}
		// 
		set {
			if(value > 0){
				status(Status.Normal);
			//	Debug.Log("day "+ _celTime.ToString());
				_celTime = new DateTime(_celTime.Year,_celTime.Month, value);
				SetLabel(value.ToString());
			}else{
				SetLabel("");
				status(Status.OutOfRange);
			}
			
		}
	}
	

// monobehavior ではできない
/*
	// 静的コンストラクタ　instanceを最初につくる直前に一度だけよばれる。 
	static CalendarCel(){
		_mycalendar = MyCanvas.Find<MyCalendar>("MyCalendar");
	}
*/

	public static CalendarCel Add(float x,float y,int row,int col){
		CalendarCel obj = parent.Add(x,y);
		if(obj == null){
			Debug.Log("null TodoField作成できず");
			return null;
		}
		obj.row = row;
		obj.col = col;
		//MyCanvas.Set obj
	//	obj.transform.SetParent(MyCanvas.GetCanvas().transform, false);
		
		return obj;
	}

	void Awake(){
		if(_mycalendar == null)_mycalendar = MyCanvas.Find<MyCalendar>("MyCalendar");
		 _button = GetComponent<Button>();
		CelImage = GetComponent<Image>();
    	status(Status.OutOfRange);
    	_celTime = DateTime.Now;

    }

    void Start(){	
    	UpdateCel();
    } 
/*
	void Update(){
		// セルの変更が必要か
		if(_celTime.Month != _mycalendar.CelMonth || _celTime.Year != _mycalendar.CelYear ){
			UpdateCel();
		}
	}
*/

	// 全てのセルを更新する
	public static void AllUpdate(){
		DateTime Time = new DateTime(_mycalendar.ShowDateTime.Year, _mycalendar.ShowDateTime.Month,1);
		List<int> numbers = MyCanvas.Find<Main>("BoardMain").CalcTodoNumbers(Time);	
		parent.ForEachExists(cel => cel.UpdateCelWithNum(numbers));
	}

	public void UpdateCel(){
		_celTime = new DateTime(_mycalendar.ShowDateTime.Year, _mycalendar.ShowDateTime.Month,1);
		// 割り当ての日にち取得
		int new_day = GetMyDay();
		if(new_day>0){
			SetLabel(new_day.ToString());
			Day = new_day;
		}else{
			SetLabel("");
			Day = 0;
		}
		// 範囲外の時はここで処理終了 
		if(_status == Status.OutOfRange)return ;
		
		//　今日のセル、指定されたセル　の場合、status 更新
		UpDateStatus();
	}

	// status とcolor 変更
	void UpDateStatus(){
		DateTime today = DateTime.Now;
		if(IsDayEq(today, _celTime)){
			if(IsDayEq(_mycalendar.MyDateTime, _celTime))status(Status.TodayPointed);
			else status(Status.Today);
		}else if(IsDayEq(_mycalendar.MyDateTime, _celTime)){
			status(Status.Pointed);
		}
	}

	// セル表示+ その日のTodoの数
	public void UpdateCelWithNum(List<int> numbers){
		UpdateCel();
		if(_status != Status.OutOfRange){
			//Debug.Log("number " + (_celTime.Day -1).ToString());
			SetPoint(numbers[_celTime.Day -1 ]);
		}else{
			// todo数ないので非表示
			SetPoint(0);
		}
	}


	int GetMyDay(){
		//　1日より左側だった
		if(row == 0 && col < _mycalendar.FirstCol)return -1;

		int new_day = _mycalendar.DAYS_IN_WEEK *row + col - _mycalendar.FirstCol + 1;
		// 最後の日より後だった
		if(new_day> _mycalendar.LastDay)return -2;
		return new_day;
	}

	// セルが押された
	public void OnClickCel(){
		if(Day!= 0){
			// 時間更新
			//_celTime = _mycalendar.AddTime(_celTime);
			_mycalendar.MyDateTime = _celTime;
			//UpDateStatus();
			AllUpdate();
			_mycalendar.OnClickCel();
			//MyCanvas.Find<MyCalendar>("MyCalendar").ExitCalWithCallBack(_celTime);
			// イベントtext 表示
			SetEventText();
		}
	}

	// todo 数表示
	void SetPoint(int num){
		if(num>0){
			numText.enabled = true;
			numText.text = num.ToString();
			numImage.Visible = true;
		}else{ 					// 非表示 
			numText.enabled = false;
			numImage.Visible = false;
		}
	}
}
