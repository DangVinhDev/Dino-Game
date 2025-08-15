using System.Collections.Generic;
using UnityEngine;

public class BGAlternatingSpawner : MonoBehaviour
{
    [Header("Refs")]
    public Transform cam;

    [Header("Prefabs (luân phiên)")]
    public GameObject[] bgPrefabs;   // BG, BG2, BG3

    [Header("Spawn rules")]
    [Range(0f, 1f)] public float triggerPercent = 0.1f; // thích đặt 0.1 thì đặt thôi
    public int maxActive = 3;

    [Header("Seam fix")]
    [Tooltip("Chồng mép theo world units. PPU=100 → 0.01")]
    public float overlap = 0.01f;

    // runtime
    class Segment { public Transform tf; public float startX; public float length; }
    readonly List<Segment> active = new List<Segment>();
    int nextPrefabIndex;

    void Start()
    {
        if (!cam || bgPrefabs == null || bgPrefabs.Length == 0)
        { Debug.LogError("Missing cam/bgPrefabs"); enabled = false; return; }

        // Spawn tấm đầu tại X=0 theo mép trái
        var seg0 = SpawnByLeftEdge(bgPrefabs[PrefabIdx()], 0f);
        // Tấm thứ 2 đặt sát NGAY mép phải của tấm đầu
        var seg1 = SpawnRightOf(seg0);
        active.Add(seg0); active.Add(seg1);
    }

    void LateUpdate()
    {
        if (active.Count == 0) return;

        var right = active[active.Count - 1];

        // dùng chiều dài của tấm phải để tính ngưỡng trigger
        float triggerX = right.startX + right.length * triggerPercent;
        if (cam.position.x >= triggerX)
        {
            var newSeg = SpawnRightOf(right);
            active.Add(newSeg);

            if (active.Count > maxActive)
            {
                var left = active[0];
                active.RemoveAt(0);
                Destroy(left.tf.gameObject);
            }
        }
    }

    // ==== Helpers ====

    Segment SpawnRightOf(Segment prev)
    {
        // đo mép phải của tấm trước
        Bounds prevB = GetWorldBounds(prev.tf);
        float desiredLeft = prevB.max.x - overlap;   // dán sát chồng nhẹ overlap
        return SpawnByLeftEdge(bgPrefabs[PrefabIdx()], desiredLeft);
    }

    Segment SpawnByLeftEdge(GameObject prefab, float desiredLeft)
    {
        var go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        // đo bounds sau khi instantiate
        Bounds b = GetWorldBounds(go.transform);

        // dời cả prefab để mép trái b.min.x trùng desiredLeft
        float shift = desiredLeft - b.min.x;
        go.transform.position += new Vector3(shift, 0f, 0f);

        // đo lại bounds an toàn rồi trả về thông tin
        b = GetWorldBounds(go.transform);

        return new Segment
        {
            tf = go.transform,
            startX = b.min.x,          // lưu mép trái thực tế
            length = b.size.x
        };
    }

    Bounds GetWorldBounds(Transform root)
    {
        var srs = root.GetComponentsInChildren<SpriteRenderer>(true);
        if (srs.Length == 0) return new Bounds(root.position, Vector3.zero);
        Bounds b = srs[0].bounds;
        for (int i = 1; i < srs.Length; i++) b.Encapsulate(srs[i].bounds);
        return b;
    }

    int PrefabIdx()
    {
        // Trả về Prefab map ngẫu nhiên
        return Random.Range(0, bgPrefabs.Length);
    }

}
