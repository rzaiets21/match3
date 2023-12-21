using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorText;

    private void OnEnable()
    {
        GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
    }
    
    public void Show(string text)
    {
        errorText.text = text;
        
        GGUtil.SetActive(gameObject, true);
    }

    public void Hide()
    {
        GGUtil.SetActive(gameObject, false);
    }
}
