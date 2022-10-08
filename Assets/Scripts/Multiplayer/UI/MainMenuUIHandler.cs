using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;


public class MainMenuUIHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public UIDocument UI_pre;

    // Start is called before the first frame update
    void Start()
    {
         var root = GetComponent<UIDocument>().rootVisualElement;
        var  start = root.Q<Button>("p1");
        start.RegisterCallback<ClickEvent>(OnJoinGameClicked);


        var  start2 = root.Q<Button>("p2");
        start2.RegisterCallback<ClickEvent>(OnJoinGameClickedSolana);


        if (PlayerPrefs.HasKey("PlayerNickname"))
            inputField.text = PlayerPrefs.GetString("PlayerNickname");


    }

    public void OnJoinGameClicked(ClickEvent evt)
    {
        PlayerPrefs.SetString("PlayerNickname", inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Arena");
        gameObject.SetActive(false);
    }

     public void OnJoinGameClickedSolana(ClickEvent evt)
    {
        PlayerPrefs.SetString("PlayerNickname", inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Solana");
        gameObject.SetActive(false);
    }

}
