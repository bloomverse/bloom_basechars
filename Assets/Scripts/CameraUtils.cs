using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraUtils : MonoBehaviour
{

     // Shake timer variables
		 private float shakeTimer;
		 private float shakeTimerTotal;
		 private float startingIntensity;
		 private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

        public static CameraUtils instance;
    // Start is called before the first frame update
  void Awake(){
        instance = this;
      
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void shakeCamera(float intensity, float time){

			var brain = CinemachineCore.Instance.GetActiveBrain(0);
 			var Vcam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

			if(Vcam!=null){
			cinemachineBasicMultiChannelPerlin = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
			cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 23f;
			startingIntensity = intensity;
			shakeTimerTotal = time;
			shakeTimer = time;
			} 

            Debug.Log(Vcam + "Vcam");

            //StartCoroutine(Shaky());
		}

    IEnumerator Shaky(){
        // Camera shake smoother

         while (shakeTimer > 0)
        {
            Debug.Log(shakeTimer + "Shake timer");
            shakeTimer -= Time.deltaTime;
			cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity,.5f, 0.5f - (shakeTimer / shakeTimerTotal));

            yield return null;
        }


        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = .5f;

		

    }

    
}
