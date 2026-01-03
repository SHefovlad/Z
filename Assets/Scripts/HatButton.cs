using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image icon;
    public Hat hat;
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        GetComponent<Image>().color = Color.white;
        FindAnyObjectByType<PlayerController>().CurrentHat = hat;
    }
}
