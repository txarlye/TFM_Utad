using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCanvas : MonoBehaviour
{
   public GameObject objectToWorkWith;

   public void ShowGameObject()
   {
      objectToWorkWith.SetActive(true);
   }

   public void HideGameObject(int seconds)
   {
      StartCoroutine(HideGameObjectForXSeconds(seconds));
   }

   private IEnumerator HideGameObjectForXSeconds(int seconds)
   {
      yield return new WaitForSeconds(seconds);
      objectToWorkWith.SetActive(false);
   }
}