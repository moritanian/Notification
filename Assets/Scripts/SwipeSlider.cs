using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/* 
 * Swipe slide event invoke class
 * 
 * {
 * 		Swipe.SwipeStart (SwipeEvent)
 * 		Swipe.SwipeMove (SwipeEvent)
 * 		Swipe.SwipeEnd (SwipeEvent)
 * 		Swipe.SwipeCancel (SwipeEvent)
 *	}
 *	=>
 *	{
 *		SlideStart (SwipeSliderEvent)
 *		SlideMove  (SwipeSliderEvent)
 *		SlideNext (SwipeSliderEvent)
 *		SlideCancel (SwipeSliderEvent)
 *		SlideEnd (SwipeSliderEvent)
 *	}
 */ 
public class SwipeSlider : MonoBehaviour {
	
	[SerializeField]
	GameObject item1;
	[SerializeField]
	GameObject item2;

	[SerializeField]
	SwipeSliderEvent slideStart;

	[SerializeField]
	SwipeSliderEvent slideMove;

	[SerializeField]
	SwipeSliderEvent slideNext;

	[SerializeField]
	SwipeSliderEvent slideCancel;

	[SerializeField]
	SwipeSliderEvent slideEnd;

	[SerializeField]
	AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField]
	float slideAnimationTime = 0.1f;

	[SerializeField]
	Vector2 targetDirection = Vector2.right;

	[SerializeField]
	float slideMinDistance = 60.0f;

	Vector2 currentItemInitPosition;
	Vector2 nextItemInitPosition;
	float itemDistance;

	enum State {
		Idle,
		SwipeSliding,
		AutoSliding,
		ReturnSliding
	}

	State state = State.Idle;

	SwipeSliderEventObj eventObj;

	Transform currentItemTransform {
		get{return eventObj.currentItem.transform;}
	}

	Transform nextItemTransform {
		get{return eventObj.nextItem.transform;}
	}

	void Awake(){
		eventObj = new SwipeSliderEventObj ();
		eventObj.currentItem = item1;
		eventObj.nextItem = item2;
	}

	// Use this for initialization
	void Start () {
		currentItemInitPosition = currentItemTransform.localPosition;
		nextItemInitPosition = nextItemTransform.localPosition;
		itemDistance = Vector2.Distance (currentItemInitPosition, nextItemInitPosition);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// should register in swipe
	public void OnSwipeMove(SwipeEventObj obj){

		eventObj.directionDelta = Vector2.Dot (obj.deltaVector, targetDirection);
		eventObj.slideDirection = eventObj.directionDelta > 0 ? 1 : -1;

		if (state == State.Idle) {
			StartSlide ();

		} else if (state == State.AutoSliding) {
			// TODO cancel animation
			StartSlide ();

		}

		if (state == State.SwipeSliding) {
			SetItemsPosition ( 
				eventObj.directionDelta / itemDistance * eventObj.slideDirection
			);
			InvokeSlideMove ();
		}

	}

	// should register in swipe
	public void OnSwipeEnd(SwipeEventObj obj){
		if (eventObj.directionDelta * eventObj.slideDirection > slideMinDistance) {
			SlideAuto ();
			Debug.Log ("slideAuto");
			InvokeSlideNext ();
		} else {
			SlideReturn ();
			Debug.Log ("slideReturn");

			InvokeSlideNext ();
		}
	}


	// should register in swipe
	public void OnSwipeCancel(SwipeEventObj obj){
		// TODO check swipe is successed
		Debug.Log("OnSwipeCancel");
		if (state == State.SwipeSliding) {
			SlideReturn ();
		} else if (state == State.AutoSliding) {
			SlideReturn ();

		}
	}

	// show slide animation without swipe or complete slide if slide animation has not completed by swipe.
	void SlideAuto(){
		state = State.AutoSliding;
		StartCoroutine( SlideAnimation() );
	}

	void SlideReturn(){
		state = State.ReturnSliding;
		StartCoroutine( SlideAnimation() );
	}
		
	private IEnumerator SlideAnimation(){

		// degree of progress
		float initProgress =  eventObj.directionDelta *  eventObj.slideDirection / itemDistance;
		// time to move remain distance
		float duration;
		if (state == State.AutoSliding) {
			duration = (1.0f - initProgress) * slideAnimationTime;
		} else {
			duration = initProgress * slideAnimationTime;
		}

		// start time
		float startTime = Time.time;

		float progress;

		// animate until finished
		if (state == State.AutoSliding) {
			while ((progress = initProgress + (1.0f - initProgress) * (Time.time - startTime) / duration) < 1.0f) {
				SetItemsPosition (animCurve.Evaluate (progress));
				yield return 0;        // 1フレーム後、再開
			}
		} else {
			while ((progress = initProgress - (initProgress) * (Time.time - startTime) / duration) > 0.0f) {
				SetItemsPosition (animCurve.Evaluate (progress));
				yield return 0;        // 1フレーム後、再開
			}
		}
		FinishSlide();
	}


	void StartSlide(){

		state = State.SwipeSliding;

		// init nextItem position
		nextItemTransform.localPosition 
			= currentItemInitPosition + (nextItemInitPosition - currentItemInitPosition) *  eventObj.slideDirection;

		InvokeSlideStart ();

		Debug.Log ("OnStart");

	}

	void FinishSlide(){

		if (state == State.SwipeSliding) {
			InvokeSlideEnd ();
			Debug.Log ("ENd");
			// swap item
			GameObject temp = eventObj.nextItem;
			eventObj.nextItem = eventObj.currentItem;
			eventObj.currentItem = temp;

		} else {
			InvokeSlideCancel ();
			Debug.Log ("Cancel");
		}

		state = State.Idle;

		nextItemTransform.localPosition = nextItemInitPosition;
		currentItemTransform.transform.localPosition = currentItemInitPosition;

		currentItemTransform.transform.localScale =  Vector3.one ;
		nextItemTransform.localScale = Vector3.one;

		currentItemTransform.localRotation = Quaternion.AngleAxis(0,  Vector3.forward);
		nextItemTransform.localRotation = Quaternion.AngleAxis(0,  Vector3.forward);

	}

	public GameObject getCurrentItem(){
		return eventObj.currentItem;
	}

	private void SetItemsPosition(float ratio){

		Vector2 moveDelta =  ( eventObj.slideDirection * ratio * itemDistance) * targetDirection;

		currentItemTransform.localPosition = currentItemInitPosition + moveDelta;
		nextItemTransform.localPosition = currentItemInitPosition + (nextItemInitPosition - currentItemInitPosition) *  eventObj.slideDirection + moveDelta;

		currentItemTransform.transform.localPosition += ratio * 3.0f * Vector3.up;
		nextItemTransform.localPosition +=  (1.0f - ratio) * 3.0f * Vector3.up;

		//currentItem.transform.localScale = (1.0f - ratio) * Vector3.one ;
		//nextItem.transform.localScale = ratio * Vector3.one;
		Vector3 angleAxis = new Vector3(0, 0.9f, -0.05f);
		currentItemTransform.localRotation = Quaternion.AngleAxis(ratio * 130.0f * (- eventObj.slideDirection), angleAxis);
		nextItemTransform.localRotation = Quaternion.AngleAxis(-(1.0f - ratio) * 130.0f *(- eventObj.slideDirection),  angleAxis);
	}

	void InvokeSlideStart (){
		eventObj.type = "SlideStart";
		eventObj.isAutoSlide = false;
		eventObj.isReturnSlide = false;
		slideStart.Invoke (eventObj);
	}

	void InvokeSlideMove (){
		eventObj.type = "SlideMove";
		slideMove.Invoke (eventObj);
	}

	void InvokeSlideNext (){
		eventObj.type = "SlideNext";
		eventObj.isAutoSlide = state == State.AutoSliding;
		eventObj.isReturnSlide = state == State.ReturnSliding;
		slideNext.Invoke (eventObj);
	}

	void InvokeSlideCancel (){
		eventObj.type = "SlideCancel";
		slideCancel.Invoke (eventObj);
	}

	void InvokeSlideEnd (){
		eventObj.type = "SlideEnd";
		eventObj.isAutoSlide = false;
		eventObj.isReturnSlide = false;
		slideEnd.Invoke (eventObj);
	}
}
