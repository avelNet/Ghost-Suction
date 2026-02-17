using TMPro;
using UnityEditorInternal;
using UnityEngine;

public class VacuumSuction : MonoBehaviour
{
    [SerializeField] private VacuumData _gunData;
    [SerializeField] private Transform _vaccumPoint;
    [SerializeField] private GameObject suctionVisualizer;

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            if (suctionVisualizer != null)
            {
                suctionVisualizer.SetActive(true);
                ApplySuction();
            }
        }
        else
        {
            if (suctionVisualizer != null)
            {
                suctionVisualizer.SetActive(false);
            }
        }
    }

    private void ApplySuction()
    {
        Collider[] targets = Physics.OverlapSphere(_vaccumPoint.position, _gunData.suctionRange);
        foreach(Collider hit in targets)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null )
            {
                if (rb == null) continue;

                Vector3 dirToTarget = (hit.transform.position - _vaccumPoint.position).normalized;
                float angle = Vector3.Angle(_vaccumPoint.forward, dirToTarget);

                if(angle <= _gunData.coanAngle)
                {
                    PullObject(rb, hit);
                }
            }
        }
    }

    private void PullObject(Rigidbody rb, Collider hit)
    {
        Vector3 pullDir = (_vaccumPoint.position - hit.transform.position);
        float distance = pullDir.magnitude;

        if(hit.CompareTag("Ghost"))
        {
            rb.AddForce(pullDir.normalized * _gunData.suctionForce, ForceMode.Acceleration);
        }
        else if (hit.CompareTag("SmallItems"))
        {
            if(distance > _gunData.stopDistance)
            {
                rb.AddForce(pullDir.normalized * _gunData.suctionForce, ForceMode.Acceleration);
            }
            else
            {
                rb.linearVelocity *= 0.5f;
            }
        }
    }
}