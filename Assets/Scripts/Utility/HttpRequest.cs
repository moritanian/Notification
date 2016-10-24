using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class HttpRequest : MonoBehaviour {

	static HttpRequest httpRequest;
	
	void Start(){
		httpRequest = this;
	}

	public static WWW GET(string url, Action<string> callBack){
		return httpRequest.Get(url, callBack);
	}
	public static WWW GET(string url){
		return httpRequest.Get(url, (string str) => {});
	}

	public static WWW POST(string url, Dictionary<string,string> post, Action<string> callBack){
		return httpRequest.Post(url, post, callBack);
	}

	public static WWW POST(string url, Dictionary<string,string> post){
		return httpRequest.Post(url, post, (string str) => {});
	}

    public static WWW JSON_POST(string url, string jsonData, Action<string> callBack){
        return httpRequest.Json_Post(url, jsonData, callBack);
    }

    public static WWW JSON_POST(string url, string jsonData){
        return httpRequest.Json_Post(url, jsonData, (string str)=>{});
    }

	public WWW Get(string url, Action<string> callBack) {
        WWW www = new WWW (url);
        StartCoroutine (WaitForRequest (www, callBack));
        return www; 
    }
     
    public WWW Post(string url, Dictionary<string,string> post, Action<string> callBack) {
        WWWForm form = new WWWForm();
        foreach(KeyValuePair<string,string> post_arg in post) {
           form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);
     
        StartCoroutine(WaitForRequest(www, callBack));
        return www; 
    }

    public WWW Json_Post(string url, string jsonData, Action<string> callBack){
          // HEADERはHashtableで記述
        Dictionary<string, string> header = new Dictionary<string, string>();
        // jsonでリクエストを送るのへッダ例
        header.Add ("Content-Type", "application/json; charset=UTF-8");

        byte[] postBytes = Encoding.Default.GetBytes (jsonData);

        // 送信開始
        WWW www = new WWW (url, postBytes, header);
        
        StartCoroutine(WaitForRequest(www, callBack));
        
        return www; 
    }
     
    private IEnumerator WaitForRequest(WWW www, Action<string> callBack) {
        yield return www;
     
        // check for errors
        if (www.error == null) {
            Debug.Log("WWW Ok!: " + www.text);
            callBack(www.text);
        } else {
            Debug.Log("WWW Error: "+ www.error);
        }
    }
}
