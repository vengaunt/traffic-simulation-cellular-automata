using KalkuzSystems.Attributes;
using UnityEngine;
using Header = KalkuzSystems.Attributes.HeaderAttribute;

namespace TrafficSimulation.Vehicles
{
    [CreateAssetMenu(menuName = "Traffic Simulation/Vehicle Data")]
    public class VehicleData : ScriptableObject
    {
        [Header("Vehicle Properties")]
        [SerializeField] protected float maxSpeed; // km/h
        [SerializeField] protected float accelerationPotential; // m/s^2
        [SerializeField] protected float decelerationPotential;
        [SerializeField] protected float mass;

        [LineSeparator(1, 18)] 
        [SerializeField] protected float length;
        [SerializeField] protected float width;
        [SerializeField] protected float height;
        
        [Header("Experimental zone")]
        [SerializeField] protected float brakeAccelerateReactionTime;
        [SerializeField] protected bool actAsObstacle;

        public float MaxSpeed => maxSpeed;
        public float AccelerationPotential => accelerationPotential;
        
        public float DecelerationPotential => decelerationPotential;
        public float Mass => mass;

        public float Length => length;
        public float Width => width;
        public float Height => height;

        public bool ActAsObstacle => actAsObstacle;
    }
}