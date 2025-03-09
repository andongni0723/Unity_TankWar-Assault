using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraShake : MonoBehaviour
{
    [Header("Component")]
    private CinemachineBasicMultiChannelPerlin noise;
    public Timer shakeTimer;
    
    //[Header("Settings")]
    [Header("Debug")]
    private Action _FadeCloseShakeAction;
    
    private void Awake()
    {
        noise = (CinemachineBasicMultiChannelPerlin)GetComponent<CinemachineCamera>().GetCinemachineComponent(CinemachineCore.Stage.Noise);
    }

    private void OnEnable()
    {
        EventHandler.CameraShake += OnCameraShake;
    }

    private void OnDisable()
    {
        EventHandler.CameraShake -= OnCameraShake;
    }

    private void OnCameraShake(float intensity, float duration, float startTimeToFade)
    {
        if(duration < 0 && startTimeToFade < 0)
            Shake(intensity);
        else if(startTimeToFade < 0)
            Shake(intensity, duration);
        else
            Shake(intensity, duration, startTimeToFade);
    }

    public void Shake(float intensity) => noise.AmplitudeGain = intensity;
    public void Shake(float intensity, float duration)
    {
        Shake(intensity);
        TimeToCloseShake(duration);
    }
    /// <param name="startTimeToFade">when the time, the shake intensity will fade to zero</param>
    public void Shake(float intensity, float duration, float startTimeToFade)
    {
        Shake(intensity, duration);
        TimeToFadeCloseShake(startTimeToFade, duration - startTimeToFade);
    }

    public void TimeToCloseShake(float time)
    {
        shakeTimer.time = time;
        shakeTimer.OnTimerEnd += CloseShake;
        shakeTimer.Play();
    }

    public void TimeToFadeCloseShake(float waitTime, float fadeDuration)
    {
        shakeTimer.time = waitTime;
        _FadeCloseShakeAction = () => FadeCloseShake(fadeDuration);
        shakeTimer.OnTimerEnd += _FadeCloseShakeAction;
        shakeTimer.Play();
    }

    public void CloseShake()
    {
        noise.AmplitudeGain = 0;
        shakeTimer.OnTimerEnd -= CloseShake;
    }

    public void FadeCloseShake(float fadeDuration)
    {
        DOTween.To(() => noise.AmplitudeGain, x => noise.AmplitudeGain = x, 0, fadeDuration);
        shakeTimer.OnTimerEnd -= _FadeCloseShakeAction;
    }
}
