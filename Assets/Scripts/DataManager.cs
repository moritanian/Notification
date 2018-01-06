using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// json dumpデータとアプリデータとの変換処理
public class DataManager{

	static string filename = "notification_dump.json";

	static string GetJson(){
		return JsonUtility.ToJson(new Serialization<TodoData>(TodoData.LoadAll())); 
	}

	public static void DumpJson(){

		string path = FilePlugin.GetDownloadPath (filename);
		FilePlugin.WriteFile (path, GetJson());
		Debug.Log (path);

	}

	public static void LoadJsonFile(){
		string path = FilePlugin.GetDownloadPath (filename);
		string jsonStr = FilePlugin.ReadFileAsText (path);
		LoadJson (jsonStr);
	}

	static void LoadJson(string jsonStr){
		List<TodoData> Todos = JsonUtility.FromJson<Serialization<TodoData>>(jsonStr).ToList();
		Debug.Log (jsonStr);
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
		Main.Instance.Reload();
	}	
}
