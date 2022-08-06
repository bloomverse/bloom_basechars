using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Spell : MonoBehaviour
{
    [SerializeField] StatusEffectData _data;
    
    private float _moveSpeed = 5f;

    private SphereCollider _sphereCol;
    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;

    [SerializeField] private float CooldownDelay = 1f;
        private float CooldownTime;

    private void Awake()
    {
        _sphereCol = GetComponent<SphereCollider>();
        _sphereCol.isTrigger = true;
        _sphereCol.radius = 1f;

        m_Rigidbody = GetComponent<Rigidbody>();

        
       
    }

    // Update is called once per frame

     private void Start() {
         Debug.Log(_data.moveSpeed);
          m_Rigidbody.velocity = _data.moveSpeed * transform.forward;
    }

    void Update()
    {
        //m_Rigidbody.AddForce(transform.up * m_Thrust);
        //transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        

        

        var effectable = other.GetComponent<IEffectable>();

        if (effectable != null)
        {


            //Doing RAYCAST TO GET hitpoint 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            _data.hitpoint = hit;
            effectable.ApplyEffect(_data);
        }
          
           
        }

        Destroy(this.gameObject);
    }
}
