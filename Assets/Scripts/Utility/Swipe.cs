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
public class Swipe : MonoBehaviour {


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

		StartCoroutine (GetTargetArea ());
		eventObj = new SwipeEventObj ();
	}

	IEnumerator GetTargetArea(){
		// wait one loop for screen size changing
		yield return null;

		RectTransform rt = GetComponent<RectTransform> ();
		Vector2 sd = rt.sizeDelta;

		// 自身の左上と右下を取得
		float w =  rt.rect.width;
		float h = rt.rect.height;
		float x = transform.localPosition.x;
		float y = transform.localPosition.y;
		ObjPos1.x = (x - w/2.0f);
		ObjPos1.y = (y - h/2.0f);
		ObjPos2.x = (x + w/2.0f);
		ObjPos2.y = (y + h/2.0f);

	}

	public void LogObjPos(){
		Debug.Log("Objpos1 = " + ObjPos1.x + ": " + ObjPos1.y);
		Debug.Log("Objpos2 = " + ObjPos2.x + ": " + ObjPos2.y);
	}

	// Update is called once per frame
	void Update () {
		UpdateStatus();
	}

	void UpdateStatus(){

		if (Input.GetMouseButtonUp (0) && (state == State.Down || state == State.Swipe)) {

			eventObj.currentPosition = GetPosition();

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

			eventObj.currentPosition = GetPosition();

			if (!IsIn (eventObj.currentPosition)) {
				return;
			}

			if (eventObj.deltaVector.sqrMagnitude > 0) {
				state = State.Swipe;
			}

		}

		if (state == State.Swipe) {
			eventObj.currentPosition = GetPosition();
			InvokeSwipMove ();
		}


		if (Input.GetMouseButtonDown (0) && state == State.Idle) {
			eventObj.currentPosition = GetPosition();

			if(!IsIn(eventObj.currentPosition))return;
			state = State.Down;
			eventObj.startPosition = eventObj.currentPosition;
			InvokeSwipeStart ();
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
