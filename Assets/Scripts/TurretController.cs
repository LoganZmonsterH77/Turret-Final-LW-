using System.Collections;
using UnityEngine;
using System.Collections.Generic; 

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private Transform emitter;
    [SerializeField] private Transform turretBody; 
    [SerializeField] private Transform inactive, active;
    [SerializeField] private Animator anim;
    [SerializeField] private bool canSeePlayer = false;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private AudioSource laserAudioSource; // Added AudioSource

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Queue<Rigidbody> laserPool = new Queue<Rigidbody>();



    
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

   private void Update()
{
    this.transform.LookAt(player);

}


    public void Activate()
    {
        anim.SetTrigger("Activate");
        StartCoroutine(MoveIntoPosition());
        StartCoroutine(LookForPlayers());
    }

    private IEnumerator LookForPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Ray ray = new Ray(emitter.position, player.position - emitter.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 targetDir = player.position - emitter.position;
                float angle = Vector3.Angle(targetDir, emitter.forward);

                if (angle < 45)
                {
                    Debug.Log("Found a bad guy!");
                    FoundPlayer();
                    Debug.DrawRay(emitter.position, player.position - emitter.position, Color.green, 2);
                }
                else
                {
                    LostPlayer();
                    Debug.DrawRay(emitter.position, player.position - emitter.position, Color.yellow, 4);
                }
            }
            else
            {
                Debug.DrawRay(emitter.position, player.position - emitter.position, Color.red, 4);
                LostPlayer();
            }
        }
    }

    private void FoundPlayer()
    {
        if (!canSeePlayer)
        {
            anim.SetTrigger("Firing");
            startPosition = turretBody.position;
            startRotation = turretBody.rotation;
            canSeePlayer = true;
        }
    }

    private void LostPlayer()
    {
        if (canSeePlayer)
        {
            anim.SetTrigger("Idle");
            canSeePlayer = false;
            turretBody.position = startPosition;
            turretBody.rotation = startRotation;
        }
    }
     

 private void Shoot()
{
    Debug.Log("Pew pew! Bang bang!");
    GameObject laser = Instantiate(laserPrefab, emitter.position, emitter.rotation);
     laserAudioSource.Play();

    Rigidbody rb;
    
    if (laserPool.Count > 0) {
        Debug.Log("using the laser pool");
        rb = laserPool.Dequeue();
        rb.gameObject.SetActive(true);
        rb.velocity = Vector3.zero;
        rb.transform.position = emitter.position;
        rb.transform.rotation = emitter.rotation;
        StartCoroutine(StoreLaser(rb));
    } else {
        rb = laser.GetComponent<Rigidbody>(); // Get the Rigidbody component from the instantiated laser
    }

    rb.AddRelativeForce(Vector3.forward * 100, ForceMode.Impulse);
    StartCoroutine(StoreLaser(rb));
}

    

    private IEnumerator MoveIntoPosition()
    {
        float t = 0;
        Transform turretBody = transform.GetChild(0);
        while (t < 1)
        {
            turretBody.position = Vector3.Lerp(inactive.position, active.position, t);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator StoreLaser(Rigidbody laser) {
        yield return new WaitForSeconds(0.2f);
        if(laser.gameObject.activeSelf == true) {
             laserPool.Enqueue(laser);
          laser.gameObject.SetActive(false);
          laser.transform.position = emitter.position;
          laser.transform.rotation = emitter.rotation;


        }
        
    }
}
