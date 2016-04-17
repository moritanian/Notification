using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TodoText : MonoBehaviour {

	static TextObj _title;
	public InputField _inputField;
	public static TodoText _todoText;
	public InputField _inputTitle;
	public TodoField _todoField;
	public TodoData todoData{
		set {_todoField._todoData = value;}
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

	public void SetTextColor(Color color){
		MyCanvas.Find<Text>("TodoTextField").color = color;
	}
	public void SetTextFontSize(int size){
		MyCanvas.Find<Text>("TodoTextField").fontSize= size;
	}

	// テキスト本文編集画面遷移時の処理
	public static void SetUp(TodoField Tf){
		// 文字カラー設定
		_todoText.SetTextColor(Setting.FontColor);
		
		// 文字サイズ設定
		_todoText.SetTextFontSize(Setting.FontSize);
		// Tododata をセット
		_todoText._todoField = Tf;
		// タイトル表示
		TitleSet(Tf._todoData.Title);
		// ファイルから本文のテキストを読み込んで表示
		_todoText.SetText(_todoText._load());
	}

	public void OnClickGoBack(){
		GoBack();
	}
	
	public void OnClickSave(){
		_save();
	}

	// idに対応するファイル削除
	public static void DeleteFile(int id){
		FileIo.Delete(_todoText._get_filename(id));
	} 

	void _save(){
		FileIo.Write(_get_filename(_todoField._todoData.Id),_get_text());
	}

	string _load(){
		return FileIo.Load(_get_filename(_todoField._todoData.Id));
	}

	string _get_filename(int id){
		return "TodoText_id"+id.ToString() + ".txt"; 
	}

	string _get_text(){
		return _inputField.text;
	}

	string _get_title_text(){
		return _inputTitle.text;
	}

	void SetText(string text){
		_inputField.text = text;
	}

	void GoBack(){
		Body.GoBoardMain();
	} 

	// タイトル編集完了
	public void TitleModified(){
		string title_str = _get_title_text();
		_todoField.SetText(title_str);
		_todoField._todoData.UpdateTitle(title_str);
	}
	public static void TitleSet(string txt){
		MyCanvas.Find<TodoText>("BoardText")._inputTitle.text = txt;
	}
}
