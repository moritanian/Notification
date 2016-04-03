using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
/*
時間記録には System.DateTime を使用
datetime.ToSing("yyyy/MM/dd HH:mm:ss") // 指定した形式で取得
datetime = DateTime.Now; //現在時刻
datetime = new DateTime("2015", "12","31");
datetime = new DateTime("2016","2","21","12","9","22");

datetime = DateTime.Parse()


*/

// タイトル
public class TodoField : Token {

	// 管理オブジェクト
	public static TokenMgr<TodoField> parent = null;
	static string Title = "";
	public InputField _inputField;

	public Text text;
	public static TodoField Add(float x,float y,int id){
		//TodoField obj = CreateInstanceEasy<TodoField>("TodoField",x,y);
		TodoField obj = parent.Add(0,0);
		if(obj == null){
			Debug.Log("null TodoField作成できず");
			return null;
		}
		obj.id = id;
		return obj;
	}


	public int id; // todo のid 本文との紐づけにも
	// todo idのわりあて方法考える
	enum Status{
		Inactive,	//利用なし
		Active,	//利用している
		Protected,	//利用しているが、表示に制限あり
	};
	
	Status status = Status.Inactive;

	// Use this for initialization
	
	void Update(){
		/*
		if(Main.Vanish_Flg){
			Vanish();
		}
		*/
	}
	// 作成時の処理
	public void Create(int _id){
		string create_time = DateTime.Now.ToString();
		id = _id;
		status = Status.Active;
		Util.SaveData(TodoData._get_data_key(DataKeys.MaxId),id.ToString());
		Util.SaveData(TodoData._get_data_key(DataKeys.TodoTime),create_time);
		Util.DoneSave();
		//再利用している場合、値が入っていることがあるため消去
		SetText("");
	}
	

	public string GetText(){
		return _inputField.text;
	}
	public void SetText(string text){
		_inputField.text = text;
	}

	public void OnclickEdit(){
		_edit();
	}

	void _edit(){
		Title = GetText();
		TodoText.TitleSet(Title);
		TodoText._todoText.Id = id;
		TodoText._todoText.SetUp();
		Body.GoBoardText();

	}

	// タイトル編集完了ボタン
	public void Modified(){
		TodoData.TitleModify(id,GetText());
	}

	
}
