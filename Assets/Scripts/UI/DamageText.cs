using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [Header("Settings")]
    public float jumpHeight = 2f;       // 文字跳躍高度
    public float jumpDuration = 1f;    // 跳躍動畫時間
    public float fadeDuration = 0.5f;  // 漸變消失時間
    public Vector3 offset = new Vector3(0, 1, 0); // 文字生成位置的偏移

    private TMP_Text damageText;           // 用於顯示傷害的 Text 組件

    void Awake()
    {
        damageText = GetComponent<TMP_Text>();
    }

    public void ShowDamage(int damage, Vector3 position)
    {
        // 設置文字內容
        damageText.text = damage.ToString();

        // 設置初始位置
        transform.position = position + offset;

        // 跳躍動畫
        transform.DOJump(transform.position + new Vector3(0, Random.Range(-1f, 3f), Random.Range(-1f, 3f)), jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad);

        // 漸變消失
        damageText.DOFade(0, fadeDuration)
            .SetDelay(jumpDuration - fadeDuration)
            .OnComplete(() => Destroy(gameObject)); // 動畫完成後銷毀物件
    }
}