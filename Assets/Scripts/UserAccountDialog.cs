using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UserAccountDialog : Token {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Revive(){
		FindDescendant<InputField>("password").text = "";
		FindDescendant<InputField>("password_confirm").text = "";
		FindDescendant<Text>("ErrorText").text = "";
		base.Revive();
	}

	public void OnclickSendCreateAccount(){
		string name = FindDescendant<InputField>("name").text;
		string password = FindDescendant<InputField>("password").text;
		string password_confirm = FindDescendant<InputField>("password_confirm").text;
		UserData userData =  new UserData(name, password, password_confirm);
		List<string> errors = new List<string>();
		if(userData.IsValid(errors)){
			Action<int> callBack = delegate(int id){
				if(id !=0){
					FindDescendant<Text>("ErrorText").text = "Successfully send your data!.";
					userData.setId(id);
					UserData.SetMyData(userData);			
				}else{
					FindDescendant<Text>("ErrorText").text = "some errors occured. Please check your send data.";
				}
			};
			TranslateData.createUser(userData, callBack);

		}else{
			string errortext = "";
			foreach(string error in errors){
				errortext += error + "\n";
			}
			FindDescendant<Text>("ErrorText").text = errortext;
		}
	}

	public void OnClickCancel(){
		Vanish();
	}
	
}
