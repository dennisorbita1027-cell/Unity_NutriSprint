using UnityEngine;

public enum CollectibleType { Gulay, Prutas, PagkaingKarne, GoFood, JunkFood }

public class Collectible : MonoBehaviour
{
    public CollectibleType type;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int amount = (type == CollectibleType.JunkFood)
                ? -GameManager.Instance.junkFoodPenalty
                : GameManager.Instance.goodFoodScore;
            GameManager.Instance.ShowScoreModifier(amount);

            GameManager.Instance.RecordCollectible(type);

            if (AudioManager.Instance != null)
            {
                if (type == CollectibleType.JunkFood)
                    AudioManager.Instance.PlayJunkCollect();
                else
                    AudioManager.Instance.PlayGoodCollect();
            }

            Destroy(gameObject);
        }
    }
}
