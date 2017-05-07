using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public bool isLocalPlayer = false;

	Vector3 oldPosition;
	Vector3 currentPosition;
	Quaternion oldRotation;
	Quaternion currentRotation;

	void Start () {
		oldPosition = transform.position;
		currentPosition = oldPosition;
		oldRotation = transform.rotation;
		currentRotation = oldRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer) return;

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
		transform.Rotate(0, x, 0);
		transform.Translate( 0, 0, z);

		currentPosition = transform.position;
		currentRotation = transform.rotation;

		if( currentPosition != oldPosition){
			oldPosition = currentPosition;
		}

		if( currentRotation != oldRotation){
			oldRotation = currentRotation;
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			CmdFire();
		}

	}

	public void CmdFire(){
		var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;
		Bullet b = bullet.GetComponent<Bullet>();
		b.playerFrom = this.gameObject;
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 6;

		Destroy(bullet, 2.0f);
	}
}
