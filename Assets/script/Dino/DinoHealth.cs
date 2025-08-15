using UnityEngine;
using System;

public class DinoHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHearts = 3;
    public int currentHearts;

    public event Action<int, int> OnHealthChanged; // (current, max)

    void Awake()
    {
        currentHearts = Mathf.Clamp(currentHearts <= 0 ? maxHearts : currentHearts, 0, maxHearts);
        OnHealthChanged?.Invoke(currentHearts, maxHearts);
    }

    // 
    public void TakeDamage(int amount = 1)
    {
        if (currentHearts <= 0) return;
        currentHearts = Mathf.Max(0, currentHearts - amount);
        OnHealthChanged?.Invoke(currentHearts, maxHearts);

        if (currentHearts == 0)
        {
            //hết máu gọi gameoverscene
        }
    }

    public void Heal(int amount = 1)
    {
        currentHearts = Mathf.Min(maxHearts, currentHearts + amount);
        OnHealthChanged?.Invoke(currentHearts, maxHearts);
    }

    public void SetHearts(int hearts)
    {
        maxHearts = Mathf.Max(1, hearts);
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        OnHealthChanged?.Invoke(currentHearts, maxHearts);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.L)) Heal(1);
    }
}
