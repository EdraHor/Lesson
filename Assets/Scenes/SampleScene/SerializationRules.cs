using UnityEngine;

public class SerializationRules : MonoBehaviour
{
    // ✅ Будут показаны в Inspector:
    public int PublicInt = 10;
    public float PublicFloat = 5.5f;
    public string PublicString = "Hello";
    public GameObject PublicGameObject;

    [SerializeField] // - Так выглядит атрибут
    private int SerializedPrivate = 20;

    // ❌ НЕ будут показаны в Inspector:
    private int PrivateInt = 30;
    public static int StaticVariable = 100;
}