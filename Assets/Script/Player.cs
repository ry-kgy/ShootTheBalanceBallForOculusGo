﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private Transform _RightHandAnchor;

    [SerializeField]
    private Transform _LeftHandAnchor;

    [SerializeField]
    private Transform _CenterEyeAnchor;

    [SerializeField]
    private float _MaxDistance = 100.0f;

    [SerializeField]
    private LineRenderer _LaserPointerRenderer;

    Rigidbody m_Rigidbody;

    private Transform Pointer
    {
        get
        {
            // 現在アクティブなコントローラーを取得
            var controller = OVRInput.GetActiveController();
            switch (controller)
            {
                case OVRInput.Controller.RTrackedRemote:
                    Debug.Log("Right");
                    return _RightHandAnchor;

                case OVRInput.Controller.LTrackedRemote:
                    Debug.Log("LEft");
                    return _LeftHandAnchor;

                default:
                    return _CenterEyeAnchor; // どちらも取れなければ目の間からビームが出る
            }
        }
    }

    void Start()
    {
        // 自分のRigidbodyを取ってくる
        m_Rigidbody = GetComponent<Rigidbody>();
    }


	// Update is called once per frame
	void Update () {
		
        var pointer = Pointer;
        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        // コントローラー位置からRayを飛ばす
        Ray pointerRay = new Ray(pointer.position, pointer.forward);

        // レーザーの起点
        _LaserPointerRenderer.SetPosition(0, pointerRay.origin);

        RaycastHit hitInfo;
        if (Physics.Raycast(pointerRay, out hitInfo, _MaxDistance))
        {
            // Rayがヒットしたらそこまで
            _LaserPointerRenderer.SetPosition(1, hitInfo.point);

            if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && hitInfo.collider.tag == "BlanceBall") {
                Destroy(hitInfo.collider.gameObject); 
            }

        }
        else
        {
            // Rayがヒットしなかったら向いている方向にMaxDistance伸ばす
            _LaserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _MaxDistance);
        }

        // WASDで移動する
        float x = 0.0f;
        float z = 0.0f;

        if (OVRInput.Get(OVRInput.Button.Right))
        {
            x += 1.0f;
        }
        if (OVRInput.Get(OVRInput.Button.Left))
        {
            x -= 1.0f;
        }
        if (OVRInput.Get(OVRInput.Button.Up))
        {

            z += 1.0f;
        }
        if (OVRInput.Get(OVRInput.Button.Down))
        {
            z -= 1.0f;
        }

        Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);


        m_Rigidbody.velocity = new Vector3(vector.x, 0.0f, vector.y);

	}
}
