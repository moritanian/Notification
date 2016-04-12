using UnityEngine;
using System.Collections;

public class ScrollController : MonoBehaviour {

	public ScrollController scroll;
	// Use this for initialization
	void Awake () {
		scroll = GetComponent<ScrollController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	// Content の下にオブジェクトを置き、スクロールできるようにする
	public void SetContent(Transform tf){
		if(tf == null) Debug.Log("scroll null");
		tf.SetParent(scroll.transform,false);
	}

}
