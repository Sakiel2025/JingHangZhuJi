// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/RulerRotate.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class RulerRotate : MonoBehaviour, IDragHandler
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
    public float rotateSpeed = 2f;

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

        // 【修复】最稳定的文本显示，绝对能看见 ° 符号
        if (angleTip != null)
        {
            angleTip.text = "当前角度：" + currentAngle + "°目标角度：" + targetAngle + "°";
        }

        // 角度判断
        if (Mathf.Abs(currentAngle - targetAngle) < angleTolerance)
            confirmBtn.interactable = true;
        else
            confirmBtn.interactable = false;
    }

    void UpdateWoodAngle(float sliderValue)
    {
        float finalAngle = (sliderValue - 0.5f) * 180f;
        finalAngle = Mathf.Clamp(finalAngle, -90, 90);
        newRafterImage.rectTransform.localEulerAngles = new Vector3(0, 0, finalAngle);
    }

    public void OnDrag(PointerEventData data)
    {
        newRafterImage.rectTransform.Rotate(0, 0, -data.delta.x * rotateSpeed);

        if (angleSlider != null)
        {
            float angle = NormalizeAngle(newRafterImage.rectTransform.localEulerAngles.z);
            float val = (angle + 90) / 180f;
            val = Mathf.Clamp01(val);
            angleSlider.SetValueWithoutNotify(val);
        }
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