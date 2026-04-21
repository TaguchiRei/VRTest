using UnityEngine;

public class DivisionVector
{
    /// <summary> 重力の方向を設定する </summary>
    public Vector3 GravityVector;

    /// <summary> 重力の大きさを設定する </summary>
    public float GravityMagnitude;

    private Vector3 _movePower;
    private Rigidbody _rigidbody;

    public DivisionVector(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
        _rigidbody.useGravity = false;
    }

    /// <summary>
    /// 正規化済み重力方向を取得する
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGravityDirection()
    {
        return GravityVector.normalized;
    }

    /// <summary>
    /// 未正規化の重力方向のベクトルを取得する
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGravityVelocity()
    {
        Vector3 v = _rigidbody.linearVelocity;
        Vector3 gDir = GetGravityDirection();

        //ドット積で射影ベクトルを利用することで重力方向の力を取得する
        return Vector3.Dot(v, gDir) * gDir;
    }

    /// <summary>
    /// 水平方向のベクトルだけを取得する
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTangentVelocity()
    {
        //重力方向の力を除くことで重力以外の力を取得する
        return _rigidbody.linearVelocity - GetGravityVelocity();
    }

    public Vector3 GetTangentDirection()
    {
        Vector3 gDir = GetGravityDirection();

        Vector3 tangent = Vector3.Cross(gDir, Vector3.forward);
        if (tangent.sqrMagnitude < 0.001f)
        {
            tangent = Vector3.Cross(gDir, Vector3.right);
        }

        return tangent.normalized;
    }

    public void SetTangentVelocity(Vector3 tangentVelocity)
    {
        Vector3 gravityVelocity = GetGravityVelocity();
        _rigidbody.linearVelocity = gravityVelocity + tangentVelocity;
    }

    public void SetGravityVelocity(Vector3 gravityVelocity)
    {
        Vector3 tangentVelocity = GetTangentVelocity();
        _rigidbody.linearVelocity = gravityVelocity + tangentVelocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _rigidbody.linearVelocity = velocity;
    }

    public void ApplyGravity()
    {
        Vector3 gDir = GetGravityDirection();
        _rigidbody.AddForce(gDir * GravityMagnitude, ForceMode.Acceleration);
    }

    /// <summary>
    /// 2次元入力を重力と直交する平面上の3次元ベクトルに変換する
    /// </summary>
    /// <param name="input">2次元入力（x: 横, y: 縦）</param>
    /// <returns></returns>
    public Vector3 ConvertToTangentVector(Vector2 input)
    {
        Vector3 gDir = GetGravityDirection();
        // 重力方向に直交する基準ベクトル（横）
        Vector3 right = GetTangentDirection();
        // それらと直交するもう一つのベクトル（縦）
        Vector3 forward = Vector3.Cross(right, gDir);

        return (right * input.x + forward * input.y);
    }

    /// <summary>
    /// 重力と直交する平面方向に入力に基づいた力を加える
    /// </summary>
    /// <param name="force">2次元入力</param>
    /// <param name="mode">力のモード</param>
    public void AddTangentForce(Vector2 force, ForceMode mode = ForceMode.Force)
    {
        _rigidbody.AddForce(ConvertToTangentVector(force), mode);
    }

    /// <summary>
    /// 毎フレーム入力に対して実行することで移動力を他の物理挙動と別個で管理できる
    /// </summary>
    /// <param name="force"></param>
    public void MoveVectorChange(Vector2 force)
    {
        StopMove();

        _movePower = ConvertToTangentVector(force);
        _rigidbody.linearVelocity += _movePower;
    }

    /// <summary>
    /// 移動力を排除する
    /// </summary>
    public void StopMove()
    {
        if (_movePower != Vector3.zero)
        {
            _rigidbody.linearVelocity -= _movePower;
            _movePower = Vector3.zero;
        }
    }
}