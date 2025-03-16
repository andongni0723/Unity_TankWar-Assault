#if PLATFORM_ANDROID && !UNITY_EDITOR
using UnityEngine;

public static class VibratorHelper
{
    /// <summary>
    /// 震動指定持續時間和強度。
    /// </summary>
    /// <param name="duration">持續時間（毫秒）</param>
    /// <param name="amplitude">震動強度（0.0 ~ 1.0）</param>
    public static void Vibrate(long duration, float amplitude)
    {
        // 確保 amplitude 在 0 到 1 範圍內
        amplitude = Mathf.Clamp01(amplitude);

        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (vibrator == null || !vibrator.Call<bool>("hasVibrator")) return;
        // Android API 26 以上可以使用 VibrationEffect 控制震動強度
        using AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
        // amplitude 需要轉換成 0~255 之間的整數
        int amp = Mathf.RoundToInt(amplitude * 255);
        AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", duration, amp);
        vibrator.Call("vibrate", vibrationEffect);
    }
}
#endif