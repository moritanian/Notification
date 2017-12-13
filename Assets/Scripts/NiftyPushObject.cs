using UnityEngine;
using System.Collections;

/////////////////////////ここから追加コード////////////////////////
//SDKの利用を宣言
using NCMB;
/////////////////////////ここまで追加コード////////////////////////

using System.Collections.Generic;

public class NiftyPushObject : MonoBehaviour {

/////////////////////////ここから追加コード////////////////////////
    private static bool _isInitialized = false;

    /// <summary>
    ///イベントリスナーの登録
    /// </summary>
    void OnEnable()
    {
        NCMBManager.onRegistration += OnRegistration;
        NCMBManager.onNotificationReceived += OnNotificationReceived;
    }

    /// <summary>
    ///イベントリスナーの削除
    /// </summary>
    void OnDisable ()
    {
        NCMBManager.onRegistration -= OnRegistration;
        NCMBManager.onNotificationReceived -= OnNotificationReceived;
    }

    /// <summary>
    ///端末登録後のイベント
    /// </summary>
    void OnRegistration (string errorMessage)
    {
        if (errorMessage == null) {
            Debug.Log ("OnRegistrationSucceeded");
        } else {
            Debug.Log ("OnRegistrationFailed:" + errorMessage);
        }
    }

    /// <summary>
    ///メッセージ受信後のイベント
    /// </summary>
    void OnNotificationReceived(NCMBPushPayload payload)
    {
        Debug.Log("OnNotificationReceived");
    }

    /// <summary>
    ///シーンを跨いでGameObjectを利用する設定
    /// </summary>
    public virtual void Awake ()
    {
        if (!_isInitialized) {
            _isInitialized = true;
            DontDestroyOnLoad (this.gameObject);
        } else {
            Destroy (this.gameObject);
        }
    }


/////////////////////////ここまで追加コード////////////////////////

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

}