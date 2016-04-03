﻿using UnityEngine;
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
		MaxId
	}
//Todoの情報をもつ/ Todoの管理
public class TodoData {


	int id;
	public int Id{
		get {return id;}
		set {id = value;}
	}

	// 以下、時間
	DateTime create_time;	// 作成日時
	DateTime modified_time;	//更新日時
	DateTime notification_time;	//通知する時間
	DateTime todo_time;	// 予定のある日程
	DateTime TodoTime {
		get {return todo_time;}
		set {todo_time = value; }
	}
	
	string _title;
	public string Title{
		get {return _title;}
		set {_title = value;}
	}

	


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
		}
		return "";
	}
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

	void LoadByDay(DateTime _time){

	}
	void LoadById(int id){

	}



	public static List<TodoData> LoadAll(){
		List<TodoData> _todos = new List<TodoData>();
		string str = Util.LoadData(_get_data_key(DataKeys.MaxId).ToString());
		if(str == "") return _todos; //データなかった
		int head_id = int.Parse(str);
		
		Debug.Log(head_id);
		if(head_id>0){
			for(int id= 1; id<=head_id; id++){
				string todo_time = Util.LoadData(_get_data_key(DataKeys.TodoTime,id));
				string title = Util.LoadData(_get_data_key(DataKeys.Title,id));
				TodoData todo = new TodoData();
				todo.Id = id;
				Debug.Log(todo_time);
				if(todo_time != "")todo.TodoTime = DateTime.Parse(todo_time);
				todo.Title = title;
				_todos.Add(todo);
			}
		}
		return _todos;
	}
}
