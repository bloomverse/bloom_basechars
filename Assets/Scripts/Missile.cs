using System;
using UnityEngine;

    public class Missile : MonoBehaviour {
        [Header("REFERENCES")] 
        [SerializeField] private Rigidbody _rb;
         public GameObject _target;
        [SerializeField] private GameObject _explosionPrefab;

        [Header("MOVEMENT")] 
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _rotateSpeed = 95;

        [Header("PREDICTION")] 
        [SerializeField] private float _maxDistancePredict = 100;
        [SerializeField] private float _minDistancePredict = 5;
        [SerializeField] private float _maxTimePrediction = 5;
        private Vector3 _standardPrediction, _deviatedPrediction;

        [Header("DEVIATION")] 
        [SerializeField] private float _deviationAmount = 10;
        [SerializeField] private float _deviationSpeed = 2;

         [SerializeField] StatusEffectData _data;
         public float radius = 5f;
         public float force = 20f;

        private void FixedUpdate() {
            _rb.velocity = transform.forward * _speed;

            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

            PredictMovement(leadTimePercentage);

            AddDeviation(leadTimePercentage);

            RotateRocket();
        }

        private void PredictMovement(float leadTimePercentage) {
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
            var Rb = _target.GetComponent<Rigidbody>();
            _standardPrediction = Rb.position + Rb.velocity; //* predictionTime;
        }

        private void AddDeviation(float leadTimePercentage) {
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
            
            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            _deviatedPrediction = _standardPrediction + predictionOffset;
        }

        private void RotateRocket() {
            var heading = _deviatedPrediction - transform.position;

            var rotation = Quaternion.LookRotation(heading);
            _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
        }

        private bool hasCollisioned;

        private void OnCollisionEnter(Collision collision) {
            

            if(collision.collider.tag!="Missile" && !hasCollisioned){

                hasCollisioned = true;
                 if(_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
         

            Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, 5);
            
            foreach(Collider nearbyObject in collidersToDestroy){

                    // Destructible
                    Destructible dest  = nearbyObject.GetComponent<Destructible>();
                    if(dest !=null){
                        dest.destroyObj();
                    }
                    // Efectable
                    var effectable = nearbyObject.GetComponent<IEffectable>();
                    if(effectable!=null){
                        float dist = Vector3.Distance(transform.position,nearbyObject.transform.position); 
                        Debug.Log(dist  + " dist" );

                        effectable.ApplyEffect(_data);
                    }
            }

            Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

            foreach(Collider nearbyObject in collidersToMove){
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                if(rb !=null){
                    rb.AddExplosionForce(force,transform.position,radius);
                }
            }   



                  Destroy(gameObject);
            }
           
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
        }
    }
