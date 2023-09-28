using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private bool canTrigger = true; // Booleano para controlar si podemos activar OnTriggerEnter
    private float triggerCooldown = 2.0f; // Tiempo en segundos durante el cual no podemos volver a activar OnTriggerEnter

    private void OnTriggerEnter(Collider other)
    {
        if (canTrigger && other.gameObject.CompareTag("Ring")) // Asegúrate de que los aros tengan la etiqueta "Ring"
        {
            UIManager.Instance.ShowSuccessText();
            // Ocultar el aro con el que colisionamos
            other.gameObject.SetActive(false);

            // Incrementar la puntuación
            UIManager.Instance.UpdateScore(1);
            UIManager.Instance.UpdateArosPasadosTotales(1);

            // Notificar al ObjectiveManager que un objetivo ha sido destruido
            ObjectiveManager.Instance.ObjectiveDestroyed();

            // Mostrar el siguiente aro
            ObjectiveManager.Instance.ActivateNextRing();

            // Desactivar el trigger durante un tiempo
            canTrigger = false;
            Invoke("ResetTrigger", triggerCooldown);
        }
    }

    private void ResetTrigger()
    {
        canTrigger = true;
    }
}