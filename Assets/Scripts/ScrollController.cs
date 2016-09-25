using UnityEngine;
using System.Collections;

public class ScrollController : MonoBehaviour {

	public ScrollController scroll;
	public bool IsHorizontalMove;

	float scrollContent_init_y = 0; // スクロールコンテント　y初期位置
	
	// Use this for initialization
	void Awake () {
		scroll = GetComponent<ScrollController>();
		
	}
	void Start(){

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

	// 指定された要素がトップになるようにスクロール
	public void ScrollElelmentTop(Token element){
		RectTransform rect =  element.GetComponent<RectTransform>();
		float element_width = rect.sizeDelta.y;
		float element_y = rect.position.y;

		Vector3 pos = transform.position;
		pos.y = scrollContent_init_y - pos.y + element_y /2.0f;
		transform.position = pos;
	}

	//最上へスクロール
	public void ScrollTop(){
		if(scrollContent_init_y == 0){
			scrollContent_init_y = scroll.GetComponent<RectTransform>().position.y;
			Debug.Log(scrollContent_init_y.ToString());
		}
		Vector3 pos = transform.position;
		pos.y = scrollContent_init_y;
		transform.position = pos;
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
