using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    
    public Label bloomies;
    public static UIController instance;

    void Start()
    {
        instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        bloomies = root.Q<Label>("bloomies");

      //  var Host_bt =GetComponent<UIDocument>().rootVisualElement.Q<Button>("Host");
      //  var Client_bt =GetComponent<UIDocument>().rootVisualElement.Q<Button>("Client");

      /*  Host_bt.clicked += () =>
        {
            Debug.Log("Host Click");
            //NetworkHandler.instance.StartHostMode();
        };


        Client_bt.clicked += () =>
        {
            //NetworkHandler.instance.StartClientMode();
            Debug.Log("Client Click");
        };*/


    }

    public void setRocketGasolineValue(float value){
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<AbstractProgressBar>("rocketgasoline").value = value;
    }   

    // Update is called once per frame
    void Update()
    {
        //bloomies.text = "";
    }
}
