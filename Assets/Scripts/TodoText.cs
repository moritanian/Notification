using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class TodoText : MonoBehaviour {

	static TextObj _title;

	[SerializeField]
	NativeEditBox _inputField;
	public NativeEditBox InputField {
		get { return _inputField;}
	}

	public static TodoText Instance;

	[SerializeField]
	NativeEditBox _inputTitle;

	[SerializeField]
	TodoData _todoData;

	[SerializeField]
	Text _changed_sign;

	[SerializeField]
	Button undoButton;

	[SerializeField]
	Button redoButton;

	[SerializeField]
	Image undoButtonImage;

	[SerializeField]
	Image redoButtonImage;

	string origi_text;
	List<string> textHistory = new List<string> ();
	int currentHistoryIndex;
	bool setTextFlag;
	DateTime lastTextChangeTime;
	static bool isAutoSave = true;

	readonly Color EnableButtonColor = new Color (255.0f, 255.0f, 255.0f, 1.0f);
	readonly Color DisableButtonColor = new Color (255.0f, 255.0f, 255.0f, 0.2f);

	public static TodoText GetInstance(){
		return Instance;
	}

	void Awake(){
		Instance = this;
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

		// Tododata をセット
		_todoData = todoData;

		// ファイルから本文のテキストを読み込む
		string text = Instance._load ();

		// 文字カラー設定
		SetTextColor (Setting.FontColor);

		// 文字サイズ設定
		SetTextFontSize (Setting.FontSize);

		// タイトル表示
		TitleSet (todoData.Title);
		// 表示
		SetText (text);
		// 参照日時更新
		_todoData.UpdateLookupTime ();

		textHistory.Clear ();
		textHistory.Add (text);
		currentHistoryIndex = 0;
		UpdateButtons ();
	}

	public void OnClickGoBack(){
		GoBack(false);
	}
	
	public void OnClickSave(){
		_save();
	}

	public void OnTextValueChanged(){

		if (setTextFlag) {
			setTextFlag = false;
			return;
		}

		// En route
		if (textHistory.Count != currentHistoryIndex + 1) {
			textHistory.RemoveRange (
				currentHistoryIndex + 1,
				textHistory.Count - currentHistoryIndex - 1
			);
		}

		DateTime currentTime = DateTime.Now;
		if (currentTime - lastTextChangeTime > TimeSpan.FromMilliseconds (500)) {
			textHistory.Add (_get_text ());
			currentHistoryIndex++;
			lastTextChangeTime = currentTime;
		} else {
			// elapsed time since last modified is too small
			textHistory[currentHistoryIndex] = _get_text ();
		}

		UpdateButtons ();
	}

	public void OnClickUndo(){

		if (currentHistoryIndex < 1) {
			Debug.LogError ("OnClickUndo: index = 0");
			return;
		}

		currentHistoryIndex--;
		SetText (textHistory [currentHistoryIndex]);
		UpdateButtons ();
	}

	public void OnClickRedo(){
		
		if (currentHistoryIndex >= textHistory.Count - 1) {
			Debug.LogError ("OnClickRedo: index = " + currentHistoryIndex);
			return;
		}

		currentHistoryIndex++;
		SetText (textHistory [currentHistoryIndex]);
		UpdateButtons ();
	}

	// Fieldを編集した
	public void TextEdited(){
		if (isAutoSave) {
			if (IsChanged ()) 
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

		setTextFlag = true;

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
		if( IsChanged()){
			if (IsSave) {
				_save ();
				if (!isAutoSave) {
					PopUp.PopUpStart ("保存しました", 1.5f);
				}
			} else {
				if (!isAutoSave) {
					PopUp.PopUpStart ("保存せず戻ります", 1.5f);
				}
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

	void UpdateButtons(){

		if (this.currentHistoryIndex < this.textHistory.Count - 1) {
			redoButton.enabled = true;
			redoButtonImage.color = EnableButtonColor;
		} else {
			redoButton.enabled = false;
			redoButtonImage.color = DisableButtonColor;
		}

		if (this.currentHistoryIndex > 0) {
			undoButton.enabled = true;
			undoButtonImage.color = EnableButtonColor;
		} else {
			undoButton.enabled = false;
			undoButtonImage.color = DisableButtonColor;
		}
			
	}
}
