using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

public class Main : Token {

	DateTime crt_time;
	const int MAX_VIEW_TODOFIELD = 5;
	const int TODOFIELD_DIST = 9;
	const int TODOFIELD_POS_X = -3;
	const int TODOFIELD_INIT_POS_Y = 3;
	int crt_pos_y = TODOFIELD_INIT_POS_Y; 
	int max_id;
	
	TextObj _title ;
	List<TodoData> Todos;

	// Use this for initialization
	void Start () {
		_title = MyCanvas.Find<TextObj>("MainTitle");
		crt_time = DateTime.Now;
		_title.Label = crt_time.Month + "月" + crt_time.Day +"日" + "今日のTodo"; 	
		Todos = TodoData.LoadAll();
		// Todo 合計ではなく、idの最大取得するようにすべき
		max_id = Todos.Count;
		ShowAll();
	
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
		TodoField _todoField = TodoField.Add(TODOFIELD_POS_X,TODOFIELD_INIT_POS_Y - TODOFIELD_DIST*(max_id-1),max_id);
		//_todoField.transform.SetParent(this.transform,false);
		ScrollController.SetContent(_todoField.transform);
		_todoField.Create(max_id);	//新規に作成時の処理
		crt_pos_y -= TODOFIELD_DIST;
		
	}

	void ShowAll (){	
		// todo ソート
		Debug.Log("ShowAll count:" + Todos.Count);
		for(int i=0; i<Todos.Count; i++){
			TodoField _todoField = TodoField.Add(TODOFIELD_POS_X,TODOFIELD_INIT_POS_Y - TODOFIELD_DIST*i,Todos[i].Id);
			//_todoField.transform.SetParent(this.transform,false);
			ScrollController.SetContent(_todoField.transform);
			_todoField.SetText(Todos[i].Title);
		}


		
	}

}
