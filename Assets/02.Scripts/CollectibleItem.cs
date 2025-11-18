using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemData;
    public int pointValue = 10;

    [Header("Visual Effects")]
    public float rotationSpeed = 50f;
    public bool useFloatingAnimation = true;
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;

    [Header("Collect Effects")]
    public GameObject shatterEffectPrefab;
    public bool useShatterEffect = true;

    [Header("Audio")]
    public AudioClip collectSound;
    public AudioClip shatterSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    private Vector3 startPosition;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private bool isCollected = false;

    void Start()
    {
        startPosition = transform.position;
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (itemData != null)
        {
            pointValue = itemData.pointValue;
            rotationSpeed = itemData.rotationSpeed;
            useFloatingAnimation = itemData.useFloatingAnimation;
            floatAmplitude = itemData.floatAmplitude;
            floatSpeed = itemData.floatSpeed;

            if (meshRenderer != null)
            {
                meshRenderer.material.color = itemData.itemColor;
            }

            if (itemData.collectSound != null)
            {
                collectSound = itemData.collectSound;
            }

            if (itemData.shatterEffect != null)
            {
                shatterEffectPrefab = itemData.shatterEffect;
            }
        }

        UpdateColorBasedOnProgress();
    }

    void Update()
    {
        if (isCollected)
            return;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        if (useFloatingAnimation)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected)
            return;

        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        isCollected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointValue);
        }

        if (useShatterEffect)
        {
            SpawnShatterEffect();
        }
        else
        {
            SpawnCollectEffect();
        }

        PlayCollectSound();

        ReturnToPool();
    }

    void SpawnShatterEffect()
    {
        if (shatterEffectPrefab != null)
        {
            GameObject effect = Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null && meshRenderer != null)
            {
                var main = ps.main;
                main.startColor = meshRenderer.material.color;
            }
        }

        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position, soundVolume);
        }
    }

    void SpawnCollectEffect()
    {
        if (itemData != null && itemData.collectEffect != null)
        {
            GameObject effect = Instantiate(itemData.collectEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    void PlayCollectSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayItemCollect();
        }
        else if (collectSound != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(collectSound, soundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
            }
        }
    }

    void ReturnToPool()
    {
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null)
        {
            spawner.ReturnItemToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void UpdateColorBasedOnProgress()
    {
        if (GameManager.Instance == null || meshRenderer == null)
            return;

        int collected = GameManager.Instance.GetItemsCollected();
        int total = GameManager.Instance.GetTotalItems();

        if (total > 0)
        {
            float progress = (float)collected / total;
            Color newColor = Color.Lerp(Color.yellow, Color.cyan, progress);
            meshRenderer.material.color = newColor;
        }
    }

    public void ResetItem(Vector3 newPosition)
    {
        isCollected = false;
        transform.position = newPosition;
        startPosition = newPosition;
        gameObject.SetActive(true);
        UpdateColorBasedOnProgress();
    }
}