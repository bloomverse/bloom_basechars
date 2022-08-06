using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int current;
    public Image mask;
    public Image fill;
    public Color color;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill(){
        float fillAmount = (float)current / (float)maximum;
        Debug.Log(current + " - " + fillAmount);
        fill.fillAmount = fillAmount;
        fill.color = color;

    }
}
