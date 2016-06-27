using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Setting : Token {


	Text outputText;
	Toggle _debugToggle;
	Toggle _normalToggle;
	public InputField _inputField;
	public AndroidSamp androidObj;
	Text _fontColorChgText;
	Dropdown _dp_notify_id;

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


	// Use this for initialization
	void Start () {
		string size_str = Util.LoadData(GetDataKey(DataKeys.FontSize));
		if(size_str != "")fontsize = int.Parse(size_str);
		string color_str = Util.LoadData(GetDataKey(DataKeys.Color));
		if(color_str != "")ChangeColor(int.Parse(color_str));
		OnChangeDebugToggle(true);
		OnChangeNormalLog(true);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void Awake(){
		outputText = MyCanvas.Find<Text>("outputText");
		_debugToggle = MyCanvas.Find<Toggle>("IsDebugLog");
		_normalToggle = transform.FindChild("NormalToggle").gameObject.GetComponent<Toggle>();
		_fontColorChgText = transform.FindChild("TextColor").gameObject.GetComponent<Text>();
		_dp_notify_id = MyCanvas.Find<Dropdown>("DropdownId");
	}

	// 設定保存
	public enum DataKeys{
		BGid,
		FontSize,
		Color,
		IsDebugLog,
		IsNormalLog
	}

	public static string GetDataKey(DataKeys key){
		return "Todo_" + key.ToString("G");
	}

	public void OnClickGoMain(){
		outputText.text = "";
		Body.GoBoardMain();
	}
	public void OnClickDataDelete(){
		Debug.Log("OnClickDataDelete");
		Dialog dialog = MyCanvas.FindChild<Dialog>("DialogBack");
		dialog.Revive();
		dialog.Text = "本当に消去してよろしいですか？";
		dialog._yesCallBack = new YesCallBack(deleteData);
		
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
		Main _main = MyCanvas.Find<Main>("BoardMain");
		_main.Reload();
	}

	// Todo 一覧を再読み込みして表示
	public void OnClickReload(){
		Main _main = MyCanvas.Find<Main>("BoardMain");
		_main.Reload();
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
				Debug.Log(bool_str);
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

    public void ChangeColor(int id){
    	if(id < 0 || color_nums <= id)id = 0;
    	color_id = id;
    	fontcolor = font_colors[id];
    	_fontColorChgText.color = fontcolor;

    	Util.SaveData(GetDataKey(DataKeys.Color),color_id.ToString());
    	
    }

    // 通知デバッグon
    public void OnClickNotifySet(){
    	int id = _dp_notify_id.value;
    	androidObj.DebugSet(id);
    }
    // 通知デバッグoff
    public void OnClickNotifyReset(){
    	int id = _dp_notify_id.value;
    	androidObj.DebugReset(id);
    }
}
