using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

/*
 * TODO
 * todoList をメインで管理するのではなくここのstatic か管理クラス作成して保持したい
 */

// 保存するデータのキー
	public enum DataKeys{
		Title,	//タイトル文
		CreateTime,
		ModifiedTime,
		NotificationTime,
		TodoTime,
		LookupTime,
		MaxId,
		IsNotify,
		IsMemo
	}
//Todoの情報をもつ/ Todoの管理
	[Serializable]
public class TodoData : ISerializationCallbackReceiver{

	[SerializeField]
	int id;
	public int Id{
		get {return id;}
		set {id = value;}
	}

	// コンストラクタ
	public TodoData(int id, string _title = ""){
		Id = id;
		this._title = _title;
	}
	// 以下、時間
	DateTime create_time;	// 作成日時
	DateTime modified_time;	//更新日時
	DateTime notification_time;	//通知する時間
	DateTime todo_time;	// 予定のある日程
	DateTime lookup_time; // 参照した時間 // これでmemoはソートする
	public DateTime TodoTime {
		get {return todo_time;}
		set {todo_time = value; }
	}
	public DateTime LookupTime {
		get {return lookup_time;}
		set {lookup_time= value; }
	}

	// 以下シリアライズ用メンバ
	[SerializeField]
	string createTimeStr;
	[SerializeField]
	string todoTimeStr;
	[SerializeField]
	string lookupTimeStr;
	[SerializeField]
	string notificationTimeStr;
	[SerializeField]
	string todoText;
	[SerializeField]
	string titleStr;

	string _title;
	public string Title{
		get {return _title;}
	}

	public bool IsNotify = false;

	// メモ、todo時間が無効
	public bool IsMemo;

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
			case DataKeys.LookupTime:
				return "Todo_LookupTime_id"+id;
			case DataKeys.MaxId:
				return "Todo_MaxId";
			case DataKeys.IsNotify:
				return "Todo_IsNotify" + id;
			case DataKeys.IsMemo:
				return "Todo_IsMemo" + id;	
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
		_title = new_title;
		Util.SaveData(_get_data_key(DataKeys.Title,id),new_title);
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id),DateTime.Now.ToString());
		UpdateLookupTime();
	}

	public void UpdateTodoTime(DateTime Dt){
		todo_time = Dt;
		Util.SaveData(_get_data_key(DataKeys.TodoTime, id),Dt.ToString());
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id),DateTime.Now.ToString());
		UpdateLookupTime();
	}
	// 新規追加をunityで保存
	public void SaveCreation(){
		//int id = NewId();
		string create_time = DateTime.Now.ToString();
		Util.SaveData(_get_data_key(DataKeys.CreateTime,id), create_time);
		Util.SaveData(_get_data_key(DataKeys.IsMemo, id), IsMemo.ToString());
		if (MaxId () < id)
			SaveMaxId (id);
		UpdateLookupTime();
	}

	public void UpdateLookupTime(){
		UpdateLookupTime(DateTime.Now);
	}
	public void UpdateLookupTime(DateTime dt){
		Util.SaveData(_get_data_key(DataKeys.LookupTime,Id), dt.ToString());
		lookup_time = dt;
	}

	// Isnotify 更新
	// 更新結果を返す  更新できなければfalse
	public bool UpdateIsNotify(bool _IsNotify){
		if(_IsNotify){
			// memoに時間設定機能はない
			if(IsMemo)return false;
		} else if(IsNotify == _IsNotify){
			//更新不要
			return false;
		}
		IsNotify = _IsNotify;
		Util.SaveData(_get_data_key(DataKeys.IsNotify, id),IsNotify.ToString());
		return true;
	}

	public bool UpdateIsMemo(bool isMemo){
		this.IsMemo = isMemo;
		Util.SaveData(_get_data_key(DataKeys.IsMemo, id),isMemo.ToString());
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id),DateTime.Now.ToString());
		UpdateLookupTime();
	Debug.Log("Updateismemo" + isMemo.ToString());
		return true;
	}

	// 今の値でデータ保存
	public void SaveCurrent(){
		Util.SaveData(_get_data_key(DataKeys.Title,id), Title);
		Util.SaveData(_get_data_key(DataKeys.CreateTime, id), create_time.ToString());
		Util.SaveData(_get_data_key(DataKeys.NotificationTime, id), notification_time.ToString());
		Util.SaveData(_get_data_key(DataKeys.LookupTime, id), lookup_time.ToString());
		Util.SaveData(_get_data_key(DataKeys.ModifiedTime, id), modified_time.ToString());
		Util.SaveData(_get_data_key(DataKeys.TodoTime, id), TodoTime.ToString());
		Util.SaveData(_get_data_key(DataKeys.IsMemo, id), IsMemo.ToString());
		Util.SaveData(_get_data_key(DataKeys.IsNotify, id),IsNotify.ToString());
		TodoText.GetInstance().SaveText (Id, WWW.UnEscapeURL(todoText));
		//Util.SaveData(_get_data_key(DataKeys.ModifiedTime,id), create_time);
	}


	// 全てをload createDateないものは不正なものとして無視
	public static List<TodoData> LoadAll(){
		List<TodoData> _todos = new List<TodoData>();
		int head_id = MaxId();
		Debug.Log ("max id = " + head_id.ToString ());
		if(head_id>0){
			for(int id= 1; id<=head_id; id++){
				string create_time_str = Util.LoadData(_get_data_key(DataKeys.CreateTime, id));
				if(create_time_str == "")continue;
				string todo_time_str = Util.LoadData(_get_data_key(DataKeys.TodoTime,id));
				string lookup_time_str = Util.LoadData(_get_data_key(DataKeys.LookupTime, id));
				string title = Util.LoadData(_get_data_key(DataKeys.Title,id));
				string isNotifyStr = Util.LoadData(_get_data_key(DataKeys.IsNotify,id));
				string isMemostr = Util.LoadData(_get_data_key(DataKeys.IsMemo, id));
				TodoData todo = new TodoData(id, title);
				if(todo_time_str != "")todo.TodoTime = DateTime.Parse(todo_time_str);
				if(lookup_time_str != "")todo.LookupTime = DateTime.Parse(lookup_time_str);
				todo.IsNotify = (isNotifyStr == "True"); 
				todo.IsMemo = (isMemostr == "True");
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
		if(_list.Count == 0)return;
		if(_list[0].IsMemo){
			_list.Sort((a,b) => b.LookupTime.CompareTo(a.LookupTime));
		}else{
			_list.Sort((a,b) => a.TodoTime.CompareTo(b.TodoTime));
		}
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
		deletekeys.Add(DataKeys.LookupTime);
		foreach (DataKeys key in deletekeys){
			PlayerPrefs.DeleteKey(_get_data_key(key, id));
		}
		// 消去したデータが最後尾のデータだった
		if(id == MaxId() && id > 0){
			// #tdodo わたされたtododataの最大使っているがplayerpref のつかうべき
			int new_max = (id==1) ? 0 :todolist.Max(_todo => _todo.Id);
			SaveMaxId (new_max);
		}
		return true;
	}
	public static void LogAll(List<TodoData> todos){
		string str = "id   title   IsMemo\n";
		foreach (TodoData todo in todos){
			str += todo.Id + "   " + todo.Title + "  " + todo.IsMemo + "\n";
		}
		Debug.Log(str);
	}

	public static void SaveMaxId(int maxId){
		Util.SaveData(_get_data_key(DataKeys.MaxId) , maxId.ToString());	
	}

	   public void OnBeforeSerialize()
    {
		if (id == 0) {
			return;
		}
		Debug.Log ("onBeforeSerialize");
    	// サーバに渡すデータ処理
    	// エスケープ処理も行う
		createTimeStr = create_time.ToString();
        todoTimeStr = todo_time.ToString();
        notificationTimeStr = notification_time.ToString();
        lookupTimeStr = lookup_time.ToString();
        todoText = WWW.EscapeURL(TodoText.GetTextFromId(id));
        titleStr = WWW.EscapeURL(Title);
    }

      public void OnAfterDeserialize()
    {
		if (id == 0) {
			return;
		}
		create_time = DateTime.Parse (createTimeStr);
		todo_time = DateTime.Parse (todoTimeStr);
		notification_time = DateTime.Parse (notificationTimeStr);
		lookup_time = DateTime.Parse (todoTimeStr);
		_title = WWW.UnEscapeURL (titleStr);
		lookup_time = DateTime.Parse (lookupTimeStr);
    }

	// ローカル通知を登録
	public bool setCall(){
		if(TodoTime.CompareTo(DateTime.Now) < 0){
			return false;
		}
		LocalNotification.LocalCallSet(Id, TodoTime, "Notify"+ Id.ToString(), "title", Title);
		return true;
	}

	// ローカル通知を削除
	public bool deleteCall(){
		return LocalNotification.LocalCallReset(Id);
	}
}
