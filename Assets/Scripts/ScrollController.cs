using UnityEngine;
using System.Collections;

public class ScrollController : MonoBehaviour {

	public ScrollController scroll;
	public bool IsHorizontalMove;

	
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

	// 同じ子要素の中で最も最初にする
	public void SetContentFirst(Transform tf){
		SetContent(tf);
		Debug.Log("first");
		tf.transform.SetAsFirstSibling();
	}

	// 強制的にスクロールする
	public void Scroll(Vector2 scl_vec){
		if(!IsHorizontalMove)scl_vec.x = 0;
		Vector3 scl_pos =  transform.position;
		scl_pos.x += scl_vec.x;
		scl_pos.y += scl_vec.y;
		transform.position = scl_pos;
	}
}
