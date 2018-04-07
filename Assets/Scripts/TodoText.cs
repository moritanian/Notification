using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TodoText : MonoBehaviour {

	static TextObj _title;
	public NativeEditBox _inputField;
	public static TodoText _todoText;
	public NativeEditBox _inputTitle;
	public TodoData _todoData;
	public Text _changed_sign;
	string origi_text;

	static bool isAutoSave = true;

	public static TodoText GetInstance(){
		return _todoText;
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

	public void SetUp(TodoData todoData){
		// 文字カラー設定
		SetTextColor (Setting.FontColor);

		// 文字サイズ設定
		SetTextFontSize (Setting.FontSize);
		// Tododata をセット
		_todoData = todoData;
		// タイトル表示
		TitleSet (todoData.Title);
		// ファイルから本文のテキストを読み込んで表示
		SetText (_todoText._load ());
		// 参照日時更新
		_todoData.UpdateLookupTime ();
	}

	public void OnClickGoBack(){
		GoBack(false);
	}
	
	public void OnClickSave(){
		_save();
	}

	// Fieldを編集した
	public void TextEdited(){
		if (isAutoSave) {
			_save();
		} else {
			if (IsChanged ()) {
				_changed_sign.enabled = true;
			} else {
				_changed_sign.enabled = false;
			}
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
		FileIo.Write(_get_filename(id), text);
	}

	void _save(){
		SaveText (_todoData.Id, _get_text());
		origi_text = _get_text();
		// 変更印オフ
		_changed_sign.enabled = false;
	}

	string _load(){
		return GetTextFromId(_todoData.Id);
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
		// set scroll 0 for NativeEditBox
		_inputField.SetVerticalScrollOffset(0);

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
		if(!isAutoSave && IsChanged()){
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
		TodoField.parent.ForEachExists (todoField => {
			if(todoField._todoData.Id == _todoData.Id){
				todoField.SetText(title_str);
			}
		});

		_todoData.UpdateTitle(title_str);
		if (_todoData.IsNotify)
			_todoData.setCall ();
	}
	public void TitleSet(string txt){
		MyCanvas.Find<TodoText>("BoardText")._inputTitle.text = txt;
	}
}
