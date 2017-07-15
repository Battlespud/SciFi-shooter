using RAIN.Motion;
using RAIN.Serialization;
using RAIN.Utility;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_5_5_0
using UnityEngine.AI;
#endif

[RAINSerializableClass]
public class UnityNavMeshMotor : RAINMotor
{
    [RAINSerializableField]
    private bool _useAgentRotation = true;

    private NavMeshAgent _agent = null;

    private Vector3 _lastPosition = Vector3.zero;
    private bool _lastFaceSucceeded = false;

    // It seems like the NavMeshAgent forces positions on
    // the NavMesh, so 3D Movement doesn't make sense
    public override bool Allow3DMovement
    {
        get { return false; }
        set { }
    }

    // Don't support this at the moment, not sure if the NavMeshAgent
    // can go off of the mesh
    public override bool AllowOffGraphMovement
    {
        get { return false; }
        set { }
    }

    // 3D Rotation is technically doable, but not the same way
    // we support it with the BasicMotor
    public override bool Allow3DRotation
    {
        get { return false; }
        set { }
    }

    // This tells us whether we want to use agent rotation or
    // the AI's rotation
    public virtual bool UseAgentRotation
    {
        get { return _useAgentRotation; }
        set
        {
            if (_useAgentRotation == value)
                return;

            _useAgentRotation = value;
            UnityAgent.updateRotation = _useAgentRotation;
        }
    }

    public virtual NavMeshAgent UnityAgent
    {
        get { return _agent; }
        set { _agent = value; }
    }

    public override void BodyInit()
    {
        base.BodyInit();

        if (AI.Body == null)
            UnityAgent = null;
        else
        {
            UnityAgent = AI.Body.GetComponent<NavMeshAgent>();
            if (UnityAgent == null)
                UnityAgent = AI.Body.AddComponent<NavMeshAgent>();

            UnityAgent.updateRotation = UseAgentRotation;
        }
    }

    public override void UpdateMotionTransforms()
    {
        // I don't believe the Unity Navigation Mesh can handle transforms, so this stays as identity
        AI.Kinematic.ParentTransform = Matrix4x4.identity;
        AI.Kinematic.Position = AI.Body.transform.position;
        AI.Kinematic.Orientation = AI.Body.transform.rotation.eulerAngles;

        // We keep the kinematic going as we use it in our Face call
        AI.Kinematic.ResetVelocities();

        // Set our speed to zero, we'll set it when we are using it
        UnityAgent.speed = 0;
    }

    public override void ApplyMotionTransforms()
    {
        // Unnecessary since we are working directly with the agent
    }

    public override bool IsAt(MoveLookTarget aTarget)
    {
        Vector3 tDistance = AI.Body.transform.position - aTarget.Position;
        tDistance.y = 0;

        return tDistance.magnitude <= UnityAgent.stoppingDistance;
    }

    public override bool IsFacing(MoveLookTarget aTarget)
    {
        if (aTarget == null || !aTarget.IsValid)
            return false;

        Vector3 tFaceTargetPosition = aTarget.Position;
        tFaceTargetPosition.y = AI.Body.transform.position.y;

        Vector3 tDirection = aTarget.Position - AI.Body.transform.position;
        tDirection.y = 0;

        return Mathf.Abs(MathUtils.WrapAngle(Vector3.Angle(AI.Body.transform.forward, tDirection.normalized))) <= CloseEnoughAngle;
    }

    public override bool Move()
    {
        if (!MoveTarget.IsValid)
            return false;

        // Set our acceleration
        UnityAgent.speed = Speed;

        // We'll just update these constantly as our value can change when the MoveTarget changes
        UnityAgent.stoppingDistance = Mathf.Max(DefaultCloseEnoughDistance, MoveTarget.CloseEnoughDistance);

        // Have to make sure the target is still in the same place
        Vector3 tEndMoved = _lastPosition - MoveTarget.Position;
        tEndMoved.y = 0;

        // If we don't have a path or our target moved
        if (!UnityAgent.hasPath || !Mathf.Approximately(tEndMoved.sqrMagnitude, 0))
        {
            UnityAgent.destination = MoveTarget.Position;
            _lastPosition = MoveTarget.Position;

            // We can return at least if we are at our destination at this point
            return IsAt(UnityAgent.destination);
        }

        // Still making a path or our path is invalid
        if (UnityAgent.pathPending || UnityAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            return false;

        // If we aren't using face rotation go ahead and face as at our next position
        if (!UseAgentRotation)
            FaceAt(UnityAgent.steeringTarget);

        return UnityAgent.remainingDistance <= UnityAgent.stoppingDistance;
    }

    public override bool Face()
    {
        if (UseAgentRotation || FaceTarget == null || !FaceTarget.IsValid)
            return false;

        bool tReturn = SimpleSteering.DoFaceTarget(AI, FaceTarget.Position, CloseEnoughAngle, false, ref _lastFaceSucceeded);
        
        // We'll update this right away as we are working directly with the agent
        AI.Kinematic.UpdateTransformData(AI.DeltaTime);
        AI.Body.transform.rotation = Quaternion.Euler(AI.Kinematic.Orientation);

        return tReturn;
    }

    public override void Stop()
    {
        UnityAgent.Stop();
    }
}