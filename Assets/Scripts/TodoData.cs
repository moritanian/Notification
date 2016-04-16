using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

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
		_title = "";
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
		//int id = NewId();
		string create_time = DateTime.Now.ToString();
		Util.SaveData(_get_data_key(DataKeys.CreateTime,id), create_time);
		if(MaxId()<id)Util.SaveData(_get_data_key(DataKeys.MaxId),id.ToString());
	}
	// Isnotify 更新
	public void UpdateIsNotify(bool _IsNotify){
		IsNotify = _IsNotify;
		Util.SaveData(_get_data_key(DataKeys.IsNotify, id),IsNotify.ToString());
	}


	// 全てをload createDateないものは不正なものとして無視
	public static List<TodoData> LoadAll(){
		List<TodoData> _todos = new List<TodoData>();
		int head_id = MaxId();
		if(head_id>0){
			for(int id= 1; id<=head_id; id++){
				string create_time = Util.LoadData(_get_data_key(DataKeys.CreateTime, id));
				if(create_time == "")continue;
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

	// 新規Id使えるのを調べる
	// ぬけている箇所があればそれを使う。なければ最大値+1
	public static int NewId(List<TodoData> list){
		int max_id = MaxId();
		for(int i = 1; i <= max_id; i++){
			if(!list.Exists(todo => todo.Id == i)){
				return i;
			}
		}
		return max_id+1;
	}

	public static int MaxId(){
		string str = Util.LoadData(_get_data_key(DataKeys.MaxId).ToString());
		if(str == ""){
			return 0; //データなかった
		} 
		return int.Parse(str);
	}

	public static void SortByDate(List<TodoData> _list){
		_list.Sort((a,b) => a.TodoTime.CompareTo(b.TodoTime));
	}

	// List からid指定されたデータ削除、playerprefのも削除する
	public static bool DeleteDataById(List<TodoData> todolist, int id){
		TodoData _tododata = todolist.Find(todo => todo.Id == id);
		if(_tododata == null) return false;
		// データ削除
		if(!todolist.Remove(_tododata) ){
			return false;
		}
		// player pref 削除
		List<DataKeys> deletekeys = new List<DataKeys>();
		deletekeys.Add(DataKeys.TodoTime);
		deletekeys.Add(DataKeys.CreateTime);
		deletekeys.Add(DataKeys.ModifiedTime);
		deletekeys.Add(DataKeys.Title); 
		foreach (DataKeys key in deletekeys){
			PlayerPrefs.DeleteKey(_get_data_key(key, id));
		}
		// 消去したデータが最後尾のデータだった
		if(id == MaxId() && id > 0){
			// #tdodo わたされたtododataの最大使っているがplayerpref のつかうべき
			int new_max = (id==1) ? 0 :todolist.Max(_todo => _todo.Id);
			Util.SaveData(_get_data_key(DataKeys.MaxId) , new_max.ToString());
		}
		return true;
	}
	public static void LogAll(List<TodoData> todos){
		string str = "id   title\n";
		foreach (TodoData todo in todos){
			str += todo.Id + "   " + todo.Title + "\n";
		}
		Debug.Log(str);
	}
}
