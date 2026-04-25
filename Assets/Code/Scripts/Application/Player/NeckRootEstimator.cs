using UnityEngine;

/// <summary>
/// HMD回転・位置から首の付け根の位置を推定するクラス
/// </summary>
public class NeckRootEstimator
{
    /// <summary>
    /// 推奨デフォルト値を入れておく
    /// </summary>
    public HmdSettings HmdSettings = new(
        neckHeight: 0.18f,
        headForwardOffset: 0.08f,
        yawWeight: 0.35f,
        pitchWeight: 1.0f,
        rollWeight: 0.65f,
        torsoDownOffset: 0.12f
    );

    /// <summary>
    /// HMD回転・位置から首の根本の位置を推定する
    /// HMDの回転から、首に対するベクトルが分かるため、HMDの位置からそのベクトル分を引いた位置を首の位置としている
    /// </summary>
    public Vector3 EstimateNeckRootPosition(
        Quaternion hmdRotation,
        Vector3 hmdPosition)
    {
        // 危険値・異常値を補正
        HmdSettings safe = Sanitize(HmdSettings);

        // Euler角を-180～180の範囲に強制して取得
        Vector3 euler = NormalizeEuler(hmdRotation.eulerAngles);

        // 各軸重み付け
        float yaw = euler.y * safe.YawWeight;
        float pitch = euler.x * safe.PitchWeight;
        float roll = euler.z * safe.RollWeight;

        Quaternion weightedRotation = Quaternion.Euler(pitch, yaw, roll);

        // 首 → 頭中心ベクトル
        Vector3 neckToHead = new Vector3(
            0f,
            safe.NeckHeight,
            safe.HeadForwardOffset);

        // 回転後オフセット
        Vector3 rotatedOffset = weightedRotation * neckToHead;

        // 頭位置から首位置逆算
        Vector3 neckPosition = hmdPosition - rotatedOffset;

        return neckPosition;
    }

    /// <summary>
    /// 設定値を安全範囲に補正
    /// </summary>
    public HmdSettings Sanitize(HmdSettings source)
    {
        return new HmdSettings(
            neckHeight: Mathf.Clamp(
                source.NeckHeight,
                HmdSettings.MIN_NECK_HEIGHT,
                HmdSettings.MAX_NECK_HEIGHT),
            headForwardOffset: Mathf.Clamp(
                source.HeadForwardOffset,
                HmdSettings.MIN_HEAD_FORWARD,
                HmdSettings.MAX_HEAD_FORWARD),
            yawWeight: Mathf.Clamp(
                source.YawWeight,
                HmdSettings.MIN_WEIGHT,
                HmdSettings.MAX_WEIGHT),
            pitchWeight: Mathf.Clamp(
                source.PitchWeight,
                HmdSettings.MIN_WEIGHT,
                HmdSettings.MAX_WEIGHT),
            rollWeight: Mathf.Clamp(
                source.RollWeight,
                HmdSettings.MIN_WEIGHT,
                HmdSettings.MAX_WEIGHT),
            torsoDownOffset: Mathf.Clamp(
                source.TorsoDownOffset,
                HmdSettings.MIN_TORSO_DOWN,
                HmdSettings.MAX_TORSO_DOWN)
        );
    }

    /// <summary>
    /// 設定値がすべて正常範囲か
    /// </summary>
    public bool IsValid(HmdSettings settings)
    {
        return
            settings.NeckHeight >= HmdSettings.MIN_NECK_HEIGHT &&
            settings.NeckHeight <= HmdSettings.MAX_NECK_HEIGHT &&
            settings.HeadForwardOffset >= HmdSettings.MIN_HEAD_FORWARD &&
            settings.HeadForwardOffset <= HmdSettings.MAX_HEAD_FORWARD &&
            settings.YawWeight >= HmdSettings.MIN_WEIGHT &&
            settings.YawWeight <= HmdSettings.MAX_WEIGHT &&
            settings.PitchWeight >= HmdSettings.MIN_WEIGHT &&
            settings.PitchWeight <= HmdSettings.MAX_WEIGHT &&
            settings.RollWeight >= HmdSettings.MIN_WEIGHT &&
            settings.RollWeight <= HmdSettings.MAX_WEIGHT &&
            settings.TorsoDownOffset >= HmdSettings.MIN_TORSO_DOWN &&
            settings.TorsoDownOffset <= HmdSettings.MAX_TORSO_DOWN;
    }

    /// <summary>
    /// Euler角を -180 ～ 180 に変換
    /// </summary>
    private Vector3 NormalizeEuler(Vector3 euler)
    {
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);
        return euler;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}