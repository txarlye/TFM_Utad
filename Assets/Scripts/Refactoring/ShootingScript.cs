using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class ShootingScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public Transform bulletSpawnPoint;
    public Transform missileSpawnPoint;
    private Rigidbody airplaneRigidbody;
    public GameObject ShootButtonOnCabine_01;
    public GameObject ShootButtonOnCabine_02;

    public float missileInitialSpeedMultiplier = 1.5f; // Velocidad inicial (configurable desde el Inspector)
    public float missileAcceleration = 5.0f; // Velocidad acelerada (configurable desde el Inspector)
    public float timeToAccelerate = 2.0f; // Tiempo para acelerar (configurable desde el Inspector)
    public int burstCount = 16; // Número de disparos en una ráfaga
    public float timeBetweenBursts = 0.1f; // Tiempo entre disparos en una ráfaga
    private Material originalMaterial;
    private Material pressedMaterial;
    private bool canDoRafaga = true;
 
    void Start()
    {
        airplaneRigidbody = GetComponentInParent<Rigidbody>();
        if (ShootButtonOnCabine_01 != null && ShootButtonOnCabine_02 != null)
        {
            Renderer renderer = ShootButtonOnCabine_01.GetComponent<Renderer>();
            originalMaterial = renderer.material;

            pressedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            pressedMaterial.color = Color.green; 
            pressedMaterial.color = Color.green;
        }
    }

    void Update()
    {
        bool fireBulletKeyboard = Input.GetKeyDown(KeyCode.Space);
        bool fireMissileKeyboard = Input.GetKeyDown(KeyCode.B);
        bool fireBulletOculus = false;
        bool fireMissileOculus = false;
        
        // Obtener el estado del gatillo del controlador derecho
        InputDevice deviceRight = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice deviceLeft = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        deviceRight.TryGetFeatureValue(CommonUsages.triggerButton, out fireBulletOculus);
        
        // Para el botón B (disparar bala)
        bool buttonBState = false;
        if (deviceRight.TryGetFeatureValue(new InputFeatureUsage<bool>("ButtonTwo"), out buttonBState) && buttonBState)
        {
            fireBulletOculus = true;
        }

        // Para el botón A (disparar misil)
        bool buttonAState = false;
        if (deviceRight.TryGetFeatureValue(new InputFeatureUsage<bool>("ButtonOne"), out buttonAState) && buttonAState)
        {
            fireMissileOculus = true;
        }
        
        

        // Obtener el estado del gatillo del controlador izquierdo
        deviceLeft = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        deviceLeft.TryGetFeatureValue(CommonUsages.triggerButton, out fireBulletOculus);

        // Obtener el estado del gatillo del controlador derecho
        deviceRight = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        deviceRight.TryGetFeatureValue(CommonUsages.triggerButton, out fireMissileOculus);

        
        //Debug.Log("ShootingScript:Velocidad del avión: " + airplaneRigidbody.velocity);
        if ((fireBulletKeyboard || fireBulletOculus) && canDoRafaga)
        {
            canDoRafaga = false;
            AudioManager.Instance.Play("metralletaLargo");
            if (ShootButtonOnCabine_01 != null)
            {
                Renderer renderer = ShootButtonOnCabine_01.GetComponent<Renderer>();
                renderer.material = pressedMaterial;
            }
            /*
             GameObject bullet = PoolManager.Instance.Spawn(bulletPrefab, bulletSpawnPoint.position,
                Quaternion.LookRotation(bulletSpawnPoint.forward, Vector3.up));
            MoveForward bulletScript = bullet.GetComponent<MoveForward>();
            bulletScript.ActivateBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, airplaneRigidbody.velocity, true);
             */
            
            StartCoroutine(ShootBurst());
            StartCoroutine(RestoreOriginalMaterial(ShootButtonOnCabine_01, 0.5f));
        }
       
        if (fireMissileKeyboard || fireMissileOculus)
        {
            if (ShootButtonOnCabine_02 != null)
            {
                Renderer renderer = ShootButtonOnCabine_02.GetComponent<Renderer>();
                renderer.material = pressedMaterial;
            }

            GameObject missile = PoolManager.Instance.Spawn(missilePrefab, missileSpawnPoint.position,
                Quaternion.LookRotation(missileSpawnPoint.forward, Vector3.up));

            if (missile != null)
            {
                MoveForward missileScript = missile.GetComponent<MoveForward>();
                if (missileScript != null)
                {
                    Vector3 initialVelocity = airplaneRigidbody.velocity * missileInitialSpeedMultiplier;
                    missileScript.ActivateBullet(missileSpawnPoint.position, missileSpawnPoint.rotation, initialVelocity,false);
                    StartCoroutine(AccelerateMissile(missileScript,missile.transform, timeToAccelerate));
                }
                else
                {
                    Debug.LogError("El componente MoveForward no se encuentra en el prefab del misil.");
                }
            }
            else
            {
                Debug.LogError("El misil no se pudo instanciar.");
            }

            StartCoroutine(RestoreOriginalMaterial(ShootButtonOnCabine_02, 0.5f));
        }
    }

    IEnumerator ShootBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            GameObject bullet = PoolManager.Instance.Spawn(bulletPrefab, bulletSpawnPoint.position,
                Quaternion.LookRotation(bulletSpawnPoint.forward, Vector3.up));
            MoveForward bulletScript = bullet.GetComponent<MoveForward>();
            bulletScript.ActivateBullet(bulletSpawnPoint.position, bulletSpawnPoint.rotation, airplaneRigidbody.velocity, true);

            
            yield return new WaitForSeconds(timeBetweenBursts);
        }
        canDoRafaga = true;
    }
    IEnumerator AccelerateMissile(MoveForward missileScript, Transform missileTransform, float delay)
    {

        yield return new WaitForSeconds(delay); 
        
        missileScript.SetSpeedMultiplier(missileAcceleration);
        showVFX_Humo(missileTransform,"SmokeFXHumo"); 

        yield return new WaitForSeconds(delay);
        
        Transform humo = missileTransform.Find("SmokeFXHumo");
        if (humo != null)
        {
            humo.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("No se encontró el hijo 'SmokeFXHumo'");
        }
        
        
        missileScript.SetSpeedMultiplier(missileAcceleration);

    }

    IEnumerator RestoreOriginalMaterial(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (button != null)
        {
            Renderer renderer = button.GetComponent<Renderer>();
            renderer.material = originalMaterial;
        }
    }


    private void showVFX_Humo(Transform proyectil,string VFX)
    {
        Transform humo = proyectil.Find(VFX);
        if (humo != null)
        {
            humo.gameObject.SetActive(true);
            humo.GetComponent<ParticleSystem>().Play();
            Debug.Log("Activando efecto" + VFX );
        }
        else
        {
            Debug.Log("No se encontró" + VFX );
        }
    }

}
