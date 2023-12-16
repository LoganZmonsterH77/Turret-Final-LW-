using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private float lookInterval = 0.1f;
    [Range(30, 110)]
    [SerializeField] private float fieldOfView = 75;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Light spotLight;
    [SerializeField] private Color detectedColor = Color.green;
    [SerializeField] private Color withinViewColor = Color.yellow;
    [SerializeField] private Color notInViewColor = Color.red;

    private Transform emitter;
    private GameObject[] players;
    private bool canSeePlayer = false;

    void Start()
    {
        emitter = transform.GetChild(0);
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(CheckForPlayers());
    }

    void Update()
    {
        if (canSeePlayer)
        {
            RotateTowardsClosestPlayer();
        }
    }

    IEnumerator CheckForPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(lookInterval);

            GameObject detectedPlayer = null;
            Color spotlightColor = notInViewColor; // Default color

            foreach (GameObject player in players)
            {
                Ray ray = new Ray(emitter.position, player.transform.position - emitter.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        Vector3 targetDir = player.transform.position - emitter.position;
                        float angle = Vector3.Angle(targetDir, emitter.forward);

                        if (angle < 45)
                        {
                            Debug.Log("Found a bad guy!");
                            StartCoroutine(CallTurrets());
                            canSeePlayer = true;
                            Debug.DrawRay(emitter.position, player.transform.position - emitter.position, detectedColor, 4);
                            detectedPlayer = player;
                            spotlightColor = detectedColor; // Detected player color
                        }
                        else
                        {
                            canSeePlayer = false;
                            Debug.DrawRay(emitter.position, player.transform.position - emitter.position, withinViewColor, 4);
                            spotlightColor = withinViewColor; // Within view but not detected color
                        }
                    }
                    else
                    {
                        canSeePlayer = false;
                        Debug.DrawRay(emitter.position, hit.transform.position - emitter.position, notInViewColor, 4);
                        spotlightColor = notInViewColor; // Not within view color
                    }
                }
                else
                {
                    canSeePlayer = false;
                    Debug.DrawRay(emitter.position, player.transform.position - emitter.position, notInViewColor, 4);
                    spotlightColor = notInViewColor; // Not within view color
                }
            }

            // Update spotlight color
            spotLight.color = spotlightColor;

            // Update spotlight direction based on detection
            if (detectedPlayer != null)
            {
                Vector3 targetDir = detectedPlayer.transform.position - emitter.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                spotLight.transform.rotation = Quaternion.Slerp(spotLight.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator CallTurrets()
    {
        Debug.Log("Calling Turrets function.");
        if (canSeePlayer == false)
        {
            canSeePlayer = true;
            yield return new WaitForSeconds(1);

            if (canSeePlayer)
            {
                GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
                foreach (GameObject turret in turrets)
                {
                    turret.GetComponent<TurretController>().Activate();
                }
            }
        }
    }

    void RotateTowardsClosestPlayer()
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, emitter.position);
            float angle = Vector3.Angle(player.transform.position - emitter.position, emitter.forward);

            if (distance < closestDistance && angle < fieldOfView * 0.5f)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            Vector3 targetDir = closestPlayer.transform.position - emitter.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            // Gradually rotate towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
