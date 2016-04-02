using UnityEngine;
using System.Collections;

public class Main : Token {

	private string datetimeStr;
	int year;
	int day;
	int month;
	int week;
	int hour;
	int min;
	int sec; 

	const int MAX_VIEW_TODOFIELD = 5;
	const int TODOFIELD_DIST = 20;
	const int TODOFIELD_POS_X = -3;
	const int TODOFIELD_INIT_POS_Y = -20;
	int crt_pos_y = TODOFIELD_INIT_POS_Y; 

	TextObj _title ;
	GameObject _prefab;
	// Use this for initialization
	void Start () {
		_title = MyCanvas.Find<TextObj>("Title");
		GetTime();
		_title.Label = month + "月" + day +"日" + "今日のTodo"; 
		
	}

	public void OnclickTodoAdd(){
		Debug.Log("TodoAdd");
		TodoAdd();
	}

	void TodoAdd(){
		TodoField _todoField = TodoField.Add(TODOFIELD_POS_X,crt_pos_y);
		crt_pos_y -= TODOFIELD_DIST;
		
	}

	
    

    void Update ()
    {
       
    }
	
	void GetTime(){
		datetimeStr = System.DateTime.Now.ToString();
       	year = System.DateTime.Now.Year;
       	day = System.DateTime.Now.Day;
       	month = System.DateTime.Now.Month;
       	
       // Debug.Log(datetimeStr);
	}
}
