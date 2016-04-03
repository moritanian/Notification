using UnityEngine;
using System.Collections;

public class ScrollController : MonoBehaviour {

	public static ScrollController scroll;
	// Use this for initialization
	void Start () {
		scroll = GetComponent<ScrollController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	// Content の下にオブジェクトを置き、スクロールできるようにする
	public static void SetContent(Transform tf){
		//scroll = GetComponent<ScrollController>();
		if(scroll.transform == null) Debug.Log("scroll null");
		if(tf == null) Debug.Log("scroll null");
		tf.SetParent(scroll.transform,false);
	}

}
