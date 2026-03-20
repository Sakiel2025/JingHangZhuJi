// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/RulerRotate.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

// UI Image专属：滑块/鼠标拖拽旋转木头（适配UI元素）
public class RulerRotate : MonoBehaviour, IDragHandler
{
    [Header("核心赋值（必须填！）")]
    public Image newRafterImage; // 木头的Image组件（直接赋值Image，不是Transform）
    public Slider angleSlider;   // 滑块组件
    public TextMeshProUGUI angleTip; // 角度提示
    public Button confirmBtn;    // 确认按钮
    public RepairGameManager gameManager;

    [Header("角度配置（新手友好）")]
    public float targetAngle = 30f; // 目标角度
    public float angleTolerance = 5f; // 误差范围
    public float rotateSpeed = 2f; // 旋转速度（调大更易感知）

    void Start()
    {
        // 1. 滑块初始化（UI适配：0→0°，1→180°）
        if (angleSlider != null)
        {
            angleSlider.minValue = 0;
            angleSlider.maxValue = 1;
            angleSlider.value = 0.5f; // 初始角度90°
            angleSlider.onValueChanged.AddListener(UpdateUIImageAngleBySlider);
        }

        // 2. 初始化按钮和木头角度
        confirmBtn.interactable = false;
        UpdateUIImageAngleBySlider(0.5f); // 强制初始角度同步
    }

    void Update()
    {
        // 角度检测（适配UI的localEulerAngles）
        float currentAngle = Mathf.Round(newRafterImage.rectTransform.localEulerAngles.z);
        angleTip.text = $"当前角度：{currentAngle}° 目标角度：{targetAngle}°";
        confirmBtn.interactable = Mathf.Abs(currentAngle - targetAngle) < angleTolerance;
    }

    // ========== 滑块控制UI Image（核心：适配UI的rectTransform） ==========
    private void UpdateUIImageAngleBySlider(float sliderValue)
    {
        // 滑块值→UI旋转角度：0=0°，0.5=90°，1=180°（UI用localEulerAngles）
        float targetAngle = sliderValue * 180f;
        newRafterImage.rectTransform.localEulerAngles = new Vector3(0, 0, targetAngle);
    }

    // ========== 鼠标拖拽控制UI Image（适配UI元素） ==========
    public void OnDrag(PointerEventData eventData)
    {
        // 鼠标X轴移动 → UI Z轴旋转（适配UI的localRotation）
        float deltaAngle = -eventData.delta.x * rotateSpeed;
        newRafterImage.rectTransform.Rotate(0, 0, deltaAngle);
        
        // 同步滑块值（UI角度→滑块值）
        if (angleSlider != null)
        {
            float currentAngle = newRafterImage.rectTransform.localEulerAngles.z;
            float sliderValue = currentAngle / 180f;
            sliderValue = Mathf.Clamp01(sliderValue); // 限制在0~1
            angleSlider.SetValueWithoutNotify(sliderValue);
        }
    }

    // ========== 确认按钮 ==========
    public void OnConfirmClick()
    {
        gameManager.OnStep2Success();
    }
}