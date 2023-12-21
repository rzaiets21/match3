using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmailDialog : MonoBehaviour
{
    [SerializeField] private ErrorDialog errorDialog;
    [SerializeField] private TMP_InputField inputField;

    private void OnEnable()
    {
        inputField.text = string.Empty;
        GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
    }

    public void Hide()
    {
        GGUtil.SetActive(gameObject, false);
    }
    
    public void Show()
    {
        GGUtil.SetActive(gameObject, true);
    }

    public void ButtonClick_Ok()
    {
        var text = inputField.text;
        var email = text.Split('@');
        if (email.Length < 2)
        {
            Error("Incorrect Email");
            return;
        }

        var domain = email[1].Split('.');

        if (domain.Length < 2)
        {
            Error("Incorrect Email");
            return;
        }
        
        MessageUtility.instance.action?.Invoke(text);
        Hide();
    }

    private void Error(string text)
    {
        errorDialog.Show(text);
    }
}
