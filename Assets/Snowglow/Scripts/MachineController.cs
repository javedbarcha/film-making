using UnityEngine;
namespace Snowglow
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class MachineController : MonoBehaviour
    {
        public VehicleDefinition definition; public AttachmentDefinition equipment; public WheelCollider[] wheels; public Transform[] wheelVisuals;
        public AttachmentController Tool; public Rigidbody Body { get; private set; }
        public float Fuel = 70, Condition = 100; public int Tyres, Engine; public bool Owned;
        public bool Assistance = true, ActiveMachine; public float Slip, Speed, LastSnowForce;
        float steering, nextHit; Vector3 parkingPosition; Quaternion parkingRotation;
        public float FuelRatio => Fuel / definition.fuelCapacity;
        public bool AtDepot => Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(-95, -90)) < 25;
        public void Initialize()
        {
            Body = GetComponent<Rigidbody>(); Body.linearDamping = .05f; Body.angularDamping = .5f; Body.interpolation = RigidbodyInterpolation.Interpolate; Body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            string error = equipment.Validate(definition); if (error != null) { Debug.LogError(error, this); enabled = false; return; }
            parkingPosition = transform.position; parkingRotation = transform.rotation; Tool.Initialize(this);
            foreach (var w in wheels) w.ConfigureVehicleSubsteps(5, 5, 8);
            SetActiveMachine(false);
        }
        public void SetActiveMachine(bool active) { ActiveMachine = active; if (Body == null) Body = GetComponent<Rigidbody>(); Body.isKinematic = !active; foreach (var w in wheels) { w.motorTorque = 0; w.brakeTorque = active ? 0 : definition.brakeTorque; } if (active) Body.WakeUp(); }
        void FixedUpdate()
        {
            var game = PrototypeSession.Instance; if (!ActiveMachine || game == null || !game.IsRunning) return;
            var input = game.Input; Speed = Body.linearVelocity.magnitude; float forward = Vector3.Dot(Body.linearVelocity, transform.forward);
            float direction = input.Reverse ? -1 : 1; float targetSteering = input.Steer * definition.steering / (1 + Speed * .055f);
            steering = Mathf.MoveTowards(steering, targetSteering, 100 * Time.fixedDeltaTime);
            float mass = definition.mass + equipment.mass + Tool.Load * 250;
            Body.mass = mass; Body.centerOfMass = (definition.dryCenterOfMass * definition.mass + transform.InverseTransformPoint(Tool.mount.position) * equipment.mass + transform.InverseTransformPoint(Tool.mouth.position) * (Tool.Load * 250)) / mass;
            int grounded = 0; Slip = 0;
            for (int i = 0; i < wheels.Length; i++)
            {
                var w = wheels[i]; w.steerAngle = i < 2 ? steering : 0; float grip = 1;
                if (w.GetGroundHit(out WheelHit hit))
                {
                    grounded++; Slip += Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip);
                    float depth = game.Snow.Depth(hit.point); grip = Mathf.Lerp(SurfaceZone.Sample(hit.point), .67f, Mathf.Clamp01(depth / .25f)); grip *= definition.baseGrip * (1 + Tyres * .12f);
                }
                var f = w.forwardFriction; f.stiffness = grip; w.forwardFriction = f; var s = w.sidewaysFriction; s.stiffness = grip * (Assistance ? 1.2f : 1); w.sidewaysFriction = s;
                float traction = Assistance && w.GetGroundHit(out WheelHit wh) && Mathf.Abs(wh.forwardSlip) > .65f ? .55f : 1;
                bool opposite = forward * direction < -.5f;
                w.motorTorque = Fuel > 0 && Condition > 0 && Speed < definition.maxSpeed && !opposite ? input.Throttle * direction * definition.torque * (1 + Engine * .18f) * traction * Mathf.Lerp(.5f, 1, Condition / 100) : 0;
                w.brakeTorque = (input.Brake + (opposite ? input.Throttle : 0)) * definition.brakeTorque;
                if (input.Throttle < .02f && Speed < .25f) w.brakeTorque = Mathf.Max(w.brakeTorque, 800);
            }
            if (grounded > 0)
            {
                float depth = game.Snow.Depth(transform.position); float drag = mass * (.025f + depth * .12f);
                if (Speed > .15f) Body.AddForce(-Body.linearVelocity.normalized * drag);
                if (Assistance) { AntiRoll(0, 1); AntiRoll(2, 3); Vector3 upright = Vector3.Cross(transform.up, Vector3.up); Body.AddTorque(upright * mass * 1.8f - Body.angularVelocity * mass * .12f); }
            }
            LastSnowForce = Tool.Simulate(Time.fixedDeltaTime, Mathf.Max(0, forward));
            if (LastSnowForce > 0 && forward > .1f) Body.AddForce(-transform.forward * LastSnowForce);
            if (input.Throttle > .02f) Fuel = Mathf.Max(0, Fuel - Time.fixedDeltaTime * (.003f + .014f * input.Throttle + LastSnowForce * .0000012f));
            if (transform.position.y < 0 || Mathf.Abs(transform.position.x) > 485 || Mathf.Abs(transform.position.z) > 485) game.Recover();
        }
        void AntiRoll(int left, int right)
        {
            var a = wheels[left]; var b = wheels[right]; bool ga = a.GetGroundHit(out WheelHit ha), gb = b.GetGroundHit(out WheelHit hb);
            float ta = ga ? (-a.transform.InverseTransformPoint(ha.point).y - a.radius) / a.suspensionDistance : 1;
            float tb = gb ? (-b.transform.InverseTransformPoint(hb.point).y - b.radius) / b.suspensionDistance : 1; float force = (ta - tb) * 9000;
            if (ga) Body.AddForceAtPosition(a.transform.up * -force, a.transform.position); if (gb) Body.AddForceAtPosition(b.transform.up * force, b.transform.position);
        }
        void LateUpdate() { if (wheels == null) return; for (int i = 0; i < wheels.Length; i++) { wheels[i].GetWorldPose(out Vector3 p, out Quaternion q); wheelVisuals[i].SetPositionAndRotation(p, q); } }
        public void RecoverToDepot()
        {
            if (Body == null) Body = GetComponent<Rigidbody>();
            // Recovery is an explicit teleport: update the physics pose itself.
            // A Transform-only move can be overwritten by the old interpolated
            // Rigidbody pose on the next physics tick (autoSyncTransforms is off).
            var interpolation = Body.interpolation;
            Body.interpolation = RigidbodyInterpolation.None;
            Body.isKinematic = false;
            Body.linearVelocity = Vector3.zero; Body.angularVelocity = Vector3.zero;
            var destination = parkingPosition + Vector3.up * .3f;
            Body.position = destination; Body.rotation = parkingRotation;
            transform.SetPositionAndRotation(destination, parkingRotation);
            Physics.SyncTransforms();
            Body.isKinematic = !ActiveMachine;
            Body.interpolation = interpolation;
            steering = Speed = Slip = LastSnowForce = 0;
            if (wheels != null) foreach (var wheel in wheels) { wheel.motorTorque = 0; wheel.brakeTorque = definition.brakeTorque; wheel.steerAngle = 0; }
            if (ActiveMachine) Body.WakeUp();
            Tool.ResetSweep();
        }
        void OnCollisionEnter(Collision c)
        {
            if (!ActiveMachine || Time.time < nextHit || c.relativeVelocity.magnitude < 2.5f) return;
            nextHit = Time.time + .8f; Condition = Mathf.Max(0, Condition - (c.relativeVelocity.magnitude - 2) * 1.1f);
        }
    }
}
