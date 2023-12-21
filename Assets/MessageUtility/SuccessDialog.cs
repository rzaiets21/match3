using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuccessDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        
    private void OnEnable()
    {
        GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
    }
    
    public void Show(string text)
    {
        _textMeshProUGUI.text = text;
        GGUtil.SetActive(gameObject, true);
    }
    
    public void Hide()
    {
        GGUtil.SetActive(gameObject, false);
    }
}
