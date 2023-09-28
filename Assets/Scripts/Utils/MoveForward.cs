using UnityEngine;
 
using System.Collections;
using System.Security.Cryptography.X509Certificates;


public class MoveForward : MonoBehaviour
{
    private Vector3 velocity;
    public Vector3 directionMultiplier = new Vector3(1, 0, 0); // Puedes ajustar esto en el Inspector
    public float speedMultiplier = 1.0f;
    public float minimumSpeed = 1.0f; 
    public bool isBullet = false; 
    public float distanceThreshold = 50.0f; 
    public GameObject bulletImpactVFX;  
    public GameObject missileImpactVFX; 
    public float missileVFXScale = 1.0f;
    public void ActivateBullet(Vector3 position, Quaternion rotation, Vector3 initialVelocity, bool isBullet)
    {
        transform.position = position;
        transform.rotation = rotation;
        this.isBullet = isBullet;
        if (isBullet && initialVelocity.magnitude < minimumSpeed)
        {
            this.velocity = transform.forward * minimumSpeed;
        }
        else
        { 
            this.velocity = Vector3.Scale(initialVelocity, directionMultiplier);   
        }
        
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime * speedMultiplier;
    }

    public void SetSpeedMultiplier(float newMultiplier)
    {
        speedMultiplier = newMultiplier;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detectado con: " + other.gameObject.name + ", tag: " + other.gameObject.tag);
        if (other.gameObject.tag == "Objective")
        {
            StartCoroutine(DespawnAfterSeconds(3f));   
            other.gameObject.SetActive(false);  

            if (!isBullet)
            {
                Transform explosion = transform.Find("WFX_ExplosionLandMine");
                if (explosion != null)
                {
                    explosion.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("No se encontró el hijo 'WFX_Explosion LandMine'");
                }
            }

            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.ObjectiveDestroyed();
            }
            else
            {
                Debug.Log("ObjectiveManager.Instance es null");
            }
            showVFX_Humo();
            playAudio(other);
        }

        if (other.gameObject.tag == "PuedeRecibirDisparos")
        {
            showVFX_Humo();
            playAudio(other);
            StartCoroutine(DespawnAfterSeconds(3f));
        }
    }
    
    
    
    
    
    
    void playAudio(Collider other)
    {
        // Calcular la distancia al objeto con el que se colisionó
        float distance = Vector3.Distance(transform.position, other.transform.position);

        if (isBullet)
        {
            AudioManager.Instance.Play("eplosionLejos");
        }
        else
        {
            if (distance <= distanceThreshold)
            {
                AudioManager.Instance.Play("eplosionCerca");
            }
            else
            {
                AudioManager.Instance.Play("eplosionLejos");
            }
        }
    }
    void showVFX_Humo()
    {
        GameObject instantiatedVFX;
        GameObject instantiatedVFX_Fire;
        if (isBullet)
        { 
            instantiatedVFX = Instantiate(bulletImpactVFX, transform.position, Quaternion.identity);
        }
        else
        {
            showVFX_Explosion(0.5f);
            instantiatedVFX = Instantiate(missileImpactVFX, transform.position, Quaternion.identity);
            instantiatedVFX_Fire= Instantiate(bulletImpactVFX, transform.position, Quaternion.identity);
            instantiatedVFX.transform.localScale *= missileVFXScale;
        }
    }

    IEnumerator showVFX_Explosion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Transform transformVfxInPrefab = transform.Find("WFX_Explosion");
        transformVfxInPrefab.gameObject.SetActive(true);
    }
    
    IEnumerator DespawnAfterSeconds(float seconds)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        yield return new WaitForSeconds(seconds);
        PoolManager.Instance.Despawn(this.gameObject);
        // Reactivar el MeshRenderer para la próxima vez que se utilice este objeto
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }
    }
}

 
   



