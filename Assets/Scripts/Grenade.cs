using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayFire;

public class Grenade : MonoBehaviour
{

    public float delay = 3f;
    public float radius = 5f;
    public float force = 20f;
    private float _moveSpeed = 22f;

    float countdown;
    bool hasExploded = false;
    public GameObject explosionEfffect;
    Rigidbody m_Rigidbody;

    [SerializeField] StatusEffectData _data;


     private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        countdown = delay;    
        m_Rigidbody.velocity = _moveSpeed * transform.forward;
    }

  
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0f && !hasExploded){
            Explode();
            hasExploded = true;
        }
    }


    void Explode(){
        Instantiate(explosionEfffect,transform.position,Quaternion.identity);

             Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToDestroy){

            var effectable = nearbyObject.GetComponent<IEffectable>();
            if(effectable!=null){

                   //Doing RAYCAST TO GET hitpoint 
               // _data.hitpoint = hit;
                effectable.ApplyEffect(_data);
            }
                GetComponent<RayfireBomb>().Explode(0f);
        }

        Destroy(gameObject);
    }
}



/*


    void Explode(){
        Instantiate(explosionEfffect,transform.position,Quaternion.identity);

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToDestroy){
 
             Destructible dest  = nearbyObject.GetComponent<Destructible>();
            if(dest !=null){
                dest.destroyObj();
            }

            var effectable = nearbyObject.GetComponent<IEffectable>();
            if(effectable!=null){

                   //Doing RAYCAST TO GET hitpoint 
               // _data.hitpoint = hit;
                effectable.ApplyEffect(_data);
            }


        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToMove){
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if(rb !=null){
                rb.AddExplosionForce(force,transform.position,radius);
            }
        }

        Destroy(gameObject);
    }
}
*/