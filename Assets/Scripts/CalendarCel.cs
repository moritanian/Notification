using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

// カレンダーのセルに割り当てる
public class CalendarCel : Token {

	static MyCalendar _mycalendar = null;
	 /// uGUI Button
  	Button _button = null;
  	Text _text = null;
  	public string Label
  	{
  		get { return _text.text; }
  		set { _text.text = value; }
  	}

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

	// これはMyCalendarの月が変更され、処理が必要か判別するため
	DateTime _celTime;
	// 日にちが割り当てられているか
	bool IsHasDay = false;
	
	public int Day{
		get {
			if(!IsHasDay) return 0;
			return _celTime.Day;
		}
		// 
		set {
			if(value > 0){
				IsHasDay = true;
				_celTime = new DateTime(_celTime.Year,_celTime.Month, value);
				Label = value.ToString();
			}else{
				Label = "";
				IsHasDay = false;
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

    	// 下の階層にあるTextを取得する
    	foreach(Transform child in transform) {
    		if(child.name.Contains("Text")) {
        // 対象のオブジェクトが見つかった
    			_text = child.GetComponent<Text>();
    			break;
    		}
    	}

    	_celTime = DateTime.Now;
    }

    void Start(){
    	UpdateCel();
    } 

	void Update(){
		// セルの変更が必要か
		if(_celTime.Month != _mycalendar.CelMonth || _celTime.Year != _mycalendar.CelYear ){
			UpdateCel();
		}
	}

	void UpdateCel(){
		_celTime = new DateTime(_mycalendar.CelYear, _mycalendar.CelMonth,1);
			
		// 割り当ての日にち取得
		int new_day = GetMyDay();
		if(new_day>0){
			Label = new_day.ToString();
			Day = new_day;
		}else{
			Label = "";
			Day = 0;
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
			//MyCanvas.Find<Main>("BoardMain").TodoAdd(_celTime);

			MyCanvas.Find<MyCalendar>("MyCalendar").ExitCalWithCallBack(_celTime);
		}
	}


}
