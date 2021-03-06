﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// uGUI Image操作モジュール

/// </summary>
public class ImageObj : Token{
	
	
	/// RectTransform
	RectTransform _rectTransform = null;
	public RectTransform RectTrans
	{
		get { return _rectTransform ?? ( _rectTransform = GetComponent<RectTransform>()); }
	}
	
	/// uGUI image
	Image _image = null;
	public Image UiImage
	{
		get { return _image ?? (_image = GetComponent<Image>()); }
	}
	// uGUI Image の　sprite
	public Sprite image
	{
		get { return UiImage.sprite; }
		set { UiImage.sprite = value; }
	}
	public Color ImageColor
	{

		get { return UiImage.color;}
		set {UiImage.color = value;}
	}

	
	/// <summary>
	/// 描画フラグ
	/// </summary>
	/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
	public bool Visible
	{
		get { return UiImage.enabled; }
		set { UiImage.enabled = value; }
	}
	
	/// <summary>
	/// X座標
	/// </summary>
	/// <value>The x.</value>
	public float X
	{
		get { return RectTrans.localPosition.x; }
		set
		{
			Vector3 p = RectTrans.localPosition;
			p.x = value;
			transform.localPosition = p;
		}
	}
	
	/// <summary>
	/// Y座標
	/// </summary>
	/// <value>The y.</value>
	public float Y
	{
		get { return RectTrans.localPosition.y; }
		set
		{
			Vector3 p = RectTrans.localPosition;
			p.y = value;
			RectTrans.localPosition = p;
		}
	}
	
	/// <summary>
	/// 座標を設定する
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SetPosition(float x, float y)
	{
		Vector3 p = RectTrans.localPosition;
		p.Set(x, y, p.z);
		RectTrans.localPosition = p;
	}
	
	/// <summary>
	/// 座標を足し込む
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void AddPosition(float x, float y)
	{
		Vector3 p = RectTrans.localPosition;
		p.x += x;
		p.y += y;
		RectTrans.localPosition = p;
		
	}
	
	/// 移動量を設定.
	public void SetVelocity (float direction, float speed)
	{
		Vector2 v;
		v.x = Util.CosEx (direction) * speed;
		v.y = Util.SinEx (direction) * speed;
		RigidBody.velocity = v;
	}
	
	/// 移動量を設定(X/Y).
	public void SetVelocityXY (float vx, float vy)
	{
		Vector2 v;
		v.x = vx;
		v.y = vy;
		RigidBody.velocity = v;
	}
	
	/// 移動量をかける.
	public void MulVelocity (float d)
	{
		RigidBody.velocity *= d;
	}
	
	/// 移動量(X).
	public float VX {
		get { return RigidBody.velocity.x; }
		set {
			Vector2 v = RigidBody.velocity;
			v.x = value;
			RigidBody.velocity = v;
		}
	}
	
	/// 移動量(Y).
	public float VY {
		get { return RigidBody.velocity.y; }
		set {
			Vector2 v = RigidBody.velocity;
			v.y = value;
			RigidBody.velocity = v;
		}
	}
	
	/// 方向.
	public float Direction {
		get {
			Vector2 v = RigidBody.velocity;
			return Mathf.Atan2 (v.y, v.x) * Mathf.Rad2Deg;
		}
	}
	
	/// 速度.
	public float Speed {
		get {
			Vector2 v = RigidBody.velocity;
			return Mathf.Sqrt (v.x * v.x + v.y * v.y);
		}
	}
	
	/// 重力.
	public float GravityScale {
		get { return RigidBody.gravityScale; }
		set { RigidBody.gravityScale = value; }
	}
}
