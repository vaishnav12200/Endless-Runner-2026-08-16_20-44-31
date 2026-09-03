using UnityEngine;

public class EndlessTrackLooper : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform[] sections;
    [SerializeField, Min(1f)] float sectionLength = 40f;
    [SerializeField, Min(0f)] float recycleBehindDistance = 5f;

    public int RecycleCount { get; private set; }

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (player == null || sections == null || sections.Length == 0)
        {
            return;
        }

        float cycleLength = sectionLength * sections.Length;
        float halfSectionLength = sectionLength * 0.5f;

        foreach (Transform section in sections)
        {
            if (section == null)
            {
                continue;
            }

            while (player.position.z > section.position.z + halfSectionLength + recycleBehindDistance)
            {
                section.position += Vector3.forward * cycleLength;
                ResetSection(section);
                RecycleCount++;
            }
        }
    }

    static void ResetSection(Transform section)
    {
        foreach (CoinWork coin in section.GetComponentsInChildren<CoinWork>(true))
        {
            coin.ResetForReuse();
        }

        foreach (CollisionReset obstacle in section.GetComponentsInChildren<CollisionReset>(true))
        {
            obstacle.ResetForReuse();
        }
    }
}
