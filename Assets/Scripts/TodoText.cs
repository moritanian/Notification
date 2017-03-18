using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TodoText : MonoBehaviour {

	static TextObj _title;
	public InputField _inputField;
	public static TodoText _todoText;
	public InputField _inputTitle;
	public TodoField _todoField;
	public Text _changed_sign;
	public TodoData todoData{
		set {_todoField._todoData = value;}
	}
	string origi_text;

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
		// 参照日時更新
		Tf._todoData.UpdateLookupTime();
	}

	public void OnClickGoBack(){
		GoBack(false);
	}
	
	public void OnClickSave(){
		_save();
	}

	// Fieldを編集した
	public void TextEdited(){
		if(IsChanged()){
			_changed_sign.enabled = true;
		} else {
			_changed_sign.enabled = false;
		}
	}

	// idに対応するファイル削除
	public static void DeleteFile(int id){
		string file_name = _get_filename(id);
		FileIo.Delete(file_name);
	} 

	public static string GetTextFromId(int id){
		return FileIo.Load(_get_filename(id));
	}

	public void SaveText(int id, string text){
		FileIo.Write(_get_filename(id),text);
	}

	void _save(){
		SaveText(_todoField._todoData.Id, _get_text());
		origi_text = _get_text();
		// 変更印オフ
		_changed_sign.enabled = false;
	}

	string _load(){
		return GetTextFromId(_todoField._todoData.Id);
	}

	static string _get_filename(int id){
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
		//編集したか比較するために持っておく
		origi_text = text;
		// 編集印をオフに
		_changed_sign.enabled = false;
	}

	bool IsChanged(){
		//return (origi_text.CompareTo(_get_text()) == 0)? true : false;
		return (origi_text.CompareTo(_get_text()) == 0) ? false: true;
		
	} 

		// saveするかしないか指定
	public void GoBack(bool IsSave = true){
		Body.GoBoardMain();
		if(IsChanged()){
			if(IsSave){
				_save();
				PopUp.PopUpStart("保存しました", 1.5f);
			
			}else{
				PopUp.PopUpStart("保存せず戻ります", 1.5f);
			}
		}
		
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
