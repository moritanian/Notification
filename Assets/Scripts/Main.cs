using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

public class Main : Token {

	DateTime crt_time;
	int max_id;
	
	TextObj _title ;
	List<TodoData> Todos;

	// Use this for initialization
	// コンポーネント取得はStartの前に処理する
	void Awake(){
		_title = MyCanvas.Find<TextObj>("MainTitle");
		TodoField.parent = new TokenMgr<TodoField>("TodoField",40);
	}
	void Start () {	
		crt_time = DateTime.Now;
		_title.Label = crt_time.Month + "月" + crt_time.Day +"日" + "今日のTodo"; 	
		Todos = TodoData.LoadAll();
		// Todo 合計ではなく、idの最大取得するようにすべき
		max_id = Todos.Count;
		ShowAll();
		//Reload();	
	}



	public void OnclickTodoAdd(){
		//TodoAdd(DateTime.Now);
		MyCanvas.Find<MyCalendar>("MyCalendar").GoCal(_celTime => MyCanvas.Find<Main>("BoardMain").TodoAdd(_celTime));
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	

	public void TodoAdd(DateTime Dt){
		max_id++;
		TodoField _todoField = TodoField.Add(0,0,max_id);
		//_todoField.transform.SetParent(this.transform,false);
		ScrollController.SetContent(_todoField.transform);
		_todoField.Create(max_id);	//新規に作成時の処理
		_todoField.SetTime(Dt);
	}

	void ShowAll (){	
		// todo ソート
		Debug.Log("ShowAll count:" + Todos.Count);
		for(int i=0; i<Todos.Count; i++){
			TodoField _todoField = TodoField.Add(0,0,Todos[i].Id);
			//_todoField.transform.SetParent(this.transform,false);
			ScrollController.SetContent(_todoField.transform);
			_todoField.SetText(Todos[i].Title);
			_todoField.IsNotify = Todos[i].IsNotify;
			_todoField.SetTime(Todos[i].TodoTime);
		}	
	}

	// 再読み込みして表示
	public void Reload(){
		Util.DoneSave();
		// 破棄する前に場所をcanvas 直下に移動
		TodoField.parent.ForEachExists(t => t.transform.SetParent(MyCanvas.GetCanvas().transform,false));
		// 生存しているフィールドをすべて破棄
		TodoField.parent.Vanish();
		Todos = TodoData.LoadAll();
		ShowAll();
		max_id = Todos.Count;
	}


}
