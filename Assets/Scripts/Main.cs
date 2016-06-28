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
		PointedDay,
		ThisMonth,
		Previous,
		Future,
		Memo
	}

	readonly string[] japanese_week = {"日", "月", "火", "水", "木", "金", "土"};

	// 一覧表示するtodoにふくまれるかどうか判定するラムダ式を入れる型
	delegate bool IsContain(TodoData td);
	readonly IsContain AllContain = (td) => {return true;};  

	// スクロ＾ルcontent
	ScrollController today_scl;
	ScrollController todo_scl;
	Image todoadd_button;

	MyCalendar _mycalendar;

	// 追加モードで表示される時間
	int def_hour = 8;
	int def_min = 0;

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
		 _mycalendar = MyCanvas.Find<MyCalendar>("MyCalendar");
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
		_mycalendar.DispCal(DateTime.Now, (DateTime dt, bool IsSet) => MyCanvas.Find<Main>("BoardMain").ShowMyDay(dt));
	}

	void Update(){

	}

	// カレンダーを通常表示モードで反映
	public void SetDispCal(){
		SetDispCal(DateTime.Now);
	}
	public void SetDispCal(DateTime pt){
		_mycalendar.DispCal(pt, (DateTime dt, bool IsSet) => ShowMyDay(dt));
	}

	// カレンダーを時間設定モードで反映
	public void SetGoCal(){
		SetGoCal(DateTime.Now);
	}
	public void SetGoCal(DateTime pt){
		_mycalendar.GoCal(pt, (DateTime _celTime, bool IsSet) => TodoAdd(_celTime,IsSet));
	}
	
	public void OnclickTodoAdd(){
		SetTodoAddImg(false);

		SetGoCal(getDefaultTime(_mycalendar.MyDateTime));	
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	
	// 追加モードのカレンダーで日にち確定した際に呼ばれる
	public void TodoAdd(DateTime Dt, bool IsSet = true){
		SetTodoAddImg(true);
		if(!IsSet){
			// Dispモード指定
			_mycalendar.DispCal(_mycalendar.MyDateTime, (DateTime dt, bool _IsSet) => ShowMyDay(dt));
			return ;
		}
		int id = TodoData.NewId(Todos);
		TodoData new_todo = new TodoData(id);
		//new_todo.UpdateCreate();
		// Todo 配列に追加
		Todos.Add(new_todo);
		// todoField を生成
		TodoField _todoField = TodoField.Add(0,0,new_todo);
		// TododField をスクロールビューの子要素にする
		todo_scl.SetContent(_todoField.transform);
		_todoField.Create(Dt);	//新規に作成時の処理
		// ドロップダウンのセレクト表示をPointedDayにしておく
		_dpDown.value = (int)SelectOpt.PointedDay;
		// Dispモード指定
		_mycalendar.DispCal(Dt, (DateTime dt, bool _IsSet) => ShowMyDay(dt));
	}

	// todo 追加ボタンの表示、非表示
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
		// コンボボックス変更
		_dpDown.value = (int)SelectOpt.PointedDay;

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
		_mycalendar.DispCal(DateTime.Now, (DateTime dt, bool _IsSet) => MyCanvas.Find<Main>("BoardMain").ShowMyDay(dt));
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
			case SelectOpt.PointedDay:
				_eq = (td) => {return (IsSameDay(td.TodoTime, _mycalendar.MyDateTime));};
				break;
			case SelectOpt.Memo:
				_eq = (td) => {return (td.IsMemo);};
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
	bool IsSameDay(DateTime dt1, DateTime dt2){
		return (dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day);
	}

	// 新規作成の際の時間を設定したDateTimeを返す
	DateTime getDefaultTime(DateTime Dt){
		return new DateTime(Dt.Year, Dt.Month, Dt.Day, def_hour, def_min, 0);
	}

}
