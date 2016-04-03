using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TodoText : MonoBehaviour {

	static TextObj _title;
	public InputField _inputField;
	public static TodoText _todoText;

	int id;
	public int Id{
		get {return id;}
		set {id = value;}
	} 
	// Use this for initialization
	void Start () {
		_title = MyCanvas.Find<TextObj>("Titletxt");
		_todoText = this;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetUp(){
		SetText(_load());
	}

	public void OnClickGoBack(){
		GoBack();
	}
	
	public void OnClickSave(){
		_save();
	}

	void _save(){
		FileIo.Write(_get_filename(id),_get_text());
	}

	string _load(){
		return FileIo.Load(_get_filename(id));
	}

	string _get_filename(int id){
		return "TodoText_id"+id.ToString() + ".txt"; 
	}

	string _get_text(){
		return _inputField.text;
	}
	void SetText(string text){
		_inputField.text = text;
	}

	void GoBack(){
		Body.GoBoardMain();
	} 
	public static void TitleSet(string txt){
		_title.Label = txt;
	}
}
