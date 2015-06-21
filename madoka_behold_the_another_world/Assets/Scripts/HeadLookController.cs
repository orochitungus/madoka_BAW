using UnityEngine;
using System.Collections;


[System.Serializable]
public class BendingSegment
{
    public Transform firstTransform;
    public Transform lastTransform;
    // しきい値の角度差
    public float thresholdAngleDifference = 0;
    // 曲げ乗数
    public float bendingMultiplier = 0.6f;
    // 最大角度差
    public float maxAngleDifference = 30;
    // 最大曲げ角度
    public float maxBendingAngle = 80;
    // 反応性
    public float responsiveness = 5;
    internal float angleH;
    internal float angleV;
    internal Vector3 dirUp;
    internal Vector3 referenceLookDir;
    internal Vector3 referenceUpDir;
    internal int chainLength;
    internal Quaternion[] origRotations;
}


[System.Serializable]
public class NonAffectedJoints
{
    public Transform joint;
    public float effect = 0;
}

public class HeadLookController : MonoBehaviour
{

    public Transform rootNode;
    public BendingSegment[] segments;
    public NonAffectedJoints[] nonAffectedJoints;
    public Vector3 headLookVector = Vector3.forward;
    public Vector3 headUpVector = Vector3.up;
    public Vector3 target = Vector3.zero;
    public float effect = 1;
    public bool overrideAnimation = false;
    public bool m_IsRockon = false;

    void Start()
    {
        if (rootNode == null)
        {
            rootNode = transform;
        }

        // Setup segments
        foreach (BendingSegment segment in segments)
        {
            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
            segment.referenceLookDir =
                parentRotInv * rootNode.rotation * headLookVector.normalized;
            segment.referenceUpDir =
                parentRotInv * rootNode.rotation * headUpVector.normalized;
            segment.angleH = 0;
            segment.angleV = 0;
            segment.dirUp = segment.referenceUpDir;

            segment.chainLength = 1;
            Transform t = segment.lastTransform;
            while (t != segment.firstTransform && t != t.root)
            {
                segment.chainLength++;
                t = t.parent;
            }

            segment.origRotations = new Quaternion[segment.chainLength];
            t = segment.lastTransform;
            for (int i = segment.chainLength - 1; i >= 0; i--)
            {
                segment.origRotations[i] = t.localRotation;
                t = t.parent;
            }
        }
    }

    void LateUpdate()
    {
        if (Time.deltaTime == 0)
            return;

        // 非ロックオン時もなにもさせない
        if (!this.m_IsRockon)
        {
            return;
        }

        // 影響を受けるべきではない関節の初期方向を覚える
        Vector3[] jointDirections = new Vector3[nonAffectedJoints.Length];
        for (int i = 0; i < nonAffectedJoints.Length; i++)
        {
            foreach (Transform child in nonAffectedJoints[i].joint)
            {
                jointDirections[i] = child.position - nonAffectedJoints[i].joint.position;
                break;
            }
        }

        // 各セグメントを処理
        foreach (BendingSegment segment in segments)
        {
            Transform t = segment.lastTransform;
            if (overrideAnimation)
            {
                for (int i = segment.chainLength - 1; i >= 0; i--)
                {
                    t.localRotation = segment.origRotations[i];
                    t = t.parent;
                }
            }

            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);

            // ワールド空間で一見の方向を希望する
            Vector3 lookDirWorld = (target - segment.lastTransform.position).normalized;

            // 首の親空間内の目的の視線方向
            Vector3 lookDirGoal = (parentRotInv * lookDirWorld);

            // ターゲットを見て水平方向および垂直方向の回転角度を取得
            float hAngle = AngleAroundAxis(
                segment.referenceLookDir, lookDirGoal, segment.referenceUpDir
            );

            Vector3 rightOfTarget = Vector3.Cross(segment.referenceUpDir, lookDirGoal);

            Vector3 lookDirGoalinHPlane =
                lookDirGoal - Vector3.Project(lookDirGoal, segment.referenceUpDir);

            float vAngle = AngleAroundAxis(
                lookDirGoalinHPlane, lookDirGoal, rightOfTarget
            );

            // しきい値の角度差、曲げ乗数、および最大角度差をここで扱う
            float hAngleThr = Mathf.Max(
                0, Mathf.Abs(hAngle) - segment.thresholdAngleDifference
            ) * Mathf.Sign(hAngle);

            float vAngleThr = Mathf.Max(
                0, Mathf.Abs(vAngle) - segment.thresholdAngleDifference
            ) * Mathf.Sign(vAngle);

            hAngle = Mathf.Max(
                Mathf.Abs(hAngleThr) * Mathf.Abs(segment.bendingMultiplier),
                Mathf.Abs(hAngle) - segment.maxAngleDifference
            ) * Mathf.Sign(hAngle) * Mathf.Sign(segment.bendingMultiplier);

            vAngle = Mathf.Max(
                Mathf.Abs(vAngleThr) * Mathf.Abs(segment.bendingMultiplier),
                Mathf.Abs(vAngle) - segment.maxAngleDifference
            ) * Mathf.Sign(vAngle) * Mathf.Sign(segment.bendingMultiplier);

            // ここで最大曲げ角度を扱う
            hAngle = Mathf.Clamp(hAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
            vAngle = Mathf.Clamp(vAngle, -segment.maxBendingAngle, segment.maxBendingAngle);

            Vector3 referenceRightDir =
                Vector3.Cross(segment.referenceUpDir, segment.referenceLookDir);

            // Lerp角
            segment.angleH = Mathf.Lerp(
                segment.angleH, hAngle, Time.deltaTime * segment.responsiveness
            );
            segment.angleV = Mathf.Lerp(
                segment.angleV, vAngle, Time.deltaTime * segment.responsiveness
            );

            // 行き方を教えてもらう
            lookDirGoal = Quaternion.AngleAxis(segment.angleH, segment.referenceUpDir)
                * Quaternion.AngleAxis(segment.angleV, referenceRightDir)
                * segment.referenceLookDir;

            // 最大垂直外観を作る
            Vector3 upDirGoal = segment.referenceUpDir;
            Vector3.OrthoNormalize(ref lookDirGoal, ref upDirGoal);

            // 首の親空間内のルックアップと方向を補間e
            Vector3 lookDir = lookDirGoal;
            segment.dirUp = Vector3.Slerp(segment.dirUp, upDirGoal, Time.deltaTime * 5);
            Vector3.OrthoNormalize(ref lookDir, ref segment.dirUp);

            // ワールド空間の回転を見る
            Quaternion lookRot = (
                (parentRot * Quaternion.LookRotation(lookDir, segment.dirUp))
                * Quaternion.Inverse(
                    parentRot * Quaternion.LookRotation(
                        segment.referenceLookDir, segment.referenceUpDir
                    )
                )
            );

            // セグメント内のすべての関節の上回転を有効化
            Quaternion dividedRotation =
                Quaternion.Slerp(Quaternion.identity, lookRot, effect / segment.chainLength);
            t = segment.lastTransform;
            for (int i = 0; i < segment.chainLength; i++)
            {
                t.rotation = dividedRotation * t.rotation;
                t = t.parent;
            }
        }

        // アタッチされていない関節を扱う
        for (int i = 0; i < nonAffectedJoints.Length; i++)
        {
            Vector3 newJointDirection = Vector3.zero;

            foreach (Transform child in nonAffectedJoints[i].joint)
            {
                newJointDirection = child.position - nonAffectedJoints[i].joint.position;
                break;
            }

            Vector3 combinedJointDirection = Vector3.Slerp(
                jointDirections[i], newJointDirection, nonAffectedJoints[i].effect
            );

            nonAffectedJoints[i].joint.rotation = Quaternion.FromToRotation(
                newJointDirection, combinedJointDirection
            ) * nonAffectedJoints[i].joint.rotation;
        }
    }

    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);

        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
}