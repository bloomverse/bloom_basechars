using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using TMPro;



public class ChestControls : MonoBehaviour
{   

    private PlayerInput playerInput;
    private Animator _animator;
    int OpenAnimationId;
    [SerializeField]
    private ParticleSystem particlesOpen;
    private InputAction openAction;
    private bool _hasAnimator;

    private  bool statusChest;

    [SerializeField] private TextMeshPro textInfo;
    [SerializeField] private string text;

    [SerializeField] private GameObject bloomie;
    [SerializeField] private GameObject bases;

    [SerializeField] Color original;
    [SerializeField] Color purple;
    [SerializeField] Color blue;

    public Material material;
    

     private int gain = 500;


     // Shake timer variables
		 private float shakeTimer;
		 private float shakeTimerTotal;
		 private float startingIntensity;
		 private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    // Start is called before the first frame update
    void Awake()
    {
        
        playerInput = GetComponent<PlayerInput>();
        openAction = playerInput.actions["ChestKey"];
        
    }

    void Start(){
        _animator = GetComponent<Animator>();
        OpenAnimationId= Animator.StringToHash("Open");
         material.DOColor(original * 7, "_EmissionColor", 0f);



         var brain = CinemachineCore.Instance.GetActiveBrain(0);
 			var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();


			cinemachineBasicMultiChannelPerlin = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = .1f;
			
        
    }

    private void OnEnable(){
			
			openAction.performed += _ => HandleOpenStart();
			//openAction.canceled += _ => CancelOpen();  
    }

    private void HandleOpenStart(){
        Debug.Log("F key pressed");
    
          statusChest = !statusChest;
          _animator.SetBool("Open", statusChest);

          if(statusChest){

              

            transform.DOShakePosition(1f,.3f,20,30f).OnComplete(()=>{
              shakeCamera(0.5f,2f);
              particlesOpen.Play();
              setPrize();
            });

              
          }

      
        
    }

     private void setPrize(){
         DOTween.To(()=>0,(u) => textInfo.SetText("+ " + u) ,gain,5).SetEase(Ease.OutSine).SetDelay(2).OnComplete(()=>{
        });
        
         StartCoroutine(FadeInText(1f,textInfo));

        textInfo.transform.DOMoveY(0,1).SetEase(Ease.OutSine).SetDelay(1.4f);
        bloomie.transform.DOMoveY(-0.43f,1).SetEase(Ease.OutSine).SetDelay(1.4f);
        bases.transform.DOMoveZ(5.6f,1).SetEase(Ease.OutSine).SetDelay(1.4f);
        material.DOColor(blue * 18, "_EmissionColor", 2f);//.SetLoops(6,LoopType.Yoyo));
    }

      private IEnumerator FadeInText(float timeSpeed, TextMeshPro text)
    {
        //yield return new WaitForSeconds(.7f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
            yield return null;
        }
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
    
    private void shakeCamera(float intensity, float time){

			var brain = CinemachineCore.Instance.GetActiveBrain(0);
 			var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();


			cinemachineBasicMultiChannelPerlin = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
			cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 23f;
			startingIntensity = intensity;
			shakeTimerTotal = time;
			shakeTimer = time;
		}

    // Update is called once per frame
    void Update()
    {
        // Camera shake smoother
			if(shakeTimer > 0){
				shakeTimer -= Time.deltaTime;
				cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity,0f, 1 - (shakeTimer / shakeTimerTotal));
				
			} else{

					try{
						cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 1;
                        
					}catch{

					}

				
			}

    }
}
