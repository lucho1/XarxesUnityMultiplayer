using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListButtonOnClick : MonoBehaviour
{
    public void SetSelectedRoom(Text text)
    {
        GameObject rooms_list = GameObject.Find("RoomList");
        if (rooms_list)
            rooms_list.GetComponent<RoomListController>().SetSelectedRoom(text);
        else
            Debug.LogError("Couldn't Find Rooms List!");
    }
}
