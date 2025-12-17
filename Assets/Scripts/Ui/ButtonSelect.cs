using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    public GameObject selectUi;

    public void On()
    {
        selectUi.SetActive(true);
    }

    public void Off()
    {
        selectUi.SetActive(false);
    }
}
