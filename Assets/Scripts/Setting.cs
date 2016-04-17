using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Setting : Token {


	Text outputText;
	Toggle _debugToggle;
	Toggle _normalToggle;
	public InputField _inputField;

	public static int fontsize = 20;
	public static int FontSize{
		get {return fontsize;}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void Awake(){
		outputText = MyCanvas.Find<Text>("outputText");
		_debugToggle = transform.FindChild("IsDebugLog").gameObject.GetComponent<Toggle>();
		_normalToggle = transform.FindChild("NormalToggle").gameObject.GetComponent<Toggle>();
	}

	// 設定保存
	public enum DataKeys{
		BGid,
		FontSize,
		Color
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
	public void OnChangeDebugToggle(){
		DebugLog.IsLogDebug = _debugToggle.isOn;
	}

	public void OnChangeNormalLog(){
		DebugLog.NormalLog = _normalToggle.isOn;
	}

	// fontsize フィールド変更
	public void OnChangeFontSize(){
		int font_size = int.Parse(_inputField.text); 
		if(font_size > 0)fontsize = font_size;
	}

	IEnumerator ImageR()
 	{
        yield return new WaitForSeconds(3.0f);
        GetComponent<Image>().color = new Color(255, 255,255 , 1.0f);
    }
}
