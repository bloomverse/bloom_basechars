using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloomiePickup : MonoBehaviour
{

    [SerializeField] private GameObject pickEffect;
     public AudioClip SoundClip;
    private AudioSource source;
    private  bool picked;
    private Rigidbody rb;
    private float rotationSpeed = 30f;

    void Update(){
        //transform.Rotate(0, Time.deltaTime * rotationSpeed,0);
    }

    void Start(){
        source = GetComponent<AudioSource>();
         source.clip = SoundClip;
         rb = GetComponent<Rigidbody>();
//         Debug.Log("llego aqui");
    }

 void OnTriggerStay(Collider other) {
    if(other.gameObject.tag=="Player" && !picked){
        Debug.Log("Stayed");
        picked = true;
        Instantiate(pickEffect, transform.position,transform.rotation);
        source.PlayOneShot(SoundClip);
        rb.velocity = new Vector3(0, 5, 0);
        Destroy(gameObject,5);

    }
  }
  
}
