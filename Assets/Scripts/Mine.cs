using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;



public class Mine : MonoBehaviour
{

    public float armTime = 3f;
    public float radius = 3f;
    public float force = 10f;
    private float _moveSpeed = 16f;
    public GameObject animObject;

    float countdown;
    bool hasExploded = false;
    public GameObject explosionEfffect;
    Rigidbody m_Rigidbody;

    public RaycastHit target { get; set; }
    public bool hit { get; set;}

    public float firingAngle = 5.0f;
    public float gravity = 9.8f;

     [SerializeField] StatusEffectData _data;
 
    
     private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>(); 
    }

    void Start()
    {
        countdown = armTime;    
       
       // m_Rigidbody.velocity = _moveSpeed * transform.forward;
    }

    

  
    private void OnCollisionEnter(Collision other) {
        //m_Rigidbody.isKinematic = true;
        
    
        
        // transform.position = other.contacts[0].point;
        // transform.rotation = Quaternion.LookRotation(other.contacts[0].normal);



        //m_Rigidbody.useGravity = false;
        //transform.position = new Vector3(transform.position.x,transform.position.y, other.contacts[0].normal.z);
    }
   
    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Enemy") && !hasExploded){
            hasExploded = true;
            Explode();
        }
        
    }


    public void StartMovement(){
         StartCoroutine(SimulateProjectile(done =>{
             if(done){    
                transform.position = (target.point +  target.normal*0.1f) ;
                transform.rotation = Quaternion.LookRotation(target.normal);

                // Scale Sphere
                animObject.transform.DOScale(new Vector3(20,20,20),2).SetEase(Ease.OutBounce);
                SphereCollider  sc= GetComponent<SphereCollider>();
                sc.enabled = true;
             }
         }));

       
        //transform.rotation = Quaternion.LookRotation(target.normal);

        
    }
IEnumerator SimulateProjectile(System.Action<bool> done)
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(.1f);
       
        // Move projectile to the position of throwing object + add some offset if needed.
            //transform.position = myTransform.position + new Vector3(0, 0.0f, 0);
       
        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, target.point);
 
        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
 
        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
 
        // Calculate flight time.
        float flightDuration = target_Distance / Vx;
   
        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target.point - transform.position );
        //Vector3 incomingVec = target.point - transform.position;
        //Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

        
       
        float elapse_time = 0;
 
        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
           
            elapse_time += Time.deltaTime;

         
 
            yield return elapse_time;
            if(elapse_time>=flightDuration){
                done(true);
            }
        }

        //  Match wiht surface
        


         
    }  


  



    void Explode(){
        Instantiate(explosionEfffect,transform.position,transform.rotation);

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToDestroy){
 
             Destructible dest  = nearbyObject.GetComponent<Destructible>();
            if(dest !=null){
                dest.destroyObj();
            }

            var effectable = nearbyObject.GetComponent<IEffectable>();
            if(effectable!=null){

                   //Doing RAYCAST TO GET hitpoint 
               RaycastHit hit;
                if (Physics.Raycast(transform.position, nearbyObject.transform.position, out hit))
                {
                    _data.hitpoint = hit;
                    
                }
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
