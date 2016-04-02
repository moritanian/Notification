using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TodoField : Token {

	public InputField _inputField;
	public Text text;
	public static TodoField Add(float x,float y){
		TodoField obj = CreateInstanceEasy<TodoField>("TodoField",x,y);
		//MyCanvas.SetCanvasChild<TodoField>(obj);
		obj.transform.SetParent(MyCanvas.GetCanvas().transform,false);
		return obj;
	}

	// Use this for initialization
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public string GetText(){
		return _inputField.text;
	}
	public void OnclickSave(){
		TextSave();
	}

	void TextSave(){
		string _text = GetText();
		Debug.Log(_text);
	}

}
