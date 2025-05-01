using System;
using Unity.VisualScripting;
using UnityEngine;

public class CardInWorld : MonoBehaviour
{
     private Card cardData;
     [SerializeField] private SpriteRenderer spriteRenderer;
     public Card CardData
     {
          get => cardData;
          set
          {
               cardData = value;
               spriteRenderer.sprite = cardData.wholeCardSprite;
               folCapacity = cardData.followersCapacity;
          }
     }


     private int folCapacity;
     private int folCount;
     private int inCount;
     [SerializeField]private Collider2D triggerCollider;

     private void Start()
     {
           
          spriteRenderer = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();
          triggerCollider = triggerCollider ? triggerCollider : GetComponent<Collider2D>();
          ChooseFollower();
     }

     
     private void GetFollowerToCome(FollowerAI follower)
     {
          if (folCount >= folCapacity) {SpawnBuilding();return;} 
          if (follower.target == transform) return;
          folCount++;
          follower.DetachFollower();
          follower.OnDeath += OnFollowerDeath; 
          follower.target =  transform;
     }

     private void SpawnBuilding()
     {
          if (Instantiate(cardData.cardBuildingPrefab, transform.position, Quaternion.identity)
                .TryGetComponent(out Building tempBuilding))
            {
                tempBuilding.LoadCardData?.Invoke(cardData);
            }
          Destroy(gameObject);
     }
     private void OnTriggerEnter2D(Collider2D collider2D)
     {
          if (collider2D.CompareTag("Human"))
          {
               Destroy(collider2D.gameObject);
               inCount++;
               if (inCount >= folCapacity) SpawnBuilding();
          }
     }

     private void ChooseFollower()
     {
          if(!GameManager.instance.HumanFollowerTail) return;
          if(folCount >= folCapacity) return;
          FollowerAI[] followers = new FollowerAI[folCapacity-folCount];
          float[] distances = new float[folCapacity - folCount];
          for (int i = 0; i < folCapacity-folCount; i++)
          {
               distances[i]= float.MaxValue;
          }
          FollowerAI followerTail = GameManager.instance.HumanFollowerTail?.GetComponent<FollowerAI>();
          bool foundNext = false;
          if (!followerTail) return;
          while(followerTail)
          {
               for (int i = 0; i < folCapacity-folCount; i++)
               {
                    if (distances[i] > Vector2.SqrMagnitude(followerTail.transform.position - transform.position))
                    {
                         for (int j =folCapacity-folCount-1; j > i; j--)
                         { 
                              distances[j] = distances[j-1];//shift to right
                              followers[j] = followers[j-1];
                         }
                         distances[i] = Vector2.SqrMagnitude(followerTail.transform.position - transform.position);
                         
                         followers[i] = followerTail;
                         
                         if (followerTail.target.TryGetComponent(out FollowerAI targetFollower))
                         {
                              followerTail = targetFollower;
                              foundNext = true;
                         }
                         break;
                    }
               }

               if (foundNext)
               {
                    foundNext = false;
                    continue;
               }

               if (followerTail.target.TryGetComponent(out FollowerAI _targetFollower))
               {
                    followerTail = _targetFollower;
                    continue;
               }
               followerTail = null;
          }

          foreach (var follower in followers)
          {
               GetFollowerToCome(follower);
          }
     }
     
     private void OnFollowerDeath()
     {
          folCount--;
          ChooseFollower();
     } 
}
