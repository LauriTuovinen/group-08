using UnityEngine;
using Pathfinding;

namespace Pathfinding
{
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationSetter : VersionedMonoBehaviour
    {
        public Transform target;
        public float range = 5.0f;
        private bool targetInRange = false;
        private IAstarAI ai;
        private bool targetInTriggerZone = false;
         public GameObject triggerZone;
        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            if (ai != null) ai.onSearchPath += Update;
        }

        void OnDisable()
        {
            if (ai != null) ai.onSearchPath -= Update;
        }

        void Update()
        {
            if (target == null || ai == null)
                return;

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= range && !targetInRange)
            {
                targetInRange = true;
                OnTargetEnterRange();
            }
            else if (distanceToTarget > range && targetInRange)
            {
                targetInRange = false;
                OnTargetExitRange();
            }

            if (targetInRange)
            {
                ai.destination = target.position;
            }
        }

        private void OnTargetEnterRange()
        {
            Debug.Log("Target entered");
        }

        private void OnTargetExitRange()
        {
            Debug.Log("Target exited");
        }
        private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(triggerZone.transform.position, range);
    }
    }
}