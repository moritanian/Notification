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

	static int crt_id = 0; // 現在編集中のid
	static string Title = "";
	public InputField _inputField;

	public Text text;
	public static TodoField Add(float x,float y,int id){
		TodoField obj = CreateInstanceEasy<TodoField>("TodoField",x,y);
		//MyCanvas.SetCanvasChild<TodoField>(obj);
		//obj.transform.SetParent(MyCanvas.GetCanvas().transform,false);
		obj.id = id;
		return obj;
	}


	int id; // todo のid 本文との紐づけにも
	// todo idのわりあて方法考える
	enum Status{
		Inactive,	//利用なし
		Active,	//利用している
		Protected,	//利用しているが、表示に制限あり
	};
	
	Status status = Status.Inactive;

	// Use this for initialization
	
	void Start () {
		//Create();
	}
	
	// 作成時の処理
	public void Create(int _id){
		string create_time = DateTime.Now.ToString();
		id = _id;
		status = Status.Active;
		Util.SaveData(TodoData._get_data_key(DataKeys.MaxId),id.ToString());
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
		Body.GoBoardText();
	}

	// タイトル編集完了ボタン
	public void Modified(){
		TodoData.TitleModify(id,GetText());
	}

	
}
