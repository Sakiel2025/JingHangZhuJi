// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/RulerRotate.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

// 新手简化版：鲁班尺拖拽旋转新椽
public class RulerRotate : MonoBehaviour, IDragHandler
{
    public Transform newRafter; // 新椽Image的Transform
    public TextMeshProUGUI angleTip; // 角度提示文本
    public Button confirmBtn; // 确认校准按钮
    public RepairGameManager gameManager;
    private float targetAngle = 30f; // 目标角度（新手可改小，比如15°）
    private float angleTolerance = 5f; // 误差范围（放大，新手更容易成功）

    void Update()
    {
        // 显示当前角度（只取Z轴旋转）
        float currentAngle = Mathf.Round(newRafter.eulerAngles.z); // 四舍五入，显示更友好
        angleTip.text = $"当前角度：{currentAngle}° 目标角度：{targetAngle}°";
        
        // 角度达标，启用确认按钮
        if (Mathf.Abs(currentAngle - targetAngle) < angleTolerance)
        {
            confirmBtn.interactable = true;
        }
        else
        {
            confirmBtn.interactable = false;
        }
    }

    // 拖拽鲁班尺旋转新椽（新手简化：鼠标左右移=旋转）
    public void OnDrag(PointerEventData eventData)
    {
        float rotateSpeed = 0.3f; // 降低旋转速度，更易控制
        newRafter.Rotate(0, 0, -eventData.delta.x * rotateSpeed);
    }

    // 确认校准（绑定到按钮）
    public void OnConfirmClick()
    {
        gameManager.OnStep2Success(); // 原第三步改为第二步
    }
}