using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeselectButton : MonoBehaviour, IDeselectHandler
{
    [SerializeField]
    private GameObject m_KickButton;

    public void OnDeselect(BaseEventData data)
    {
        if(!m_KickButton.GetComponent<ButtonHoverHandler>().Hovering)
            m_KickButton.SetActive(false);
    }
}
