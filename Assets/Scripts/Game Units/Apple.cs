using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Apple : MonoBehaviour
{
   [SerializeField] private GameObject slisedApple;

   [SerializeField] private List<Rigidbody2D> slisedApplesPieces;

   private void OnTriggerEnter2D(Collider2D col)
   {
      //!GameManager.Instance.IsWaitNewStage()
      if (col.TryGetComponent(out Knife knife))
      {
         if (!knife.isAfterLogDestroy)
         {
            sliseApple();
            GameManager.Instance.AddApple();
         }
      }
   }

   private void sliseApple()
   {
      GetComponentInChildren<SpriteRenderer>().enabled = false;
      slisedApple.SetActive(true);
      slisedApple.transform.parent = null;
      for (int i = 0; i < slisedApplesPieces.Count; i++)
      {
         slisedApplesPieces[i].transform.parent = null;
         Vector3 forceDir;
         if (Random.Range(0, 1) == 0)
         {
            forceDir= (slisedApplesPieces[i].transform.position - Vector3.right).normalized * 4;
         }
         else
         {
            forceDir= (slisedApplesPieces[i].transform.position - Vector3.left).normalized * 4;
         }
         slisedApplesPieces[i].AddForceAtPosition(forceDir,slisedApplesPieces[i].transform.position,ForceMode2D.Impulse);
         slisedApplesPieces[i].AddTorque(4,ForceMode2D.Impulse);
         Destroy(slisedApplesPieces[i],2.0f);
      }
      Destroy(slisedApple);
      Destroy(gameObject);
   }
}
