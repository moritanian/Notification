using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TodoText : MonoBehaviour {

	static TextObj _title;
	public InputField _inputField;
	public static TodoText _todoText;
	public InputField _inputTitle;
	int id;
	public int Id{
		get {return id;}
		set {id = value;}
	} 

	void Awake(){
		//_title = MyCanvas.Find<>("Titletxt");
		_todoText = this;
	}
	// Use this for initialization
	void Start () {
		
	
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
		Debug.Log("TitleSet" + txt);
		MyCanvas.Find<TodoText>("BoardText")._inputTitle.text = txt;
	}
}
