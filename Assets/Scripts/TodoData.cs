using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

// 保存するデータのキー
	public enum DataKeys{
		Title,	//タイトル文
		CreateTime,
		ModifiedTime,
		NotificationTime,
		TodoTime,
		MaxId,
		IsNotify
	}
//Todoの情報をもつ/ Todoの管理
public class TodoData {


	int id;
	public int Id{
		get {return id;}
		set {id = value;}
	}

	// コンストラクタ
	public TodoData(int id){
		Id = id;
	}
	// 以下、時間
	DateTime create_time;	// 作成日時
	DateTime modified_time;	//更新日時
	DateTime notification_time;	//通知する時間
	DateTime todo_time;	// 予定のある日程
	public DateTime TodoTime {
		get {return todo_time;}
		set {todo_time = value; }
	}
	
	string _title;
	public string Title{
		get {return _title;}
		set {_title = value;}
	}

	public bool IsNotify = false;


// 保存するデータのキー
	public static string _get_data_key(DataKeys key, int id = 0){
		switch(key){
			case DataKeys.Title:
				return "Todo_Title_id"+id;
			case DataKeys.CreateTime:
				return "Todo_CreateTime_id"+id;
			case DataKeys.ModifiedTime:
				return "Todo_ModifiedTime_id"+id;
			case DataKeys.TodoTime:
				return "Todo_TodoTime_id"+id;
			case DataKeys.NotificationTime:
				return "Todo_NotificationTime_id"+id;
			case DataKeys.MaxId:
				return "Todo_MaxId";
			case DataKeys.IsNotify:
				return "Todo_IsNotify" + id;	
		}
		return "";
	}
	/*
	// タイトルセーブ
	public static void TitleSave(int id,string title){
		Util.SaveData(_get_data_key(DataKeys.Title,id),title);
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id),DateTime.Now.ToString());
		Util.DoneSave();
	}
	// 更新時の処理
	public static void TitleModify(int id,string title){
		//modified_time = DateTime.Now.ToString();
		TitleSave(id,title);
	}
	*/
	// タイトル更新
	public void UpdateTitle(string new_title){
		Title = new_title;
		Util.SaveData(_get_data_key(DataKeys.Title,id),new_title);
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id),DateTime.Now.ToString());
	}

	public void UpdateTodoTime(DateTime Dt){
		Debug.Log("UpDateTodoTime " + Dt.ToString());
		todo_time = Dt;
		Util.SaveData(_get_data_key(DataKeys.TodoTime, id),Dt.ToString());
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id),DateTime.Now.ToString());
	}
	// 新規追加を保存
	public void UpdateCreate(){
		string create_time = DateTime.Now.ToString();
		Util.SaveData(_get_data_key(DataKeys.MaxId),id.ToString());
	}
	// Isnotify 更新
	public void UpdateIsNotify(bool _IsNotify){
		IsNotify = _IsNotify;
		Util.SaveData(_get_data_key(DataKeys.IsNotify, id),IsNotify.ToString());
	}


	public static List<TodoData> LoadAll(){
		List<TodoData> _todos = new List<TodoData>();
		string str = Util.LoadData(_get_data_key(DataKeys.MaxId).ToString());
		if(str == "") return _todos; //データなかった
		int head_id = int.Parse(str);
		if(head_id>0){
			for(int id= 1; id<=head_id; id++){
				string todo_time = Util.LoadData(_get_data_key(DataKeys.TodoTime,id));
				string title = Util.LoadData(_get_data_key(DataKeys.Title,id));
				string isNotifyStr = Util.LoadData(_get_data_key(DataKeys.IsNotify,id));
				TodoData todo = new TodoData(id);
				if(todo_time != "")todo.TodoTime = DateTime.Parse(todo_time);
				todo.Title = title;
				todo.IsNotify = (isNotifyStr == "True"); 
				_todos.Add(todo);
			}
		}
		return _todos;
	}
}
