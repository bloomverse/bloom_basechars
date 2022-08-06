using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class logoStore : MonoBehaviour, IActivable
{
     [SerializeField] ActivableData _data;
     private bool active;
     private float timeActive= 2f;
     private float timeAct;

     [SerializeField] private GameObject logo;
     [SerializeField] private GameObject backEffect;

    // Start is called before the first frame update
    void Start()
    {
        logo.transform.DOScale(0,0);
        backEffect.transform.DOScale(0,0);
        logo.SetActive(false);
        backEffect.SetActive(false);
      
    }

    public void Update(){
        if(active){
            timeAct -= Time.deltaTime ;
            if(timeAct<=0){
                Debug.Log("deactivantdo");
                active = false;
                Deactivate();
                
            }
        }
    }

    public void Activate(){

        if(!active){
            timeAct = timeActive;
            active= true;
            logo.SetActive(true);
            backEffect.SetActive(true);
            logo.transform.DOScale(1,1).SetEase(Ease.OutBack);
            backEffect.transform.DOScale(1,1).SetEase(Ease.OutBack);
            
            
        }else{
            timeAct = timeActive;
        }
        
        
    }

    public void Deactivate(){
        if(!active){
            logo.transform.DOScale(0,1).SetEase(Ease.InBounce);
        backEffect.transform.DOScale(0,1).SetEase(Ease.InBounce).OnComplete(()=>{
            logo.SetActive(false);
            backEffect.SetActive(false);
        });
        }
       
    }

}
