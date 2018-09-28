﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using UnityEngine.SceneManagement;


public class Setting : Token {

	[SerializeField]
	Sprite mono_back;
	[SerializeField]
	Sprite default_back;

	Text outputText;
	Toggle _debugToggle;
	Toggle _normalToggle;
	public InputField _inputField;
	Text _fontColorChgText;

	static int fontsize = 11;
	public static int FontSize{
		get {return fontsize;}
		set {
			if(value>0)fontsize = value;
		}
	}

	static Color fontcolor = Color.white;
	int color_id = 0;
	public static Color FontColor{
		get{return fontcolor;}
	}
 	const int color_nums = 7;

   	Color[] font_colors = {
   		Color.white,
   		new Color(0.14f,0.03f,0.9f), //new Color(35,9,234,1.0f), // 青系
   		new Color(0.14f, 0.3f, 0.05f),// new Color(34,70,16, 1.0f), // 緑系
   		new Color(0.4f,0.1f,0.1f),  // 赤系
   		new Color(0.3f,0.1f,0.4f),  // 紫系
   		new Color(0.03f,0.4f, 0.4f), // 青緑
   		Color.black // 黒 
   	};

   	enum Theme{
   		color,
   		mono
   	} 
   	Theme theme;

   	Color[] style_mono_colors = {
   		Color.black,						// 基本色
   		Color.white,						// 文字色
   		new Color(0.3f,0.3f,0.3f),			// スクロール背景
   		new Color(0.1f, 0.1f, 0.1f, 0.5f),	// outofrange
   		new Color(0, 0, 0, 206.0f/255.0f), // textInput 背景
   		new Color(0, 0, 0, 44.0f/255.0f), // boardMain 背景
   		new Color(182.0f/255.0f, 175.0f/255.0f, 24.0f/255.0f), // 押したセルの色
   		new Color(7.0f/255.0f , 92.0f/255.0f ,71.0f/255.0f),
   	};
   	Color[] style_default_colors = {
   		Color.white,
   		Color.black,
   		new Color(63.0f/255.0f,113.0f/255.0f,35.0f/255.0f),
   		new Color(255,255,255, 0.5f),
   		Color.white,
   		Color.white,
   		new Color(1.0f,247.0f ,59.0f/255.0f),
   		new Color(156.0f/255.0f, 1.0f, 230.0f/255.0f),
   	};

	public static Setting instance; 

   	void Awake(){
		outputText = MyCanvas.Find<Text>("outputText");
		_debugToggle = MyCanvas.Find<Toggle>("IsDebugLog");
		_normalToggle = transform.Find("NormalToggle").gameObject.GetComponent<Toggle>();
		_fontColorChgText = transform.Find("TextColor").gameObject.GetComponent<Text>();
		instance = this;

		string size_str = Util.LoadData(GetDataKey(DataKeys.FontSize));
		if(size_str != "")fontsize = int.Parse(size_str);
		string color_str = Util.LoadData(GetDataKey(DataKeys.Color));
		if(color_str != "")ChangeColor(int.Parse(color_str));
		OnChangeDebugToggle(true);
		OnChangeNormalLog(true);
		LoadTheme();
	}

	// Use this for initialization
	void Start(){
	}


	// Update is called once per frame
	void Update () {

	}

	void LoadTheme(){
		string theme_str = Util.LoadData(GetDataKey(DataKeys.Theme));
		if(theme_str != ""){
			theme = (Theme)int.Parse(theme_str);
		}
		ApplyTheme();
	}

	// 設定保存
	public enum DataKeys{
		BGid,
		FontSize,
		Color,
		IsDebugLog,
		IsNormalLog,
		Theme,
	}

	public static string GetDataKey(DataKeys key){
		return "Todo_" + key.ToString("G");
	}

	public void OnClickGoMain(){
		outputText.text = "";
		Body.GoBoardMain();
	}
	public void OnClickDataDelete(){
		ShowDialog ("本当に消去してよろしいですか？", deleteData);
	}

	void  deleteData(){
		int max_id = TodoData.MaxId();
		// ファイル消去
		for(int i=1; i<=max_id; i++){
			TodoText.DeleteFile(i);
		}
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		outputText.text = "全てのデータを削除しました!";
		Main.Instance.Reload();
	}

	// Todo 一覧を再読み込みして表示
	public void OnClickReload(){
		Main.Instance.Reload();
		outputText.text = "リロードしました。";
	}

	public void OnClickBGChange(){
		outputText.text = "";
		// 背景画像変更
		int id = MyCanvas.Find<BackGound>("BackGround").BGChange();
		Util.SaveData(GetDataKey(DataKeys.BGid), id.ToString());
		GetComponent<Image>().color = new Color(255, 255,255 , 0.5f); 
		StartCoroutine(ImageR());
	}

	// 画面デバッグログ出力On/Off
	// 表示するかどうかboolを保存する
	public void OnChangeDebugToggle(bool is_init = false){
		if(is_init){
			string bool_str = Util.LoadData(GetDataKey(DataKeys.IsDebugLog));
			if(bool_str != ""){
				bool isLog = bool.Parse(bool_str);
				DebugLog.IsLogDebug = isLog;
				_debugToggle.isOn = isLog;
				return ;
			}
		}
		DebugLog.IsLogDebug = _debugToggle.isOn;
		Util.SaveData(GetDataKey(DataKeys.IsDebugLog), _debugToggle.isOn.ToString());
		
	}

	public void OnChangeNormalLog(bool is_init = false){
		if(is_init){
			string bool_str = Util.LoadData(GetDataKey(DataKeys.IsNormalLog));
			if(bool_str != ""){
				bool isLog = bool.Parse(bool_str);
				DebugLog.NormalLog = isLog;
				_normalToggle.isOn = isLog;
				return ;
			}
		} 
		DebugLog.NormalLog = _normalToggle.isOn;
		Util.SaveData(GetDataKey(DataKeys.IsNormalLog), _normalToggle.isOn.ToString());
		
	}

	// fontsize フィールド変更
	public void OnChangeFontSize(){
		int font_size = int.Parse(_inputField.text); 
		if(font_size > 0)fontsize = font_size;
		Util.SaveData(GetDataKey(DataKeys.FontSize), font_size.ToString());
	}

	IEnumerator ImageR()
 	{
        yield return new WaitForSeconds(3.0f);
        GetComponent<Image>().color = new Color(255, 255,255 , 1.0f);
    }

   
    // 文字色変更
    public void OnClickFontColorChange(){
    	ChangeColor(color_id + 1);
    }

    // テーマ変更
    public void OnClickThemeChange(){
    	if(theme == Theme.color){
    		theme = Theme.mono;
    	}else{
    		theme = Theme.color;
    	}
    	Util.SaveData(GetDataKey(DataKeys.Theme), ((int)theme).ToString());

		SceneManager.LoadScene("Main");
    }

    public void ApplyTheme(){
    	Color[] colors;
    	if(theme == Theme.mono){
    		colors = style_mono_colors;
    		MyCanvas.Find<SpriteRenderer>("BackGround").sprite = mono_back;
    	}else{
    		colors = style_default_colors;
    		MyCanvas.Find<SpriteRenderer>("BackGround").sprite = default_back;
    	}
    	fontcolor = colors[1];
    	_fontColorChgText.color = colors[1];
    	GetComponent<Image>().color = colors[4];
    	MyCanvas.Find<Image>("ScrollView").color = colors[2];
    	MyCanvas.Find<Image>("BoardMain").color = colors[5];
		MyCanvas.Find<Image>("TextInput").color = colors[4];
		MyCanvas.Find<Text>("TodoTextField").color = colors[1];
		MyCanvas.Find<Text>("DispDateTimeText").color = colors[1];
    	MyCanvas.Find<Image>("SearchField").color = colors[3];
    	MyCanvas.Find<Text>("SearchText").color = colors[1];
    	MyCalendar myCalendar = MyCanvas.Find<MyCalendar>("MyCalendar");
    	myCalendar.NormalDayColor = colors[0];
    	myCalendar.OutofRangeColor = colors[3];
    	myCalendar.FontColor = colors[1];
    	myCalendar.MyDayColor = colors[6];
    	myCalendar.TodayColor = colors[7];
    	myCalendar.FindChild<Text>("EditDateTime").color = colors[1];

		// set TodoField prefab
		switch (theme) {
		case Theme.color:
			TodoField.parent = new TokenMgr<TodoField> ("TodoField", 40);
			break;
		case Theme.mono:
			TodoField.parent = new TokenMgr<TodoField> ("TodoFieldMono", 40);
			break;
		}
    }

    public void ChangeColor(int id){
    	if(id < 0 || color_nums <= id)id = 0;
    	color_id = id;
    	fontcolor = font_colors[id];
    	_fontColorChgText.color = fontcolor;

    	Util.SaveData(GetDataKey(DataKeys.Color),color_id.ToString());
    	
    }
		
	// alarm debug
	public void OnClickAlarmSet(){
		int id = 0;
		LocalNotification.AlarmSet (id, DateTime.Now.AddSeconds(5));
	}

	// notification debug
	public void OnClickNotificationSet(){
		int id = 2;
		MyLocalNotification.LocalCallSet (id, DateTime.Now.AddSeconds(5), "name", "title", "label");
		Main.Instance.ShowDayById (2);
	}

    public void OnClickSendData(){
    	TranslateData.test();
    }

	public void OnClickDumpData(){
		ShowDialog ("Dump user data.", ExecDumpData);
	}

	public void OnClickLoadData(){
		ShowDialog ("Load user data from json file", ExecLoadData);
	}

	public void ExecDumpData(){
		if (DataManager.DumpJson ()) {
			outputText.text = "Successfully dumped!";
		} else {
			outputText.text = "Validation failed";
		}
	}

	public void ExecLoadData(){
		if (DataManager.LoadDataFromJsonFile ()) {
			Main.Instance.Reload ();
			outputText.text = "Successfully loaded!";
		} else {
			outputText.text = "Failed to load json file.";
		}
	}

	void ShowDialog(string text, Action okAction){
		Dialog dialog = MyCanvas.FindChild<Dialog>("DialogBack");
		dialog.Revive();
		dialog.Text = text;
		dialog._yesCallBack = new DialogAction(okAction);
	}

	public void OnClickCreateDummyData(){
		InputField dummyNumInput = MyCanvas.Find<InputField>("DummyNumInput");
		int dummyNum;
		try {
			dummyNum = Int32.Parse(dummyNumInput.text);

		} catch(FormatException e){
			Debug.Log ("dummyNumInput: \"" + dummyNumInput.text + "\" is invalid");
			return;
		}

		ShowDialog (string.Format ("Create {0} dummy data", dummyNum), () => {
			CreateDummyData (dummyNum);
		});
	}

	public void CreateDummyData(int dummyNum){
		
			
		for (int i = 0; i < dummyNum; i++) {
			int id = i + 1;
			TodoData new_todo = new TodoData(id);
			new_todo.IsMemo = i % 4 == 0;
			new_todo.SaveCreation();
			// title
			new_todo.UpdateTitle ("title" + id);
			// date
			new_todo.UpdateTodoTime (DateTime.Now.AddDays (UnityEngine.Random.Range (-100, 100)));
			//
			new_todo.UpdateIsNotify(i%6 == 0);
			// content
			TodoText.GetInstance().SaveText(id, "content" + id);
		}
		Main.Instance.Reload();
		Debug.Log("append " + dummyNum + " dummy data");
	}
}
