
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using Cinemachine;
using TMPro;

public class ArenaManager : MonoBehaviour
{

    public static ArenaManager instance;
    public  CinemachineVirtualCamera virtualCamera;

    [SerializeField] Color original;
    [SerializeField] Color green;
    [SerializeField] Color red;
    [SerializeField] Color white;

    public AudioClip counterTick;
    public AudioSource source;


    new Renderer renderer;
    public Material material;

     [SerializeField] private Transform electricDoor;

    void Awake(){
        instance = this;
        //virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    
    void Start()
    {
        material.DOColor(original * 7, "_EmissionColor", 0f);
        StartCoroutine(CountDownStart());
    }


   [SerializeField] private int countdownTime;
   [SerializeField] private TextMeshPro countdownDisplay;

   IEnumerator CountDownStart(){
       while(countdownTime > 0){
           countdownDisplay.SetText(countdownTime.ToString());
           yield return new WaitForSeconds(1f);

           countdownTime --;
           if(countdownTime<4){
               source.PlayOneShot(counterTick);
           }
       }

       countdownDisplay.text = "GO!";

       startSequence();

       yield return new WaitForSeconds(1f);

       countdownDisplay.gameObject.SetActive(false);
   }

    void startSequence(){

        Sequence mySequence = DOTween.Sequence(); 
        electricDoor.DOMoveY(5,3).SetEase(Ease.InOutBack).SetDelay(0);
        mySequence.Append(material.DOColor(green * 17, "_EmissionColor", 1f).SetLoops(6,LoopType.Yoyo));
       // mySequence.Append(material.DOColor(green * 6, "_Color", 1f).SetLoops(9,LoopType.Yoyo));
       // mySequence.Append(material.DOColor(original * 4, "_EmissionColor", 1f));
        mySequence.PrependInterval(0);
        //mySequence.Init(1);
        
     
    }

    public void winner(){

    }

    // Update is called once per frame
     void Update()
    {
      

    }

    float m_Timer;
    float m_SlowMOtionScale = 0.2f;
    bool m_TimerRunning = false;

    public IEnumerator SlowMotionSequence(){
      
      virtualCamera.Priority += 10;
      yield return new WaitForSecondsRealtime(.2f);
      Time.timeScale = m_SlowMOtionScale;
      yield return new WaitForSecondsRealtime(3);
      Time.timeScale = 1;
      virtualCamera.Priority -= 10;
    }
}




  // DOTween.To(() =>0, (u) => BodyRecoil.weight = u,1f,.1f).OnComplete(() =>{
		//			DOTween.To(() => 1 , (u) => BodyRecoil.weight = u ,0f,.1f);
		//	});
        //DoTween.()

        //Objs[0].GetComponent<MeshRenderer>().materials[1].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        //Objs[0].GetComponent<MeshRenderer>().materials[1].EnableKeyword("_EMISSION");
       // Debug.Log(Objs[0].GetComponent<MeshRenderer>().materials[1].enabledKeywords);

        //foreach (var localKeyword in Objs[0].GetComponent<MeshRenderer>().materials[1].enabledKeywords)
        //{
            //Debug.Log("Local shader keyword " + localKeyword.name + " is currently enabled");
       // }


        //Objs[0].GetComponent<MeshRenderer>().materials[1].DOColor(Color.blue * 2, "_EmissionColor", 2f).SetLoops(50,LoopType.Yoyo);
        /* for (int i = 0; i < Objs.Length; i++)
        {
            var mesh = Objs[i].GetComponent<MeshRenderer>();
           mesh.materials[1].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
           mesh.materials[1].EnableKeyword("_EMISSION");
            Meshes.Add(mesh);
        }
        len = myColors.Length;*/



          /*
       foreach(MeshRenderer currMesh in Meshes)
       {

           currMesh.materials[1].SetColor("_Color",Color.Lerp(currMesh.materials[1].GetColor("_EmissionColor"), myColors[colorIndex],lerpTime*Time.deltaTime)) ;
    
           t = Mathf.Lerp (t, 1f, lerpTime*Time.deltaTime);

           if(t > .9f){
               t = 0f;
               colorIndex++;
               colorIndex = (colorIndex >= len) ? 0 : colorIndex;
           }

       }
       */