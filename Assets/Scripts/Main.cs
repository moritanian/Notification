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
	// 一覧表示するtodoにふくまれるかどうか判定するラムダ式を入れる型
	delegate bool IsContain(TodoData td);
	readonly IsContain AllContain = (td) => {return true;};  

	// スクロ＾ルcontent
	ScrollController today_scl;
	ScrollController todo_scl;

	// Use this for initialization
	// コンポーネント取得はStartの前に処理する
	void Awake(){
		_title = MyCanvas.Find<TextObj>("MainTitle");
		TodoField.parent = new TokenMgr<TodoField>("TodoField",40);
		_dpDown = MyCanvas.Find<Dropdown>("Dropdown");
		today_scl = MyCanvas.Find<ScrollController>("TodayContent");
		todo_scl = MyCanvas.Find<ScrollController>("TodoContent");
		
	}

	void Start () {	

		crt_time = DateTime.Now;
		_title.Label = crt_time.Month + "月" + crt_time.Day +"日" + "今日のTodo"; 	
		Todos = TodoData.LoadAll();
		TodoData.SortByDate(Todos);
		// 全て表示
		Show(AllContain);
	}



	public void OnclickTodoAdd(){
		//TodoAdd(DateTime.Now);
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(DateTime.Now, _celTime => MyCanvas.Find<Main>("BoardMain").TodoAdd(_celTime));
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	
	// 追加モードのカレンダーで日にちを押したさいに呼ばれる
	public void TodoAdd(DateTime Dt){
		int id = TodoData.NewId(Todos);
		TodoData new_todo = new TodoData(id);
		//new_todo.UpdateCreate();
		Todos.Add(new_todo);
		TodoField _todoField = TodoField.Add(0,0,new_todo);
		todo_scl.SetContent(_todoField.transform);
		_todoField.Create(Dt);	//新規に作成時の処理
		// ドロップダウンのセレクト表示をAllにしておく
		_dpDown.value = (int)SelectOpt.All;
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
		ShowToday(GetEq(SelectOpt.Today));
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
		_dpDown.value = (int)SelectOpt.All;//
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

	// コンボボックスの値を取得
	int GetSelectOpt(){
		_dpDown = MyCanvas.Find<Dropdown>("Dropdown");
		return _dpDown.value;
	}

	// 上のボード、今日のtodo表示
	void ShowToday(IsContain _eq){
		for(int i=0; i<Todos.Count; i++){
			if(!_eq(Todos[i]))continue;
			TodoField _todoField = TodoField.Add(0,0,Todos[i]);
			//_todoField.transform.SetParent(this.transform,false);
			today_scl.SetContent(_todoField.transform);
			_todoField.SetText(Todos[i].Title);
			_todoField.IsNotify = Todos[i].IsNotify;
			_todoField.SetTimeText(Todos[i].TodoTime);
		}	
	}

	public bool DeleteDataById(int id){
		return TodoData.DeleteDataById(Todos,id);
	}



}
