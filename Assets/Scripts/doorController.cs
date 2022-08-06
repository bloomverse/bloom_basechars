using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    [SerializeField] private float cooldown = 2f;
     public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    private string openStatus = "idle";
    private bool opening = false;
    
    public Transform start;
    public Transform end;
    float t;
    float coolt;
    public float duration = 1.5f;
   


    void OnTriggerStay(Collider other) {
        if(other.gameObject.tag=="Player"){
            StartInteraction();
        }
         
    }

    public void StartInteraction(){
        if(openStatus=="idle"){
            t = 0.0f;
            openStatus = "openning";
        }
    }
 

   void Update(){

        if(openStatus=="openning"){
          
            t += Time.deltaTime;
            float s = t / duration;
            
            transform.position = Vector3.Lerp(start.position, end.position, curve.Evaluate(s));
            if(transform.position==end.position){
                coolt = 0.0f;
                openStatus = "opened";
                StartCoroutine(waitingTime());
            }
        }

        if(openStatus=="closing"){
           t += Time.deltaTime;
            float s = t / duration;
            
            transform.position = Vector3.Lerp(end.position, start.position, curve.Evaluate(s));
            if(transform.position==start.position){
                openStatus = "idle";
            }
        }

        
    }

    IEnumerator waitingTime(){
        
    yield return  new WaitForSeconds(3);
    t = 0.0f;
    openStatus = "closing";

}

 
}

