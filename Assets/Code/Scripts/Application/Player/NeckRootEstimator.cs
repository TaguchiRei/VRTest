using UnityEngine;

/// <summary>
/// HMD回転・位置から首の付け根の位置を推定するクラス
/// </summary>
public class NeckRootEstimator
{
    private Quaternion _prevNeckRotation = Quaternion.identity;

    /// <summary>
    /// 推奨デフォルト値を入れておく
    /// </summary>
    public HmdSettings HmdSettings = new(
        neckHeight: 0.18f,
        headForwardOffset: 0.08f,
        yawWeight: 1f,
        pitchWeight: 1f,
        rollWeight: 1f,
        neckYawLimit: 70f
    );

    /// <summary>
    /// HMD回転・位置から首の根本の位置を推定する
    /// HMDの回転から、首に対するベクトルが分かるため、HMDの位置からそのベクトル分を引いた位置を首の位置としている
    /// </summary>
    public NeckTransform EstimateNeckRootTransform(
        Quaternion hmdRotation,
        Vector3 hmdPosition,
        Quaternion bodyRotation)
    {
        // HMD回転をBody空間へ変換（相対回転）
        // bodyRotation.inverse * hmdRotation = "Bodyから見たHMDの向き"
        Quaternion relativeRotation = Quaternion.Inverse(bodyRotation) * hmdRotation;

        Vector3 euler = NormalizeEuler(relativeRotation.eulerAngles);

        float yaw = euler.y * HmdSettings.YawWeight;
        float pitch = euler.x * HmdSettings.PitchWeight;
        float roll = euler.z * HmdSettings.RollWeight;

        // 首Yaw制限（Body相対なので±70°が自然な可動域）
        float clampedYaw = Mathf.Clamp(yaw, -HmdSettings.NeckYawLimit, HmdSettings.NeckYawLimit);

        Quaternion neckRotationLocal = Quaternion.Euler(pitch, clampedYaw, roll);

        // ワールド空間へ戻す
        Quaternion neckRotationWorld = bodyRotation * neckRotationLocal;

        // 首→頭オフセットをワールド空間で計算
        Vector3 neckToHead = new Vector3(0f, HmdSettings.NeckHeight, HmdSettings.HeadForwardOffset);
        Vector3 rotatedOffset = neckRotationWorld * neckToHead;
        Vector3 neckPosition = hmdPosition - rotatedOffset;

        float excessYaw = (yaw - clampedYaw); // Body相対での超過分

        return new NeckTransform(neckPosition, neckRotationWorld, excessYaw);
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
                HmdSettings.MAX_WEIGHT
            ),
            source.NeckYawLimit
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
            settings.RollWeight <= HmdSettings.MAX_WEIGHT;
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