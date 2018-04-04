using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// json dumpデータとアプリデータとの変換処理
using System;


public class DataManager{

	static string filename = "notification_dump.json";

	static string GetJson(){
		return JsonUtility.ToJson(new Serialization<TodoData>(TodoData.LoadAll())); 
	}

	public static bool DumpJson(){

		string path = FilePlugin.GetDownloadPath (filename);
		string jsonStr = GetJson ();
		int len = jsonStr.Length;
		FilePlugin.WriteFile (path, jsonStr);
		Debug.Log (path);

		return ValidateJsonFile ();
	}

	static bool ValidateJsonFile(){
		string jsonStr = LoadJsonFile ();
		try {
			GetTodoListFromJson(jsonStr);
		} catch(Exception e){
			return false;
		}

		return true;
	}

	static string LoadJsonFile(){
		string path = FilePlugin.GetDownloadPath (filename);
		string jsonStr = FilePlugin.ReadFileAsText (path);
		return jsonStr;
	}

	public static bool LoadDataFromJsonFile(){
		return LoadJson (LoadJsonFile());
	}

	static bool LoadJson(string jsonStr){
		List<TodoData> Todos;
		try {
			Todos = GetTodoListFromJson(jsonStr);
		} catch(Exception e){
			return false;
		}
		Debug.Log (Todos.Count.ToString ());
		int id = TodoData.MaxId();
		foreach(TodoData todoData in Todos){
			id ++;
			todoData.Id = id;
			todoData.SaveCurrent();
			if (todoData.IsNotify) {
				todoData.setCall ();
			}
		}
		TodoData.SaveMaxId(id);
		return true;
	}

	static List<TodoData> GetTodoListFromJson(string jsonStr){
		return JsonUtility.FromJson<Serialization<TodoData>>(jsonStr).ToList();
	}
}
