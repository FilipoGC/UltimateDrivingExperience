using UnityEngine;
using TMPro;

public class PositionHUD : MonoBehaviour
{
    public Transform playerTransform;
    public TextMeshProUGUI positionText;

    void Update()
    {
        Vector3 pos = playerTransform.position;
        positionText.text = $"Position: X={pos.x:F2}  Y={pos.y:F2}  Z={pos.z:F2}";
    }
}
