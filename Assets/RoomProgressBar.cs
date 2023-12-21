using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressTextLabel;
    [SerializeField] private Image progressBar;
    
    public void InitProgressbar(float progress)
    {
        GGUtil.ChangeText(progressTextLabel, $"{GGFormat.FormatPercent(progress)}%");
        progressBar.fillAmount = progress;
    }
}
