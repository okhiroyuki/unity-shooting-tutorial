using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// 爆発エフェクトが生成された時に呼び出される関数
        // 演出が完了したら削除する
        var particleSystem = GetComponent<ParticleSystem>();
        Destroy( gameObject, particleSystem.main.duration );
	}

	// Update is called once per frame
	void Update () {

	}
}
