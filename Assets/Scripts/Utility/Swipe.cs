using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/* 
 * Swipe event invoke class
 * 
 * {}
 * 	=> 
 * {
 * 		SwipeStart(SwipeEvent)
 * 		SwipeMove(SwipeEvent)
 * 		SwipeEnd(SwipeEvent)
 * 		SwipeCancel(SwipeEvent)
 *	}
 */ 
public class Swipe : Token {


	Vector2 startPosition;
	Vector2 endPosition;

	[SerializeField] // on touch down event
	SwipeEvent swipeStart;

	[SerializeField] // swipe end event
	SwipeEvent swipeEnd;

	[SerializeField] // cancel swipe event
	SwipeEvent swipeCancel;

	[SerializeField]  // swiping event
	SwipeEvent swipeMove;

	float margin_delta = 0.4f; // これ以上スワイプすれば。長押しイベントがキャンセル

	enum State {
		Idle,
		Down,
		Swipe
	}
	State state;

	Vector2 ObjPos1;
	Vector2 ObjPos2;

	SwipeEventObj eventObj;

	// Use this for initialization
	void Start () {
		
		state = State.Idle;
		RectTransform rt = GetComponent<RectTransform> ();
		Vector2 sd = rt.sizeDelta;

		// 自身の左上と右下を取得
		ObjPos1.x = local_X - rt.rect.width/2.0f;
		ObjPos1.y = local_Y - rt.rect.height/2.0f;
		ObjPos2.x = local_X + rt.rect.width/2.0f;
		ObjPos2.y = local_Y + rt.rect.height/2.0f;

		eventObj = new SwipeEventObj ();
	}

	public void LogObjPos(){
		Debug.Log(name + "center: " +  local_X + ":" + local_Y);
		Debug.Log("Objpos1 = " + ObjPos1.x + ": " + ObjPos1.y);
		Debug.Log("Objpos2 = " + ObjPos2.x + ": " + ObjPos2.y);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateStatus();
	}

	void UpdateStatus(){

		eventObj.currentPosition = GetPosition();

		if (Input.GetMouseButtonUp (0) && (state == State.Down || state == State.Swipe)) {

			if (state == State.Down) {
				state = State.Idle;
				return;
			}

			state = State.Idle;

			if (!IsIn (eventObj.currentPosition)) {
				InvokeSwipeCancel ();
				return;
			}

			InvokeSwipeEnd ();
		}

		if (state == State.Down) {
		
			if (!IsIn (eventObj.currentPosition)) {
				return;
			}

			if (eventObj.deltaVector.sqrMagnitude > 0) {
				state = State.Swipe;
			}

		}

		if (state == State.Swipe) {
			InvokeSwipMove ();
		}

		
		if (Input.GetMouseButtonDown (0) && state == State.Idle) {
			if(!IsIn(eventObj.currentPosition))return;
			state = State.Down;
			eventObj.startPosition = eventObj.currentPosition;
			//InvokeSwipeStart ();
		}


	}
	// canvas内での位置
	Vector2 GetPosition(){
		Vector2 pos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		return MyCanvas.GetCanvasPosFromWorld(pos);
	}

	bool IsIn(Vector2 pos){
		return (ObjPos1.x < pos.x && pos.x < ObjPos2.x && ObjPos1.y < pos.y && pos.y < ObjPos2.y );
	}

	void InvokeSwipeStart(){
		eventObj.type = "SwipeStart";
		swipeStart.Invoke (eventObj);	
	}

	void InvokeSwipeEnd(){
		eventObj.type = "SwipeEnd";
		swipeEnd.Invoke (eventObj);
	}

	void InvokeSwipeCancel(){
		eventObj.type = "SwipeCancel";
		swipeCancel.Invoke (eventObj);
	}

	void InvokeSwipMove(){
		eventObj.type = "SwipeMove";
		swipeMove.Invoke (eventObj);
	}

}
