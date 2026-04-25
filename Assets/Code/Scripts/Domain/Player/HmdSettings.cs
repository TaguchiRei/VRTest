using System;

/// <summary>
/// 胴体位置推定パラメータ
/// </summary>
[Serializable]
public readonly struct HmdSettings
{
    #region あり得ない値を定数で取得できるように

    /// <summary>首高さの最低値</summary>
    public const float MIN_NECK_HEIGHT = 0.08f;

    /// <summary>首高さの最高値</summary>
    public const float MAX_NECK_HEIGHT = 0.35f;

    /// <summary>前後オフセット最低値</summary>
    public const float MIN_HEAD_FORWARD = -0.05f;

    /// <summary>前後オフセット最高値</summary>
    public const float MAX_HEAD_FORWARD = 0.25f;

    /// <summary>反映率最小</summary>
    public const float MIN_WEIGHT = 0f;

    /// <summary>反映率最大</summary>
    public const float MAX_WEIGHT = 1f;

    /// <summary>胴体下降最低値</summary>
    public const float MIN_TORSO_DOWN = 0.02f;

    /// <summary>胴体下降最高値</summary>
    public const float MAX_TORSO_DOWN = 0.50f;

    #endregion


    /// <summary>首から頭中心までの高さ(m)</summary>
    public readonly float NeckHeight;

    /// <summary>首から頭中心までの前後距離(m)</summary>
    public readonly float HeadForwardOffset;

    /// <summary>Yaw反映率(0～1)</summary>
    public readonly float YawWeight;

    /// <summary>Pitch反映率(0～1)</summary>
    public readonly float PitchWeight;

    /// <summary>Roll反映率(0～1)</summary>
    public readonly float RollWeight;

    /// <summary>胴体を頭より何m下に置くか</summary>
    public readonly float TorsoDownOffset;

    public HmdSettings(float neckHeight, float headForwardOffset, float yawWeight, float pitchWeight, float rollWeight,
        float torsoDownOffset)
    {
        NeckHeight = neckHeight;
        HeadForwardOffset = headForwardOffset;
        YawWeight = yawWeight;
        PitchWeight = pitchWeight;
        RollWeight = rollWeight;
        TorsoDownOffset = torsoDownOffset;
    }
}