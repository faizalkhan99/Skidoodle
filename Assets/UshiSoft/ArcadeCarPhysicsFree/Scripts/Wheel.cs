using UnityEngine;

namespace UshiSoft.UACPF
{
    [DisallowMultipleComponent]
    public class Wheel : MonoBehaviour
    {
        public enum HitDetectionType
        {
            Ray,
            Sphere,
        }

        [SerializeField] private HitDetectionType _hitDetectionType = HitDetectionType.Ray;
        [SerializeField] private LayerMask _layerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Ignore;
        [SerializeField, Min(0f)] private float _suspensionStroke = 0.1f;
        [SerializeField, Min(0f)] private float _suspensionSpring;
        [SerializeField, Min(0f)] private float _suspensionDamper;
        [SerializeField, Min(0.001f)] private float _radius;
        [SerializeField] private float _width = 0.2f;
        [SerializeField, Range(-45f, 45f)] private float _camberAngle = 0f;
        [SerializeField] private float _groundOffset = 0f;
        [SerializeField] private Transform _model;
        [SerializeField, Min(8f)] private const int _gizmosSmoothness = 16;

        private Rigidbody _rigidbody;
        private float _suspensionCompression;
        private float _prevSuspensionCompression;
        private bool _grounded;
        private RaycastHit _hitInfo;
        private float _steerAngle;
        private float _angle;
        private float _angularVelocity;
        private float _sidewaysVel; // The sideways velocity of the wheel.
        private Vector3 _totalForce;

        // This new public property exposes the wheel's sideways velocity to other scripts, like the car controller.
        public float SidewaysVelocity => _sidewaysVel;

        public float SteerAngle
        {
            get => _steerAngle;
            set => _steerAngle = value;
        }
        public float SuspensionLength => _suspensionStroke - _suspensionCompression;
        public float SuspensionStroke
        {
            get => _suspensionStroke;
            set => _suspensionStroke = value;
        }
        public float SuspensionSpring
        {
            get => _suspensionSpring;
            set => _suspensionSpring = value;
        }
        public float SuspensionDamper
        {
            get => _suspensionDamper;
            set => _suspensionDamper = value;
        }
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public float Width
        {
            get => _width;
            set => _width = value;
        }
        public bool Grounded => _grounded;
        public RaycastHit HitInfo => _hitInfo;
        public Vector3 Forward => Quaternion.AngleAxis(_steerAngle, transform.up) * transform.forward;
        public Vector3 Center => transform.position - transform.up * SuspensionLength;
        public Quaternion Rotation => transform.rotation
            * Quaternion.Euler(0f, _steerAngle, 0f)
            * Quaternion.Euler(0f, 0f, FixedCamberAngle)
            * Quaternion.Euler(_angle, 0f, 0f);
        public float FixedCamberAngle => -Mathf.Sign(transform.localPosition.x) * _camberAngle;
        public float GroundOffset
        {
            get => _groundOffset;
            set => _groundOffset = value;
        }
        public Transform Model
        {
            get => _model;
            set => _model = value;
        }

        private void Awake()
        {
            _rigidbody = GetComponentInParent<Rigidbody>();
        }

        [SerializeField] private float forwardGrip = 1.5f;   // forward resistance
        [SerializeField] private float sidewaysGrip = 2.5f;  // sideways resistance
        [SerializeField] private float handbrakeGripFactor = 0.3f; // how much grip remains on handbrake

        private void ApplyFriction()
        {
            if (!_grounded) return;

            // velocity at contact point
            Vector3 vel = _rigidbody.GetPointVelocity(_hitInfo.point);

            // break into forward/side components
            float forwardVel = Vector3.Dot(vel, Forward);
            float sidewaysVel = Vector3.Dot(vel, transform.right);
            // We store the sideways velocity here so the car controller can access it for effects.
            _sidewaysVel = sidewaysVel;

            // Grip scaling (reduce sideways grip if handbrake pressed)
            float sideGrip = Input.GetKey(KeyCode.Space) ? sidewaysGrip * handbrakeGripFactor : sidewaysGrip;

            // Opposing forces
            Vector3 forwardForce = forwardGrip * forwardVel * -Forward * 10;
            Vector3 sidewaysForce = sideGrip * sidewaysVel * -transform.right * 10;

            // Apply at wheel contact
            Vector3 total = forwardForce + sidewaysForce;
            _rigidbody.AddForceAtPosition(total, _hitInfo.point);

            _totalForce += total; // debug gizmo
        }

        private void FixedUpdate()
        {
            _totalForce = Vector3.zero;
            CheckGrounded();
            UpdateSuspensionCompression();
            AddSuspensionForce();
            ApplyFriction();
            UpdateModel();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, Center);

            var originalMat = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(Center, Rotation, transform.lossyScale);

            switch (_hitDetectionType)
            {
                case HitDetectionType.Ray:
                    DrawWheelWithGizmos();
                    break;

                case HitDetectionType.Sphere:
                    Gizmos.DrawWireSphere(Vector3.zero, _radius);
                    break;
            }

            Gizmos.matrix = originalMat;

            if (_grounded)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(_hitInfo.point, _totalForce * 0.0001f);
            }
        }

        private void DrawWheelWithGizmos()
        {
            for (var i = 0; i < _gizmosSmoothness; i++)
            {
                var r1 = 2f * Mathf.PI * (float)i / (float)_gizmosSmoothness;
                var r2 = 2f * Mathf.PI * (float)(i + 1) / (float)_gizmosSmoothness;
                var lp1 = GetLocalPointOnWheelCircumference(r1);
                var lp2 = GetLocalPointOnWheelCircumference(r2);
                Gizmos.DrawLine(lp1, lp2);
            }
        }

        private Vector3 GetLocalPointOnWheelCircumference(float angleInRadian)
        {
            return new Vector3(
                0f,
                Mathf.Sin(angleInRadian) * _radius,
                -Mathf.Cos(angleInRadian) * _radius);
        }

        private void CheckGrounded()
        {
            switch (_hitDetectionType)
            {
                case HitDetectionType.Ray:
                    _grounded = Physics.Raycast(
                        transform.position,
                        -transform.up,
                        out _hitInfo,
                        _suspensionStroke + _radius * Mathf.Cos(_camberAngle * Mathf.Deg2Rad),
                        _layerMask,
                        _queryTriggerInteraction);
                    break;

                case HitDetectionType.Sphere:
                    _grounded = Physics.SphereCast(
                        transform.position + transform.up * _radius,
                        _radius,
                        -transform.up,
                        out _hitInfo,
                        _suspensionStroke + _radius,
                        _layerMask,
                        _queryTriggerInteraction);
                    break;
            }
        }

        private void UpdateSuspensionCompression()
        {
            _prevSuspensionCompression = _suspensionCompression;

            if (_grounded)
            {
                var dist = _hitDetectionType == HitDetectionType.Ray ? _radius * Mathf.Cos(_camberAngle * Mathf.Deg2Rad) : _radius;
                _suspensionCompression = _suspensionStroke - (_hitInfo.distance + _groundOffset - dist);
            }
            else
            {
                _suspensionCompression = 0f;
            }
        }

        private void AddSuspensionForce()
        {
            if (!_grounded)
            {
                return;
            }

            var springForce = _suspensionSpring * _suspensionCompression;

            var susVel = (_suspensionCompression - _prevSuspensionCompression) / Time.fixedDeltaTime;
            var damperForce = _suspensionDamper * susVel;

            var force = springForce + damperForce;
            if (force < 0f)
            {
                force = 0f;
            }

            var forceVec = transform.up * force;
            _rigidbody.AddForceAtPosition(forceVec, _hitInfo.point);

            _totalForce += forceVec;
        }

        private void UpdateModel()
        {
            if (_model == null)
            {
                return;
            }

            if (_grounded)
            {
                var vel = _rigidbody.GetPointVelocity(_hitInfo.point);

                if (_hitInfo.collider.attachedRigidbody != null)
                {
                    var contactVel = _hitInfo.collider.attachedRigidbody.GetPointVelocity(_hitInfo.point);
                    vel -= contactVel;
                }

                var forwardVel = Vector3.Dot(vel, Forward);

                _angularVelocity = forwardVel / _radius;
            }

            _angle += _angularVelocity * Time.fixedDeltaTime * Mathf.Rad2Deg;

            _model.position = Center;
            _model.rotation = Rotation;
        }
    }
}