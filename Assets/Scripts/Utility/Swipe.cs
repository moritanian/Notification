using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Swipe : MonoBehaviour {


	Vector2 startPosition;
	Vector2 endPosition;
	[SerializeField]
	UnityEvent SwipeEventRight;
	[SerializeField]
	UnityEvent SwipeEventLeft;
	[SerializeField]
	UnityEvent SwipeEventUp;
	[SerializeField]
	UnityEvent SwipeEventDown;


	[SerializeField]
	SwipeEvent SyncSwipeEvent;

	float margin_distance = 60f;
	float margin_delta = 3f;

	enum State {
		Idle,
		Down
	}
	State state;

	Vector2 ObjPos1;
	Vector2 ObjPos2;

	Vector2 CrtPos;
	// Use this for initialization
	void Start () {
		//Application.OpenURL("https://github.com/moritanian/Notification");
		state = State.Idle;
		Vector3 center = transform.position;
		//float width = GetComponent<SpriteRenderer>().bounds.size.x;
		//float heught = GetComponent<SpriteRenderer>().bounds.size.y;
		Vector2 sd = GetComponent<RectTransform>().sizeDelta;

		ObjPos1.x = center.x - sd.x/2.0f;
		ObjPos1.y = center.y - sd.y/2.0f;
		ObjPos2.x = center.x + sd.x/2.0f;
		ObjPos2.y = center.y + sd.y/2.0f;
		LogObjPos();
	}

	public void LogObjPos(){
		Debug.Log("Objpos1 = " + ObjPos1.x + ": " + ObjPos1.y);
		Debug.Log("Objpos2 = " + ObjPos2.x + ": " + ObjPos2.y);
	}
	
	// Update is called once per frame
	void Update () {
		// async swipe event 処理
		GetPos();

		// sysnc swipe event 処理
		SyncSwipe();
	}

	void GetPos(){

		if (Input.GetMouseButtonDown (0) && state == State.Idle) {
			startPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			Debug.Log("Swipe Start" + startPosition.x + " : " + startPosition.y);
			if(!IsIn(startPosition))return;
			state = State.Down;
			CrtPos = startPosition;
		}
		if (Input.GetMouseButtonUp (0) && state == State.Down) {
			state = State.Idle;
			endPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			if(!IsIn(endPosition))return ;
			float distanceX = endPosition.x - startPosition.x;
			float distanceY = endPosition.y - startPosition.y;
			Debug.Log("swipe X" + distanceX.ToString());
			if(margin_distance < distanceX){
				SwipeEventRight.Invoke();
			}else if( -margin_distance > distanceX){
				SwipeEventLeft.Invoke();
			}
			if(margin_distance < distanceY){
				SwipeEventUp.Invoke();
			}else if( -margin_distance > distanceY){
				SwipeEventDown.Invoke();
			}
			
		}
	}

	void SyncSwipe(){
		if(state == State.Down){
			Vector2 newPos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			Vector2 pos_delta = newPos - CrtPos;
			CrtPos = newPos;
			if((pos_delta.x*pos_delta.x + pos_delta.y * pos_delta.y) > margin_delta*margin_delta){
				// 長押しイベントキャンセル
				LongPress.PressInit();

				SyncSwipeEvent.Invoke(pos_delta);
			}
			
		}
	}
	bool IsIn(Vector2 pos){
		return (ObjPos1.x < pos.x && pos.x < ObjPos2.x && ObjPos1.y < pos.y && pos.y < ObjPos2.y );
	}

}
