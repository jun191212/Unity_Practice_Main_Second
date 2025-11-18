using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName = "Item";
    public string description = "Collectible item";
    public Sprite icon;
    public int pointValue = 10;

    [Header("Visual Settings")]
    public Color itemColor = Color.yellow;
    public GameObject itemPrefab;

    [Header("Effects")]
    public GameObject collectEffect;
    public GameObject shatterEffect;
    public AudioClip collectSound;
    public AudioClip shatterSound;

    public float rotationSpeed = 50f;
    public bool useFloatingAnimation = true;
    public float floatAmplitude = 0.5f;
    public float floatSpeed = 2f;
}