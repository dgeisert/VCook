using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class StartButton : MonoBehaviour
{

    public string sceneToLoad = "Lobby";
    private VRTK_Button_UnityEvents buttonEvents;

    private void Start()
    {
        buttonEvents = GetComponent<VRTK_Button_UnityEvents>();
        if (buttonEvents == null)
        {
            buttonEvents = gameObject.AddComponent<VRTK_Button_UnityEvents>();
        }
        buttonEvents.OnPushed.AddListener(handlePush);
    }

    private void handlePush(object sender, Control3DEventArgs e)
    {
        StartCoroutine("SetScene");
    }

    IEnumerator SetScene()
    {
        yield return null;
        NetworkManager.instance.SendSceneChange(sceneToLoad);
    }
}
