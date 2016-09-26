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

	public bool SendUserData(){
		
		string jsonStr = JsonUtility.ToJson(new Serialization<TodoData>(TodoData.LoadAll())); 
		//string jsonStr = LitJson.JsonMapper.ToJson(TodoData.LoadAll());

		Debug.Log(jsonStr);
		StartCoroutine(Post(jsonStr));
		return true;
	}

	public bool GetUserData(){
		// Json文字列 -> List<T>
		string str = "";
		List<TodoData> Todos = JsonUtility.FromJson<Serialization<TodoData>>(str).ToList();
		return true;
	}

	public void ApplyUserData(){

	}

	IEnumerator Post(string jsonData){
		Debug.Log("Post");
		WWWForm form = new WWWForm();
		form.AddField("data", jsonData);
 
        using(UnityWebRequest www = UnityWebRequest.Post("http://moritanian.s2.xrea.com/UnityTest.php", form)) {
            yield return www.Send();
 
            if (www.isError) {
                Debug.Log(www.error);
            } else {
                Debug.Log("Form upload complete!");
            }
        }
	}


}
