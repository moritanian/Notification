using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyCanvas : MonoBehaviour {

  static Canvas _canvas;
  void Awake () {
    // Canvasコンポーネントを保持
    _canvas = GetComponent<Canvas>();
  }

  /// 表示・非表示を設定する
  public static void SetActive(string name, bool b) {
    foreach(Transform child in _canvas.transform) {
      // 子の要素をたどる
      if(child.name == name) {
        // 指定した名前と一致
        // 表示フラグを設定
        child.gameObject.SetActive(b);
        // おしまい
        return;
      }
    }
    // 指定したオブジェクト名が見つからなかった
    Debug.LogWarning("Not found objname:"+name);
  }

  /// オブジェクトを検索する
  public static Type Find<Type>(string name) {
    return GameObject.Find(name).GetComponent<Type>();
  }
/*
  public static void SetCanvasChild<Type>(Type obj){
    obj.transform.SetParent(_canvas.transform,false);
  }
  */
  // 再帰的にオブジェクトを探索
  static GameObject  SearchObj(string name, Transform children){
    Debug.Log(name);
    foreach (Transform child in children.transform){
      Debug.Log(child.name);
      if(child.name == name){
        return child.gameObject;
      }else if(child.transform != null){
        GameObject obj = SearchObj(name, child);
        if(obj) return obj.gameObject;
      } 
    }
    return null;
  }

  // 再帰的に探す
  public static Type FindFromAll<Type>(string name){
    
    GameObject obj = SearchObj(name, _canvas.transform);
    return obj.GetComponent<Type>();
  } 

  public static Type FindChild<Type>(string name) { //非表示のゲームオブジェクトを取得
  
 // if(_canvas.transform == null) Debug.Log("canvas_null");
    foreach(Transform child in _canvas.transform) {
     // Debug.Log("FindChild " + child.name);
      // 子の要素をたどる

      if(child.name == name) {
        // 指定した名前と一致
        // 表示フラグを設定
        //return  child.GetComponent<Type>();
        return child.gameObject.GetComponent<Type> ();
      }

    }
    return _canvas.transform.FindChild(name).gameObject.GetComponent<Type>();
  }

  public static Canvas GetCanvas(){
    return _canvas;
  }

  // ワールド座標をcanvas座標に変換
  public static Vector2 GetCanvasPosFromWorld(Vector2 WorldPos){
    
    Vector2 result = Vector2.zero;
    RectTransform CanvasRect = MyCanvas._canvas.GetComponent<RectTransform>();
    // レンダラモードにより変換方法は異なる
    switch(MyCanvas._canvas.GetComponent<Canvas>().renderMode){
      case RenderMode.ScreenSpaceOverlay:
        result = MyCanvas._canvas.transform.InverseTransformPoint(WorldPos);
        break;

      case RenderMode.ScreenSpaceCamera:
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, WorldPos, Camera.main, out result);
        break;

      case RenderMode.WorldSpace:
        Vector3 result_3v = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(CanvasRect, WorldPos, Camera.main, out result_3v);
        result.x = result_3v.x;
        result.y = result_3v.y;
        break;

      default: 
        break;
    }
    /*    
     //first you need the RectTransform component of your canvas
    RectTransform CanvasRect = MyCanvas._canvas.GetComponent<RectTransform>();

    //then you calculate the position of the UI element
    //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

    Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(WorldPos);
    Vector2 WorldObject_ScreenPosition = new Vector2(
      ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
      ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
    return WorldObject_ScreenPosition;
  */
    return result;
  }
}
