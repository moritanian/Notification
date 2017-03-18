using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Experimental.Networking;
using LitJson;  //Litjson読み込み


// ユーザデータをサーバと通信する
public class TranslateData : MonoBehaviour{
	static TranslateData Ts;
	
	void Start(){
		Ts = this;
	}

	public static void test(){
		Ts.SendUserData();
	}

	public static void recieve(){
		Ts.GetUserData();
	}

	public static void createUser(UserData userData, Action<int> callBack){
		Ts.CreateUser(userData, callBack);
	}

	public bool SendUserData(){
		/* this is for debug 
			本番時は削除すること！！
		*/
		
			UserData userData =  new UserData("moritanian", "123456", "123456");
			userData.setId(22);
			UserData.SetMyData(userData);
			
		
		
		string jsonStr = JsonUtility.ToJson(new Serialization<TodoData>(TodoData.LoadAll())); 
		//string jsonStr = LitJson.JsonMapper.ToJson(TodoData.LoadAll());
		jsonStr = jsonStr.Substring(0, jsonStr.Length - 1); // delete last 1 letter
		
		//string user_json_Str = LitJson.JsonMapper.ToJson(UserData.myData);
		//string user_json_str = LitJson.JsonMapper.ToJson(userData);
		string user_json_str = "{\"id\":\"" + UserData.myData.id.ToString() + "\",\"name\":\"" 
						+ UserData.myData.name + "\",\"password\":\"" + UserData.myData.password + "\"}";
		jsonStr += ",\"user\":" + user_json_str + "}";
		Debug.Log(jsonStr);
		if(UserData.IsActivated){
			int id = UserData.myData.id;
			string url = "https://notification-moritanian.c9users.io/api/v1/users/" + id.ToString() + "/todos/create";
			HttpRequest.JSON_POST(url, jsonStr);
		}else{
			return false;
		}
		return true;
	}

	public bool GetUserData(){
		// Json文字列 -> List<T>
		if(UserData.IsActivated){
			int id = UserData.myData.id;
			string url = "https://notification-moritanian.c9users.io/api/v1/users/" + id.ToString() + "/todos";

			Action<string> callBack = delegate(string ret){
				//List<TodoData> Todos = JsonUtility.FromJson<Serialization< TodoData>>(ret).ToList();			
				List<TodoData> Todos = JsonMapper.ToObject<List<TodoData>>(ret);
				TodoText boardText = MyCanvas.Find<TodoText>("BoardText");
				foreach(TodoData todoData in Todos){
					todoData.OnAfterDeserialize();
					boardText.SaveText(todoData.Id, WWW.UnEscapeURL(todoData.todoText));

				}
				//List<TodoData> Todos = JsonUtility.FromJson<List<TodoData>>(ret);

				Main mainBoard = MyCanvas.Find<Main>("BoardMain");
	  			mainBoard.insertTodos(Todos);
	  		//TodoData.LogAll(Todos);
	  		Debug.Log("todos insert " + Todos.Count.ToString());
	  			mainBoard.Restart();
			}; 

			//JsonUtility.FromJson<Serialization<TodoData>>(ret).ToList();	

			//WWW www = HttpRequest.GET(url, callBack );

			string user_json_str = "{\"id\":\"" + UserData.myData.id.ToString() + "\",\"name\":\"" 
						+ UserData.myData.name + "\",\"password\":\"" + UserData.myData.password + "\"}";
			string jsonStr = "{\"user\":" + user_json_str + "}";
			Debug.Log("Json = " + jsonStr);
			WWW www = HttpRequest.JSON_POST(url, jsonStr, callBack);
		}else{
			return false;
		}
		return true;
	}

	// サーバにユーザデータ登録
	public bool CreateUser(UserData userData, Action<int> callBack){
		string url = "https://notification-moritanian.c9users.io/api/v1/users/create";
		//string jsonStr = JsonUtility.ToJson(userData);
		string jsonStr = LitJson.JsonMapper.ToJson(userData);
		Debug.Log(jsonStr);
		Action<string> postCallBack = delegate(string json){
			Dictionary<string, int> result = JsonMapper.ToObject<Dictionary<string, int>>(json);
			Debug.Log(result);
			int id = result["id"];
			callBack(id);
		};
		//HttpRequest.POST(url, new Dictionary<string, string>(){{"data", jsonStr}}, postCallBack);
		HttpRequest.JSON_POST(url, jsonStr, postCallBack);
		return true;
	}
}
