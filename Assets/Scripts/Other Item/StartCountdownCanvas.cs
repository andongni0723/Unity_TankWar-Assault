using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartCountdownCanvas : MonoBehaviour
{
    [Header("Component")]
    public GameObject countdownPanel;
    public RawImage barRawImage;
    public TMP_Text countdownText;
    public TMP_Text countdownTextShadow;
    public Timer countdownTimer;
    
    private CanvasGroup _canvasGroup;
    
    [Header("Settings")]
    public float imageMoveSpeed = 1f;
    
    //[Header("Debug")]

    private void Awake() => InitialSetting();

    private void OnEnable()
    {
        EventHandler.OnOwnerSpawned += UpdateRotation;
        EventHandler.OnAllPlayerSpawned += StartCountdown;
        countdownTimer.OnTimerEnd += FadeOut;
        countdownTimer.OnTimerEnd += EventHandler.CallOnGameStart;
    }

    private void OnDisable()
    {
        EventHandler.OnOwnerSpawned -= UpdateRotation;
        EventHandler.OnAllPlayerSpawned -= StartCountdown;
        countdownTimer.OnTimerEnd -= FadeOut;
        countdownTimer.OnTimerEnd -= EventHandler.CallOnGameStart; 
    }

    private void Update() => UpdateUI();
    
    private void InitialSetting()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        countdownPanel.SetActive(false);
    }
    
    private void UpdateRotation(CharacterController obj) => 
        transform.rotation = Quaternion.Euler(0, NetworkManager.Singleton.IsHost ? 90 : -90, 0); 
    
    private void StartCountdown() => countdownTimer.Play();

    private void UpdateUI()
    {
        barRawImage.uvRect = new Rect(0, barRawImage.uvRect.y + Time.deltaTime * imageMoveSpeed, 1, barRawImage.uvRect.height);
        if(!countdownTimer.isPlay) return;
        countdownText.text = (countdownTimer.time - countdownTimer.currentTime).ToString("0");
        countdownTextShadow.text = countdownText.text;
    }

    private void FadeOut()
    {
        _canvasGroup.DOFade(0, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }
}
