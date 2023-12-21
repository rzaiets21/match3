using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasNewestDialog : MonoBehaviour
{
    public void Show()
    {
        GGUtil.SetActive(gameObject, true);
    }

    public void Hide()
    {
        GGUtil.SetActive(gameObject, false);
    }
}
