using UnityEngine;
using System.Collections;

/*
 * Resize window corresponding to the screen size
 */
public class WindowResizer : MonoBehaviour {

	[SerializeField]
	bool resizeAlways = true;
	[SerializeField]
	bool resizeWithKeyboard = false;

	RectTransform rect;

	void Awake(){
		rect = GetComponent<RectTransform> ();
	}

	// Use this for initialization
	void Start () {

		ResizeWithCanvas();

		if (resizeWithKeyboard) {
			PluginMsgHandler.getInst ().OnShowKeyboard += (bool bKeyboardShow, int nKeyHeight) => {
				Debug.Log ("OnSHowKeyboard" + bKeyboardShow + nKeyHeight + " | " + Screen.height);
				OnShowKeyboard(bKeyboardShow, nKeyHeight);
			};
		}

	}
	
	// Update is called once per frame
	void Update () {
	
		if (resizeAlways) {
			ResizeWithCanvas ();
		}
	}

	void ResizeWithCanvas(){
		rect.sizeDelta = MyCanvas.canvasSize;
	}

	void OnShowKeyboard(bool keyboardShow, int keyHeight){
		// TODO time management
		ResizeWithKeyboard (keyHeight);
	}

	void ResizeWithKeyboard(int keyHeight){
		float sizeX = MyCanvas.canvasScale.x;
		float sizeY = MyCanvas.canvasScale.y;
		Vector2 sizeDelta = new Vector2( Screen.width / sizeX , (Screen.height - keyHeight) / sizeY) ;
		rect.sizeDelta = sizeDelta;
		Vector3 pos = rect.localPosition;
		rect.localPosition = new Vector3(pos.x, keyHeight/sizeY /2.0f, pos.z);
	}
}
