using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRocket : MonoBehaviour
{

    public float delay = 1.2f;
    public float force = 20f;
    private float _moveSpeed = 30f;
    private int rocketNumber = 6;


    float countdown;
    bool hasExploded = false;
    public GameObject explosionEfffect;
    
    public GameObject[] enemies;

    Rigidbody m_Rigidbody;

    [SerializeField] private GameObject homeingRocket;

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


    private float strayFactor = 20f;

    void Explode(){

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        

      //  Instantiate(explosionEfffect,transform.position,transform.rotation);

        for (int i = 0; i < rocketNumber; i++)
        {   
            var randomNumberX = Random.Range(-strayFactor, strayFactor);
            var randomNumberY = Random.Range(-strayFactor, strayFactor);
            var randomNumberZ = Random.Range(-strayFactor, strayFactor);
            GameObject missile = (GameObject)Instantiate(homeingRocket, transform.position, transform.rotation);
            var misScript = missile.GetComponent<Missile>();
            misScript._target = enemies[Random.Range(0,enemies.Length)];
            
            missile.transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
             
        }

       

        
        Destroy(gameObject);
    }
}
