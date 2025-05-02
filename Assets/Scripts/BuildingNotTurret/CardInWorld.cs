using UnityEngine; 
using CardEnum;

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
               SetUpSound();
          }
     }


     private int folCapacity;
     private int folCount;
     private int inCount;
     [SerializeField]private Collider2D triggerCollider;
     [SerializeField]private AudioSource audioSource;
     [SerializeField]private AudioClip tree,cannon, ballista, stoneHenge;
     private void Start()
     { 
          spriteRenderer = spriteRenderer ? spriteRenderer : GetComponent<SpriteRenderer>();
          triggerCollider = triggerCollider ? triggerCollider : GetComponent<Collider2D>();
          ChooseFollower();
          
     }

     private void SetUpSound()
     {
          if (!cardData)
          {
               Debug.LogError("Card Data Not Found");
               return;
          }
          switch (cardData.cardType)
          {
               case TypeEnum.CardType.tree:
                    audioSource.clip = tree;
                    break;
               case TypeEnum.CardType.ballista:
                    audioSource.clip = ballista;
                    break;
               case TypeEnum.CardType.cannon:
                    audioSource.clip = cannon;
                    break;
               case TypeEnum.CardType.stuck:
                    audioSource.clip = stoneHenge;
                    break;
               default:
                    Debug.LogError("Unknown card type");
                    break;
          }
          audioSource.Play();
     }
     
     private void GetFollowerToCome(FollowerAI follower)
     {
          if (folCount >= folCapacity) {SpawnBuilding();return;}
          if (!follower.target) return;
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
                switch(cardData.attributeType) 
                {
                     case TypeEnum.AttributeType.Attack:
                          GameManager.instance.PlayerDamage -= cardData.cardCost;
                          break;
                     case TypeEnum.AttributeType.Health:
                          tempBuilding.GetComponent<TreeBuilding>().BuildingCard(cardData);
                          GameManager.instance.PlayerHealth -= cardData.cardCost;
                          break;
                     case TypeEnum.AttributeType.AttackSpeed:
                          GameManager.instance.PlayerAttackSpeed -= cardData.cardCost;
                          break;
                     default:
                          Debug.LogError("Unknown attribute type");
                          break;
                }
                GameManager.instance.Pop(cardData.attributeType,cardData.cardCost);
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
               if (!follower)
               {
                    continue;
               }
               GetFollowerToCome(follower);
          }
     }
     
     private void OnFollowerDeath()
     {
          folCount--;
          ChooseFollower();
     } 
}
