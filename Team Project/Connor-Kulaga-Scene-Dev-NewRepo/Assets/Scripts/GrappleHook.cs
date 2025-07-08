using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] CharacterController controller;
    [SerializeField] float arcHeight;
    [SerializeField] float pullSpeed;
    [SerializeField] float maxGrappleDistance;
    [SerializeField] playerController player;
    [SerializeField] LineRenderer lr;
    public LayerMask grappleLayer;

    bool isGrappling;
    Vector3 grapplePoint;
    Vector3 velocity;
    float timeToTarget;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGrappling = false;
        lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ShootGrapple();
        }

        if(isGrappling)
        {
            lr.enabled = true;
            lr.SetPosition(0,transform.position);
            lr.SetPosition(1,grapplePoint);
            timer += Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            if(timer >= timeToTarget)
            {
                isGrappling=false;
                player.isGrappling = false;
            }
        }
        else
        {
            lr.enabled=false;
        }
    }

    void ShootGrapple()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        if(Physics.Raycast(ray,out RaycastHit hit,maxGrappleDistance,grappleLayer))
        {
            grapplePoint = hit.point;
            StartGrappleArc();
        }
    }

    void StartGrappleArc()
    {
        Vector3 start = transform.position;
        Vector3 end = grapplePoint;
        float gravity = Mathf.Abs(Physics.gravity.y);

        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x -start.x,0,end.z -start.z);

        float timeToApex = Mathf.Sqrt(2 * arcHeight / gravity);
        float totalVerticalTime = timeToApex + Mathf.Sqrt(2 * Mathf.Max(arcHeight - displacementY, 0.1f) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * arcHeight);
        Vector3 velocityXZ = displacementXZ /totalVerticalTime;

        velocity = velocityXZ + velocityY;
        timeToTarget = totalVerticalTime;
        timer = 0;
        isGrappling = true;
        player.isGrappling = true;
    }


}
