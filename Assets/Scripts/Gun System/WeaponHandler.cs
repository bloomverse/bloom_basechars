using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Fusion;
using Cinemachine;
using DG.Tweening;
using RayFire;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    public ParticleSystem fireParticleSystem;
    public Transform aimPoint;
    public Transform weaponPivot;
    public LayerMask collisionLayers;

    // NEW VARS 
    [SerializeField]
    private GameObject ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private GameObject BulletParticles;
    [SerializeField]
    private float ShootDelay = 0.2f;
    [SerializeField]
    private LayerMask Mask;


    [SerializeField] private OverrideTransform WeaponRecoil;
	[SerializeField] private OverrideTransform BodyRecoil;

    [SerializeField] StatusEffectData _data;

    [Header("RayFire")]
	[SerializeField] private RayfireGun rayfireGun;
	[SerializeField] private Transform  rayfireTarget;

    float lastTimeFired = 0;

    //Other components
    HPHandler hpHandler;

    NetworkPlayer networkPlayer;

    private  GameObject trail ;

      // --- Audio ---
    public AudioClip GunShotClip;
    public AudioSource source;
    public Vector2 audioPitch = new Vector2(.9f, 1.1f);

    // RECOIL SYSTEM
		  public float verticalRecoilMin = 0.1f;
		  public float verticalRecoilMax = 0.2f;
		  public float horizontalRecoilMin = -0.14f;
		  public float horizontalRecoilMax = 0.14f;

    private void Awake()
    {
        hpHandler = GetComponent<HPHandler>();
        networkPlayer = GetBehaviour<NetworkPlayer>();

        if(source != null) source.clip = GunShotClip;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (hpHandler.isDead)
            return;

        //Get the input from the network
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireButtonPressed)
                Fire(networkInputData.aimForwardVector, networkInputData.cameraTransform);  
        }
    }

    void Fire(Vector3 aimForwardVector,Vector3 cameratransform)
    {
        //Limit fire rate
        if (Time.time - lastTimeFired < ShootDelay)
            return;


            float hitDistance = 100;
       
        //StartCoroutine(FireEffectCO());
        //aimPoint.position
        Runner.LagCompensation.Raycast(cameratransform, aimForwardVector, hitDistance, Object.InputAuthority, out var hitinfo, collisionLayers, HitOptions.IncludePhysX );

        
        bool isHitOtherPlayer = false;

   
        if (hitinfo.Distance > 0)
            hitDistance = hitinfo.Distance;

        if (hitinfo.Hitbox != null)
        {

            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitinfo.Hitbox.transform.root.name}");

            if (Object.HasStateAuthority)
                hitinfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName.ToString(),5);
               
            isHitOtherPlayer = true;

        }
        else if (hitinfo.Collider != null)
        {
           // Debug.Log($"{Time.time} {transform.name} hit PhysX collider {hitinfo.Collider.transform.name}");
           var rayFireColl = hitinfo.Collider.gameObject.GetComponent<Rigidbody>();

			if(rayFireColl){
				rayfireTarget.position = hitinfo.Point;
				rayfireGun.Shoot();
            
			}

        }

        trail = Instantiate(BulletParticles, weaponPivot.position, weaponPivot.rotation);
        StartCoroutine(SpawnTrail(trail, hitinfo.Point, hitinfo.Normal));

        source.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
        source.pitch = Random.Range(audioPitch.x, audioPitch.y);
        source.PlayOneShot(GunShotClip);

        Debug.Log(hitinfo.Normal);
        
        //Debug
        if (isHitOtherPlayer)
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
        else Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);

        //ShakeManager.instance.shakeCamera(1.4f,0.2f);

        lastTimeFired = Time.time;
    }

    IEnumerator SpawnTrail(GameObject Trail, Vector3 Hit, Vector3 Normal)
    {
        isFiring = true;
        float time = 0;
        fireParticleSystem.Play();

        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position,Hit);

       // Debug.Log(distance + "Distance Spawn trail" + Trail.transform.position);

        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, Hit, time);
            time += Time.deltaTime * (100 - distance);

            yield return null;
        }
//        Animator.SetBool("IsShooting", false);


    
       //Trail.transform.position = Hit.point;
        var impact = Instantiate(ImpactParticleSystem, Hit +  Normal*0.1f, Quaternion.LookRotation(Hit));

        //if(!target.collider.gameObject.CompareTag("Enemy")){
         //   Instantiate(metalDecal, Hit +  Normal*0.1f, Quaternion.LookRotation(Normal));
        //}
    
        
         yield return new WaitForSeconds(0.09f);
         isFiring = false;
         Destroy(Trail.gameObject, 1);
         Destroy(impact, 2);
         GenerateRecoil();
    }


    private void GenerateRecoil(){
			var brain = CinemachineCore.Instance.GetActiveBrain(0);
			var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

			if(Vcam!=null ){


			CinemachinePOV pov = Vcam.GetCinemachineComponent<CinemachinePOV>();
			
			try {
				pov.m_VerticalAxis.Value -= Random.Range(verticalRecoilMin,verticalRecoilMax);
			    pov.m_HorizontalAxis.Value -= Random.Range(horizontalRecoilMin,horizontalRecoilMax);
			/*DOTween.To(() =>0, (u) => WeaponRecoil.weight = u,1f,.1f).OnComplete(() =>{
					DOTween.To(() => 1 , (u) => WeaponRecoil.weight = u ,0f,.1f);
			});*/
			DOTween.To(() =>0, (u) => BodyRecoil.weight = u,1f,.1f).OnComplete(() =>{
					DOTween.To(() => 1 , (u) => BodyRecoil.weight = u ,0f,.1f);
			});

            DOTween.To(() =>0, (u) => WeaponRecoil.weight = u,1f,.1f).OnComplete(() =>{
					DOTween.To(() => 1 , (u) => WeaponRecoil.weight = u ,0f,.1f);
			});
			} catch{}
			


			//DOTween.To(currBodyRecoil,currBodyRecoil,1,.3f).SetLoops(1,LoopType.Yoyo);
			
			
			//WeaponRecoil.weight = currWeaponRecoil;
			//BodyRecoil.weight = currBodyRecoil ;
			}
		

		}

        
   

    IEnumerator FireEffectCO()
    {
        isFiring = true;

        fireParticleSystem.Play();

        yield return new WaitForSeconds(0.09f);

        isFiring = false;
    }


    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        //Debug.Log($"{Time.time} OnFireChanged value {changed.Behaviour.isFiring}");

        bool isFiringCurrent = changed.Behaviour.isFiring;

        //Load the old value
        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
            changed.Behaviour.OnFireRemote();

    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
            fireParticleSystem.Play();
    }
}
