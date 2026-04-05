// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/RulerRotate.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RulerRotate : MonoBehaviour
{
    [Header("核心赋值")]
    public Image newRafterImage;
    public Slider angleSlider;
    public TextMeshProUGUI angleTip;
    public Button confirmBtn;
    public RepairGameManager gameManager;

    [Header("角度设置")]
    public float targetAngle = 30f;
    public float angleTolerance = 5f;

    void Start()
    {
        if (angleSlider != null)
        {
            angleSlider.minValue = 0;
            angleSlider.maxValue = 1;
            angleSlider.value = 0.5f;
            angleSlider.onValueChanged.AddListener(UpdateWoodAngle);
        }

        confirmBtn.interactable = false;
        UpdateWoodAngle(0.5f);
    }

    void Update()
    {
        float rawAngle = newRafterImage.rectTransform.localEulerAngles.z;
        float currentAngle = NormalizeAngle(rawAngle);
        currentAngle = Mathf.Round(currentAngle);

        if (angleTip != null)
        {
            angleTip.text = "当前角度：" + currentAngle + "°  目标角度：" + targetAngle + "°";
        }

        if (Mathf.Abs(currentAngle - targetAngle) < angleTolerance)
            confirmBtn.interactable = true;
        else
            confirmBtn.interactable = false;
    }

    void UpdateWoodAngle(float sliderValue)
    {
        // 滑块 0~1 → 角度 -90 ~ +90
        float finalAngle = (sliderValue - 0.5f) * 180f;
        finalAngle = Mathf.Clamp(finalAngle, -90, 90);
        newRafterImage.rectTransform.localEulerAngles = new Vector3(0, 0, finalAngle);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        return Mathf.Clamp(angle, -90, 90);
    }

    public void OnConfirmClick()
    {
        gameManager.OnStep2Success();
    }
}