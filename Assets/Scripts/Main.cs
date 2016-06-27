using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Main : Token {

	DateTime crt_time;
	TextObj _title ;
	List<TodoData> Todos;
	
	Dropdown _dpDown; 
	// コンボボックス用選択肢列挙
	enum  SelectOpt{
		All,
		Today,
		ThisMonth,
		Previous,
		Future
	}

	readonly string[] japanese_week = {"日", "月", "火", "水", "木", "金", "土"};

	// 一覧表示するtodoにふくまれるかどうか判定するラムダ式を入れる型
	delegate bool IsContain(TodoData td);
	readonly IsContain AllContain = (td) => {return true;};  

	// スクロ＾ルcontent
	ScrollController today_scl;
	ScrollController todo_scl;
	Image todoadd_button;

	// Use this for initialization
	// コンポーネント取得はStartの前に処理する
	void Awake(){
		_title = MyCanvas.Find<TextObj>("MainTitle");
		TodoField.parent = new TokenMgr<TodoField>("TodoField",40);
		//TodayField.parent = new TokenMgr<TodayField>("TodayField", 10);
		_dpDown = MyCanvas.Find<Dropdown>("Dropdown");
		today_scl = MyCanvas.Find<ScrollController>("TodayContent");
		todo_scl = MyCanvas.Find<ScrollController>("TodoContent");
		todoadd_button = MyCanvas.Find<Image>("TodoAdd");
		 
	}

	void Start () {	

		// 祝日初期化
		Holiday.init();
		crt_time = DateTime.Now;
		_title.Label = crt_time.Month + "月" + crt_time.Day +"日 (" + japanese_week[(int)(crt_time.DayOfWeek)] + ") 今日のTodo"; 	
		Todos = TodoData.LoadAll();
		TodoData.SortByDate(Todos);
		// 全て表示
		Show(AllContain);
		MyCanvas.Find<MyCalendar>("MyCalendar").DispCal(DateTime.Now, (DateTime dt, bool IsSet) => MyCanvas.Find<Main>("BoardMain").ShowMyDay(dt));
	}

	void Update(){

	}

	public void OnclickTodoAdd(){
		//TodoAdd(DateTime.Now);
		SetTodoAddImg(false);
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(DateTime.Now, (DateTime _celTime, bool IsSet) => MyCanvas.Find<Main>("BoardMain").TodoAdd(_celTime,IsSet));
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	
	// 追加モードのカレンダーで日にち確定した際に呼ばれる
	public void TodoAdd(DateTime Dt, bool IsSet = true){
		SetTodoAddImg(true);
		// Dispモード指定
		MyCanvas.Find<MyCalendar>("MyCalendar").DispCal(DateTime.Now, (DateTime dt, bool _IsSet) => MyCanvas.Find<Main>("BoardMain").ShowMyDay(dt));
		if(!IsSet)return ;
		int id = TodoData.NewId(Todos);
		TodoData new_todo = new TodoData(id);
		//new_todo.UpdateCreate();
		Todos.Add(new_todo);
		TodoField _todoField = TodoField.Add(0,0,new_todo);
		todo_scl.SetContent(_todoField.transform);
		_todoField.Create(Dt);	//新規に作成時の処理
		// ドロップダウンのセレクト表示をAllにしておく
		_dpDown.value = (int)SelectOpt.All;
		// Dispモード指定
		MyCanvas.Find<MyCalendar>("MyCalendar").DispCal(DateTime.Now, (DateTime dt, bool _IsSet) => ShowMyDay(dt));
	}

	public void SetTodoAddImg(bool IsEnabled){
		todoadd_button.enabled = IsEnabled;

	}

	void Show (IsContain _eq){	
		//Debug.Log("Show AllCount:" + Todos.Count);
		// 破棄する前に場所をcanvas 直下に移動
		TodoField.parent.ForEachExists(t => t.transform.SetParent(MyCanvas.GetCanvas().transform,false));
		// 生存しているフィールドをすべて破棄
		TodoField.parent.Vanish();
		for(int i=0; i<Todos.Count; i++){
			if(!_eq(Todos[i]))continue;
			TodoField _todoField = TodoField.Add(0,0,Todos[i]);
			//_todoField.transform.SetParent(this.transform,false);
			todo_scl.SetContent(_todoField.transform);
			_todoField.SetText(Todos[i].Title);
			_todoField.IsNotify = Todos[i].IsNotify;
			_todoField.SetTimeText(Todos[i].TodoTime);
		}	
		//ShowToday();
	}

	public void ShowMyDay(DateTime dt){
		Show(GetEq(dt));
	}
	// 再読み込みして表示
	public void Reload(){
		// 読み込む前にセーブ対象をすべてセーブ
		Util.DoneSave();
		
		Todos = TodoData.LoadAll();
		TodoData.SortByDate(Todos);
		TodoData.LogAll(Todos);
		// 全て表示
		Show(AllContain);
		_dpDown.value = (int)SelectOpt.All;
		// カレンダーreload
		MyCanvas.Find<MyCalendar>("MyCalendar").DispCal(DateTime.Now, (DateTime dt, bool _IsSet) => MyCanvas.Find<Main>("BoardMain").ShowMyDay(dt));
	}

	// コンボボックスが変更された
	// todo一覧内容更新する
	public void OnChangeSelectOpt(){
		SelectOpt opt = (SelectOpt)GetSelectOpt();
		Debug.Log("OnChangeSelect" + opt.ToString());
		IsContain _eq = GetEq(opt);
		Show(_eq);
	}

	// TodoData の条件つき取得用の条件
	// delegate が返る
	IsContain GetEq(SelectOpt opt){
		IsContain _eq;
		switch(opt){
			case SelectOpt.All:
				_eq = AllContain;
				break;
			case SelectOpt.Today:
				_eq = (td) => {return(IsDayEq(td.TodoTime,DateTime.Now));};
				break;
			case SelectOpt.ThisMonth:
				_eq = (td) => {return (td.TodoTime.Year == DateTime.Now.Year 
									&& td.TodoTime.Month == DateTime.Now.Month);};
				break;
			case SelectOpt.Previous:
				_eq = (td) => {return (td.TodoTime.CompareTo(DateTime.Now) < 0);};
				break;
			case SelectOpt.Future:
				_eq = (td) => {return (td.TodoTime.CompareTo(DateTime.Now) > 0);};
				break;
			default:
				_eq = AllContain;
				break;
		}
		return _eq;
	}

	// 同じ日を検索するラムダ式取得
	IsContain GetEq(DateTime eq_date){
		IsContain _eq = (td) => {return (IsDayEq(td.TodoTime, eq_date));};
		return _eq;
	}

	// コンボボックスの値を取得
	int GetSelectOpt(){
		_dpDown = MyCanvas.Find<Dropdown>("Dropdown");
		return _dpDown.value;
	}

	/*
	// 上のボード、今日のtodo表示
	public void ShowToday(){
		ShowUpperBoard(SelectOpt.Today);
	}
	*/
/*
	void ShowUpperBoard(SelectOpt option){
		// 破棄する前に場所をcanvas 直下に移動
		TodayField.parent.ForEachExists(t => t.transform.SetParent(MyCanvas.GetCanvas().transform,false));
		// 生存しているフィールドをすべて破棄
		TodayField.parent.Vanish();

		IsContain _eq = GetEq(option);
		for(int i=0; i<Todos.Count; i++){
			if(!_eq(Todos[i]))continue;
			TodayField _todayField = TodayField.Add(0,0,Todos[i]);
			//_todoField.transform.SetParent(this.transform,false);
			today_scl.SetContent(_todayField.transform);
			_todayField.SetText(Todos[i].Title);
			_todayField.SetTimeText(Todos[i].TodoTime);
		}	
	}
*/
	public bool DeleteDataById(int id){
		return TodoData.DeleteDataById(Todos,id);
	}

	// 該当月のそれぞれの日の持つtodoの数をそれぞれ計算
	public List<int> CalcTodoNumbers(DateTime dt){
		int days_in_month = _days_in_month(dt);
		//Debug.Log("days_in_month" + days_in_month);
		List<int> numbers = new List<int>();
		for(int i=0;i<days_in_month; i++){
			numbers.Add(0);
		}
		for(int i=0;i<Todos.Count;i++){
			if(IsSameMonth(dt,Todos[i].TodoTime))numbers[Todos[i].TodoTime.Day - 1]++;
		}
		return numbers;
	}

	int _days_in_month(DateTime dt){
		return DateTime.DaysInMonth(dt.Year, dt.Month);
	}
	bool IsSameMonth(DateTime dt1, DateTime dt2){
		return (dt1.Year == dt2.Year && dt1.Month == dt2.Month);
	}



}
