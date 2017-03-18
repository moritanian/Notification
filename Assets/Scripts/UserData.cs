using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class UserData{

	public static UserData myData;
	static bool isActivated;
	public static bool IsActivated{
		get {return isActivated;}
	}

	[SerializeField]
	int _id;
	public int id{
		get {return _id;} 
	} 
	public string name {
		get {return _name;}
	}

	public string password {
		get {return _password;}
	}
	public string password_confirmation {
		get {return _password_confirmation;}
	}

	[SerializeField]
	string _name = "";
	[SerializeField]
	string _password = "";
	[SerializeField]
	string _password_confirmation = "";

	// validation data
	int password_len_min = 6;
	int name_len_min = 3;

	public static void SetMyData(UserData userData){
		myData = new UserData(userData.name, userData.password, userData.password_confirmation);
		myData.setId(userData.id);
		isActivated = true;
		SaveData();

		// ユーザ作成/ 変更ボタン　の切り替え
		// fix me このクラスはデータを扱うクラスなので出来れば表示部分の関数を呼びたくない
		MyCanvas.Find<Setting>("BoardSetting").ApplyUserAccountButton();
	}

	public static void SaveData(){
		Util.SaveData("IsCreateUserData", "1");
		Util.SaveData("UserName", myData.name);
		Util.SaveData("UserPass", myData.password);
		Util.SaveData("UserId", myData.id.ToString());
	}

	public static bool LoadData(){

		try {
            int isCreate = int.Parse(Util.LoadData("IsCreateUserData"));
            if(isCreate != 1)return false;
        }
        catch (FormatException) {
           return false;
         }   
        catch (OverflowException) {
            return false;   
        }  
		
		string name = Util.LoadData("UserName");
		string password = Util.LoadData("UserPass");
		int id = int.Parse(Util.LoadData("UserId"));
		myData = new UserData(name, password, password);
		myData.setId(id);
		isActivated = true;
		return true;

	}

	public UserData(string _name, string _password, string _password_confirmation){
		this._name = _name;
		this._password = _password;
		this._password_confirmation = _password_confirmation;
	}

	public void setId(int _id){
		this._id = _id;
	}

	// データが有効か？  return result, 
	public bool IsValid(List<string> errors){
		bool result = true;

		if(password.Length < password_len_min){
			errors.Add("Password is too short. Password must be more than " + password_len_min.ToString() + " letters" );
		}
		if(password != password_confirmation){
			errors.Add("Password and password confirmation are not identical");
		}
		if(name.Length < name_len_min){
			errors.Add("Your name is too short. Your name must be more than " + name_len_min.ToString() + " letters");
		}
		if(errors.Count > 0) result = false;

		return result;
	}
}
