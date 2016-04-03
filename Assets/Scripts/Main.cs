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
	void Start () {
		_title = MyCanvas.Find<TextObj>("MainTitle");
		crt_time = DateTime.Now;
		_title.Label = crt_time.Month + "月" + crt_time.Day +"日" + "今日のTodo"; 	
		//Todos = TodoData.LoadAll();
		// Todo 合計ではなく、idの最大取得するようにすべき
		//max_id = Todos.Count;
		TodoField.parent = new TokenMgr<TodoField>("TodoField",40);
		//ShowAll();
		//Reload();
	
	}

	public void OnclickTodoAdd(){
		Debug.Log("TodoAdd");
		TodoAdd();
	}

	public void OnClickGoSetting(){
		Body.GoBoardSetting();
	}

	void TodoAdd(){
		max_id++;
		TodoField _todoField = TodoField.Add(0,0,max_id);
		//_todoField.transform.SetParent(this.transform,false);
		ScrollController.SetContent(_todoField.transform);
		_todoField.Create(max_id);	//新規に作成時の処理
		
		
	}

	void ShowAll (){	
		// todo ソート
		Debug.Log("ShowAll count:" + Todos.Count);
		for(int i=0; i<Todos.Count; i++){
			TodoField _todoField = TodoField.Add(0,0,Todos[i].Id);
			//_todoField.transform.SetParent(this.transform,false);
			ScrollController.SetContent(_todoField.transform);
			_todoField.SetText(Todos[i].Title);
		}	
	}

	// 再読み込みして表示
	public void Reload(){
		// 破棄する前に場所をcanvas 直下に移動
		TodoField.parent.ForEachExists(t => t.transform.SetParent(MyCanvas.GetCanvas().transform,false));
		// 生存しているフィールドをすべて破棄
		TodoField.parent.Vanish();
		Todos = TodoData.LoadAll();
		ShowAll();
		max_id = Todos.Count;
	}

}
