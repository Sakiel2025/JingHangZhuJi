using UnityEngine;

public class PuzzlePiece_dougong : MonoBehaviour
{
    public bool isComplete;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    public Transform targetPosition;
    public float snapThreshold = 0.5f;
    public bool isPlayerPiece; // 玩家碎片勾上，AI碎片不勾

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        if (isComplete || !isPlayerPiece) return;
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseUp()
    {
        if (isComplete || !isPlayerPiece) return;
        isDragging = false;
        CheckPosition();
    }

    void Update()
    {
        if (isDragging && isPlayerPiece)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePoint);
    }

    private void CheckPosition()
    {
        if (Vector3.Distance(transform.position, targetPosition.position) < snapThreshold)
        {
            transform.position = targetPosition.position;
            isComplete = true;

            if (isPlayerPiece)
                PuzzleMG_ai_version.instance.Check();
        }
    }

    // 给AI用的移动
    public void MoveToTarget(float speed)
    {
        if (isComplete) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition.position) < snapThreshold)
        {
            transform.position = targetPosition.position;
            isComplete = true;
        }
    }
}