using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListButtonOnClick : MonoBehaviour
{
    public GameObject LobbyObject = null;
    public void SetSelectedRoom(Text text)
    {
        if(LobbyObject)
            LobbyObject.GetComponent<LobbyScript>().SetCurrentSelectedRoom(text.text);
    }
}
