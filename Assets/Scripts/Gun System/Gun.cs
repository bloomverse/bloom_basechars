using System.Collections;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private GameObject ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private GameObject BulletParticles;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;


    [SerializeField] private GameObject metalDecal ;

    [SerializeField] StatusEffectData _data;

    private Animator Animator;
    private float LastShootTime;

    //public RaycastHit target { get; set; }
    public RaycastHit target { get; set; }
    public bool hit { get; set;}

     // --- Audio ---
        public AudioClip GunShotClip;
        public AudioSource source;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        if(source != null) source.clip = GunShotClip;
       // Vector3 forward = BulletSpawnPoint.TransformDirection(BulletSpawnPoint.forward) * 10;
       //  Debug.DrawRay(BulletSpawnPoint.position, forward);
    }


    public void Shoot()
    {
       // if(PV.IsMine){
       //     PV.RPC("RPC_gunhit",RpcTarget.All);   
       // }

       RPC_gunhit();
         
    }


  
    void RPC_gunhit(){      
          if (LastShootTime + ShootDelay < Time.time )
        {
            GameObject trail = Instantiate(BulletParticles, BulletSpawnPoint.position, BulletSpawnPoint.rotation);
            StartCoroutine(SpawnTrail(trail, target));
            LastShootTime = Time.time;

         /*    transform.position = Vector3.MoveTowards(transform.position,target,speed * Time.deltaTime);
        if(!hit && Vector3.Distance(transform.position,target) < .01f){
            Destroy(gameObject);
        }*/


          //  Debug.Log("shootng");
            // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
            // For more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

           // Animator.SetBool("IsShooting", true);
            ShootingSystem.Play();
            
            if(hit && target.collider.gameObject.CompareTag("Destructible")){
                var effectable2 = target.collider.gameObject.GetComponent<IEffectable>();
                if (effectable2 != null)
                {
                    _data.hitpoint = target;
                    effectable2.ApplyEffect(_data);
                }


              /*      Rigidbody rigid = target.transform.gameObject.GetComponent<Rigidbody>();
                    float jumpForce = 5f;
                    if(rigid!=null){
                        rigid.AddForce((target.collider.gameObject.transform.position - target.point).normalized * jumpForce, ForceMode.VelocityChange);
                    }*/

                    
            }

            // APPLY EFFECT
             if (!hit || !target.collider.gameObject.CompareTag("Player") ) return;
        
            var effectable = target.collider.gameObject.GetComponent<IEffectable>();

            if (effectable != null)
            {
                  _data.hitpoint = target;
                effectable.ApplyEffect(_data);
            }

            

        }

        
        source.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
        source.pitch = Random.Range(audioPitch.x, audioPitch.y);
        source.PlayOneShot(GunShotClip);

       
    }


  

    private IEnumerator SpawnTrail(GameObject Trail, RaycastHit Hit)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position,Hit.point);

        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime * (100 - distance);

            yield return null;
        }
//        Animator.SetBool("IsShooting", false);

    if(hit){
        Trail.transform.position = Hit.point;
        Instantiate(ImpactParticleSystem, Hit.point +  Hit.normal*0.1f, Quaternion.LookRotation(Hit.normal));

      /*  if(!target.collider.gameObject.CompareTag("Enemy")){
            Instantiate(metalDecal, Hit.point +  Hit.normal*0.1f, Quaternion.LookRotation(Hit.normal));
        }*/
        

    }
        
        Destroy(Trail.gameObject, 2);
    }
}


