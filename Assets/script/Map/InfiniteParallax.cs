using UnityEngine;

public class InfiniteScroller2Segments : MonoBehaviour
{
    [Header("Refs")]
    public Transform cam;          // Main Camera (transform)
    public Transform segmentA;     // Prefab/segment 1 (đã đặt trong scene)
    public Transform segmentB;     // Prefab/segment 2 (đã đặt liền kề)

    [Header("Length & Trigger")]
    [Tooltip("Để 0 = tự tính theo bounds sprite trong segmentA")]
    public float segmentLength = 0f;   // chiều dài 1 segment (world units)
    [Tooltip("Dịch sớm/ trễ bao nhiêu đơn vị trước khi chạm mép")]
    public float lead = 2f;

    private Transform leftSeg, rightSeg;

    void Awake()
    {
        if (!cam || !segmentA || !segmentB)
        {
            Debug.LogError("[InfiniteScroller2Segments] Thiếu cam/segmentA/segmentB");
            enabled = false; return;
        }

        // Tự tính chiều dài nếu chưa set
        if (segmentLength <= 0f)
            segmentLength = ComputeSegmentLength(segmentA);

        // Xác định trái/phải ban đầu
        if (segmentA.position.x <= segmentB.position.x)
        {
            leftSeg = segmentA; rightSeg = segmentB;
        }
        else
        {
            leftSeg = segmentB; rightSeg = segmentA;
        }
    }

    void LateUpdate()
    {
        float camX = cam.position.x;

        // Đi sang phải: nếu camera sắp qua nửa phải của segmentRight -> đẩy segmentLeft lên tiếp theo
        if (camX > rightSeg.position.x - (segmentLength * 0.5f) - lead)
        {
            MoveLeftAhead();
        }
        // Đi sang trái: nếu camera sắp qua nửa trái của segmentLeft -> kéo segmentRight về phía sau
        else if (camX < leftSeg.position.x + (segmentLength * 0.5f) + lead)
        {
            MoveRightBehind();
        }
    }

    void MoveLeftAhead()
    {
        leftSeg.position = rightSeg.position + Vector3.right * segmentLength;
        // hoán đổi tham chiếu
        var tmp = leftSeg; leftSeg = rightSeg; rightSeg = tmp;
    }

    void MoveRightBehind()
    {
        rightSeg.position = leftSeg.position - Vector3.right * segmentLength;
        // hoán đổi tham chiếu
        var tmp = leftSeg; leftSeg = rightSeg; rightSeg = tmp;
    }

    float ComputeSegmentLength(Transform segRoot)
    {
        var srs = segRoot.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs.Length == 0) return 0f;
        Bounds b = srs[0].bounds;
        for (int i = 1; i < srs.Length; i++) b.Encapsulate(srs[i].bounds);
        return b.size.x;
    }
}
