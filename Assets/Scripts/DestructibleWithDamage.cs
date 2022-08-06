using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWithDamage : MonoBehaviour, IEffectable
{
    private int _maxHealth = 400;
    public GameObject destroyedVersion;

    [SerializeField] private int currentHealth;
    private StatusEffectData _data;
    public bool live = true;

    private GameObject _effectParticles;
    private float _currentEffectTime = 0f;
    private float _nextTickTime = 0f;

    void Start()
    {
         currentHealth = _maxHealth;
    }

     
     public void ApplyEffect(StatusEffectData _data)
    {
         RemoveEffect();
        this._data = _data; 
       _effectParticles = Instantiate(_data.EffectParticles,  _data.hitpoint.point +  _data.hitpoint.normal*0.1f, Quaternion.LookRotation(_data.hitpoint.normal) );
        
       

    }

    public void destroyObj(){
            Debug.Log("Destroyed version");
            live = false;
            var dversion = Instantiate(destroyedVersion,transform.position, transform.rotation);
            dversion.transform.localScale = transform.localScale;

            Destroy(gameObject);
    }
    
     public void RemoveEffect() 
    {
        _data = null;
        _currentEffectTime = 0;
        _nextTickTime = 0;
       
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

             currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);    
        
           if(currentHealth<=0){
              
              
              destroyObj();
           }
        }
    }

    // Update is called once per frame
    void Update()
    {
         if (_data != null) HandleEffect();

    }
}
