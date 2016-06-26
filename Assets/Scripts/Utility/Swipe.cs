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
	float margin_distance = 50f;

	enum State {
		Idle,
		Down
	}
	State state;
	// Use this for initialization
	void Start () {
		//Application.OpenURL("https://github.com/moritanian/Notification");
		state = State.Idle;
	}
	
	// Update is called once per frame
	void Update () {
		GetPos();
	}

	void GetPos(){

		if (Input.GetMouseButtonDown (0) && state == State.Idle) {
			startPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			state = State.Down;
		}
		if (Input.GetMouseButtonUp (0) && state == State.Down) {
			endPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			float distanceX = endPosition.x - startPosition.x;
			float distanceY = endPosition.y - startPosition.y;
			Debug.Log("swipe" + distanceX.ToString());
			if(margin_distance < distanceX){
				SwipeEventRight.Invoke();
			}else if( -margin_distance > distanceX){
				SwipeEventLeft.Invoke();
			}
			state = State.Idle;
		}
		
	
	}

}
