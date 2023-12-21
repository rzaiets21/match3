using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDialog : MonoBehaviour
{
    public Transform loading;
    public float rotateSpeed = 5f;
    private float angle = 0;

    private void OnEnable()
    {
        GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
    }
    
    public void Show()
    {
        GGUtil.SetActive(gameObject, true);
    }
    public void Hide()
    {
        GGUtil.SetActive(gameObject, false);
    }
    
    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * rotateSpeed;
        loading.Rotate(Vector3.forward, angle);
    }
}
