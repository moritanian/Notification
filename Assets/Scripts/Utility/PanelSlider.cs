using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
 
public class PanelSlider : MonoBehaviour {
    public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector3[] inPosition = new Vector3[2];        // スライドイン後の位置
    public Vector3 outPosition;      // スライドアウト後の位置
    public float duration = 1.0f;    // スライド時間（秒）

	private Action slideEndAction; 
 
	// スライドイン（Pauseボタンが押されたときに、これを呼ぶ）
	public void SlideIn(int id = 0, Action endAction = null){
		slideEndAction = endAction;
		StartCoroutine( StartSlidePanel(true,id) );
	}
 
    // スライドアウト
	public void SlideOut(Action endAction = null){
		slideEndAction = endAction;
        StartCoroutine( StartSlidePanel(false) );
    }
 
    private IEnumerator StartSlidePanel( bool isSlideIn ,int id=0){
        float startTime = Time.time;    // 開始時間
        Vector3 startPos = transform.localPosition;  // 開始位置
        Vector3 moveDistance;            // 移動距離および方向
 
        if( isSlideIn )
            moveDistance = (inPosition[id] - startPos);
        else
            moveDistance = (outPosition - startPos);
 
        while((Time.time - startTime) < duration){
            transform.localPosition = startPos + moveDistance * animCurve.Evaluate((Time.time - startTime) / duration);
            yield return 0;        // 1フレーム後、再開
        }
        transform.localPosition = startPos + moveDistance;
    	
		// set scroll 0 for NativeEditBox
		if(slideEndAction != null)slideEndAction();
	}
}