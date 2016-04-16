using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Setting : Token {


	Text outputText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Awake(){
		outputText = MyCanvas.Find<Text>("outputText");
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
		// 背景画像変更
		int id = MyCanvas.Find<BackGound>("BackGround").BGChange();
		Util.SaveData(GetDataKey(DataKeys.BGid), id.ToString());
		GetComponent<Image>().color = new Color(255, 255,255 , 0.5f); 
		StartCoroutine(ImageR());
	}

	IEnumerator ImageR()
 	{
        yield return new WaitForSeconds(3.0f);
        GetComponent<Image>().color = new Color(255, 255,255 , 1.0f);
    }
}
