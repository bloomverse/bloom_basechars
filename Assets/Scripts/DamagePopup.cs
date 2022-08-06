using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;
using DG.Tweening;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float dissapearTimer;
    private Color textColor;
    private Vector3 moveVector;
    private int sortingOrder;


    private void Awake(){
        textMesh = transform.GetComponent<TextMeshPro>();
    }

     float defaultDistance =3f;
     float defaultSize = 1f;


   public void Setup(int damageAmount, bool isCriticalHit){

       float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
       Debug.Log(dist);
        textMesh.fontSize = 5f; //Mathf.RoundToInt(defaultSize * (dist/10));

       textMesh.SetText(damageAmount.ToString());
       if(!isCriticalHit){
           //normal
           //textMesh.fontSize = 6;
           textColor = UtilsClass.GetColorFromString("FFC500");
       }else{
           //Critical
          // textMesh.fontSize = 10;
           textColor = UtilsClass.GetColorFromString("FF2B00");
       }
       
       textMesh.color = textColor;
    
       
       int endY = Random.Range(-2,2);
       float endX = Random.Range(-2f,2f);
       moveVector = new Vector3(textMesh.transform.position.x+ endX ,textMesh.transform.position.y + endY,textMesh.transform.position.z ) ;
       sortingOrder++;
       textMesh.sortingOrder = sortingOrder;

      textMesh.transform.DOMove(moveVector,1.8f).SetEase(Ease.OutBack).OnComplete(()=>{
            Destroy(gameObject);
       });

       StartCoroutine(FadeOutText(1f,textMesh));


      


   

   }

    

    private IEnumerator FadeOutText(float timeSpeed, TextMeshPro text)
    {
        yield return new WaitForSeconds(.7f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

/*
   private void Update(){
       float moveYSpeed = 3f;
       transform.position += moveVector * Time.deltaTime;
       moveVector -= moveVector * 8f * Time.deltaTime;

       if(dissapearTimer > DISSAPEAR_TIMER_MAX * .5f){
           float increaseScaleAmount = .3f;
           transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
       }else{
           float increaseScaleAmount = 0.1f;
           transform.localScale -= Vector3.one * increaseScaleAmount * Time.deltaTime;
       }

       dissapearTimer -= Time.deltaTime;
       if(dissapearTimer < 0){
           float dissapearSpeed = 3f;
           textColor.a -= dissapearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0){
                Destroy(gameObject);
            }
       }
   }
   */

}
