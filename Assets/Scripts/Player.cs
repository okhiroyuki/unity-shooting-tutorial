using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float m_Speed;
	public Shot m_shotPrefab; // 弾のプレハブ
	public float m_shotSpeed; // 弾の移動の速さ
	public float m_shotAngleRange; // 複数の弾を発射する時の角度
	public float m_shotTimer; // 弾の発射タイミングを管理するタイマー
	public int m_shotCount; // 弾の発射数
	public float m_shotInterval; // 弾の発射間隔（秒）
	public int m_hpMax; // HP の最大値
	public int m_hp; // HP
	// プレイヤーのインスタンスを管理する static 変数
	public static Player m_instance;

	// Use this for initialization
	void Start () {

	}

	// ゲーム開始時に呼び出される関数
	private void Awake()
	{
    	m_hp = m_hpMax; // HP
		// 他のクラスからプレイヤーを参照できるようにstatic 変数にインスタンス情報を格納する
		m_instance = this;
	}

	// Update is called once per frame
	void Update () {
		var h = Input.GetAxis("Horizontal");
		var v = Input.GetAxis("Vertical");

		var verocity = new Vector3(h,v) * m_Speed;
		transform.localPosition += verocity;
		transform.localPosition = Utils.ClampPosition(transform.localPosition);

		// プレイヤーのスクリーン座標を計算する
		var screenPos = Camera.main.WorldToScreenPoint( transform.position );

		// プレイヤーから見たマウスカーソルの方向を計算する
		var direction = Input.mousePosition - screenPos;

		// マウスカーソルが存在する方向の角度を取得する
		var angle = Utils.GetAngle( Vector3.zero, direction );

		// プレイヤーがマウスカーソルの方向を見るようにする
		var angles = transform.localEulerAngles;
		angles.z = angle - 90;
		transform.localEulerAngles = angles;

		// 弾の発射タイミングを管理するタイマーを更新する
		m_shotTimer += Time.deltaTime;

		// まだ弾の発射タイミングではない場合は、ここで処理を終える
		if ( m_shotTimer < m_shotInterval ) return;

		// 弾の発射タイミングを管理するタイマーをリセットする
		m_shotTimer = 0;

		// 弾を発射する
		ShootNWay( angle, m_shotAngleRange, m_shotSpeed, m_shotCount );
	}

	// 弾を発射する関数
	private void ShootNWay(	float angleBase, float angleRange, float speed, int count )
	{
		var pos = transform.localPosition; // プレイヤーの位置
		var rot = transform.localRotation; // プレイヤーの向き

		// 弾を複数発射する場合
		if ( 1 < count )
		{
			// 発射する回数分ループする
			for ( int i = 0; i < count; ++i )
			{
				// 弾の発射角度を計算する
				var angle = angleBase +
					angleRange * ( ( float )i / ( count - 1 ) - 0.5f );

				// 発射する弾を生成する
				var shot = Instantiate( m_shotPrefab, pos, rot );

				// 弾を発射する方向と速さを設定する
				shot.Init( angle, speed );
			}
		}
		// 弾を 1 つだけ発射する場合
		else if ( count == 1 )
		{
			// 発射する弾を生成する
			var shot = Instantiate( m_shotPrefab, pos, rot );

			// 弾を発射する方向と速さを設定する
			shot.Init( angleBase, speed );
		}
	}

	// ダメージを受ける関数
// 敵とぶつかった時に呼び出される
	public void Damage( int damage )
	{
		// HP を減らす
		m_hp -= damage;

		// HP がまだある場合、ここで処理を終える
		if ( 0 < m_hp ) return;

		// プレイヤーが死亡したので非表示にする
		// 本来であれば、ここでゲームオーバー演出を再生したりする
		gameObject.SetActive( false );
	}
}
