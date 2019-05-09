using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clackSoundPlayer : MonoBehaviour {

	public AudioClip clack;
	public AudioClip[] wood;
	public AudioClip[] dieHit;
	private AudioSource theAudio;


	// Use this for initialization
	void Start () {

		theAudio = GetComponent<AudioSource> ();

		theAudio.playOnAwake = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter (Collision collision)  //Plays Sound Whenever collision detected
	{
		if(theAudio.isPlaying)
			return;

		if (GetComponent<Rigidbody> ().velocity.sqrMagnitude > 100000f) {

			if(collision.collider.name == "d6(Clone)")
				theAudio.clip = dieHit[Random.Range(0,dieHit.Length)];

			else 
				theAudio.clip = wood[Random.Range(0,wood.Length)];

			theAudio.Play ();
		}
	}

}
