using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEffectable
{
    private int _maxHealth = 1000;
    [SerializeField] private int currentHealth;
    
    private StatusEffectData _data;

    private float _moveSpeed = 2f;
    private float _currentMoveSpeed;

    private bool live = true;

    private Animator animator;

    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    public Transform playerEnemy ;

    //damage popup
    [SerializeField] private Transform  pfDamagePopup;
    [SerializeField] private Transform  pfDamageSpawn;
    [SerializeField] private Slider  HealthBar;
    [SerializeField] private GameObject  HealthBarCanvas;

    [SerializeField] private ParticleSystem deadthParticles;
    [SerializeField] private Transform deadthParticlesSpawn;

   

    //[SerializeField] private Rigidbody _rb;
    // [SerializeField] private float _size = 10;
    //[SerializeField] private float _speed = 10;
    //public Rigidbody Rb => _rb;
    //[SerializeField] private ProgressBar  HealthBar;
   
    

    private void Start()
    {
         //setRigidBodyState(true);
         //setColliderState(false);

        agent = GetComponent<NavMeshAgent>();
    
        _currentMoveSpeed = _moveSpeed;
        currentHealth = _maxHealth;
        HealthBar.maxValue = _maxHealth;
        HealthBar.value = _maxHealth;
        animator = GetComponent<Animator>();
       // HealthBar.maximum = _maxHealth;

    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

    }

    // Update is called once per frame
    void Update()
    {
        if (_data != null && live) HandleEffect();

        float distance = Vector3.Distance(playerEnemy.position,transform.position);
        if(distance <= lookRadius && live){
            agent.SetDestination(playerEnemy.position);
            
             animator.SetFloat("MoveZ", 1f);
            if(distance <= agent.stoppingDistance){
                FaceTarget();
                 
            }
        }
    }

    void FaceTarget(){
        Vector3 direction = (playerEnemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime * 5f);
    }


    private GameObject _effectParticles;
    public void ApplyEffect(StatusEffectData _data)
    {
        
        RemoveEffect();
        this._data = _data;
        if (_data.MovementPenalty > 0) _currentMoveSpeed = _moveSpeed / _data.MovementPenalty;
//        Debug.Log(_data.hitpoint);
       _effectParticles = Instantiate(_data.EffectParticles,  _data.hitpoint.point +  _data.hitpoint.normal*0.1f, Quaternion.LookRotation(_data.hitpoint.normal) );
    }

    private float _currentEffectTime = 0f;
    private float _nextTickTime = 0f;
    public void RemoveEffect() 
    {
        _data = null;
        _currentEffectTime = 0;
        _nextTickTime = 0;
        _currentMoveSpeed = _moveSpeed;
        if (_effectParticles != null) Destroy(_effectParticles);
    }

    
    public void HandleEffect()
    {
        _currentEffectTime += Time.deltaTime;
        
        if (_currentEffectTime >= _data.Lifetime) RemoveEffect();

        if (_data == null) return;
        
        if (_data.DOTAmount != 0 && _currentEffectTime > _nextTickTime)
        {
            _nextTickTime += _data.TickSpeed;
            currentHealth -= _data.DOTAmount;
            bool isCriticalHit = Random.Range(0,100) < 30;
            damagePop(_data.DOTAmount,isCriticalHit);

            // Damage Animation
            int damageAnimation = Animator.StringToHash("KidneyHit");
            animator.CrossFade(damageAnimation,0.15f);


            currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);

           HealthBar.value = currentHealth;
           
           if(currentHealth<=0){
               live = false;
               die();
               ArenaManager.instance.StartCoroutine(ArenaManager.instance.SlowMotionSequence());
               //HealthBar.setEnabled(false);
               
               //Instantiate(deadthParticles, deadthParticlesSpawn.position,deadthParticlesSpawn.rotation);
               //Destroy(gameObject);
           }
        }
    }

    void die(){
        //
        int dieAnimation = Animator.StringToHash("Die");
        animator.CrossFade(dieAnimation,0.15f);
        HealthBarCanvas.SetActive(false);
        agent.velocity = Vector3.zero;
        //var capsuleCollider = GetComponent<CapsuleCollider>(); 

        //capsuleCollider.height = .2f;
        //capsuleCollider.radius = 0.2f;
        //setRigidBodyState(false);
        //setColliderState(true);
        //GetComponent<Animator>().enabled = false;
        
    }

    // RAGDOLL STUFF
    void setRigidBodyState(bool state){
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rigidbody in rigidbodies){
            rigidbody.isKinematic = state;
        }
        GetComponent<Rigidbody>().isKinematic = !state;
    }
    void setColliderState(bool state){
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders){
            collider.enabled = state;
        }
    }

    private void damagePop(int damage, bool isCriticalHit){
        //var look = _data.hitpoint.normal*-1;
        //look.y = 0.01f;

       // var brain = CinemachineCore.Instance.GetActiveBrain(0);
        //var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        
    //+  _data.hitpoint.normal*0.1f
        Transform damagePopupTransform = Instantiate(pfDamagePopup, pfDamageSpawn.position , Camera.main.transform.rotation);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage,isCriticalHit);
    }

 
}
