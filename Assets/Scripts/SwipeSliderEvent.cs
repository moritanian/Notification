using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

[Serializable]
public class SwipeSliderEvent : UnityEvent <SwipeSliderEventObj> {}


public struct SwipeSliderEventObj {
	public GameObject currentItem;
	public GameObject nextItem;
	public string type;
	public bool isAutoSlide;
	public bool isReturnSlide;
	public int slideDirection;
	public float directionDelta;
}
