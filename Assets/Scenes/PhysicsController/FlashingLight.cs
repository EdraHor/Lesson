using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    [Header("Flash Settings")]
    public float MinFlashTime = 0.1f; // Минимальное время мигания
    public float MaxFlashTime = 1.5f; // Максимальное время мигания
    
    [Header("Emission Settings")]
    public GameObject EmissionObject; // Объект с эмиссией
    public string EmissionProperty = "_EmissionColor"; // Имя свойства эмиссии

    private Light lightComponent;
    private Material emissionMaterial;
    private Color originalEmissionColor;
    private float nextFlashTime;
    private bool isLightOn = true;

    void Start()
    {
        lightComponent = GetComponent<Light>();
        
        // Получаем материал объекта с эмиссией
        if (EmissionObject != null)
        {
            Renderer renderer = EmissionObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                emissionMaterial = renderer.material; // Создаем экземпляр материала
                originalEmissionColor = emissionMaterial.GetColor(EmissionProperty);
            }
        }
        
        nextFlashTime = Time.time + Random.Range(MinFlashTime, MaxFlashTime);
    }

    void Update()
    {
        // Хаотичное мигание как в хорроре
        if (Time.time >= nextFlashTime)
        {
            isLightOn = !isLightOn;
            
            // Переключаем свет
            lightComponent.enabled = isLightOn;
            
            // Переключаем эмиссию
            if (emissionMaterial != null)
            {
                if (isLightOn)
                {
                    // Включаем эмиссию
                    emissionMaterial.SetColor(EmissionProperty, originalEmissionColor);
                    emissionMaterial.EnableKeyword("_EMISSION");
                }
                else
                {
                    // Выключаем эмиссию
                    emissionMaterial.SetColor(EmissionProperty, Color.black);
                    emissionMaterial.DisableKeyword("_EMISSION");
                }
            }
            
            nextFlashTime = Time.time + Random.Range(MinFlashTime, MaxFlashTime);
        }
    }

    void OnDestroy()
    {
        // Восстанавливаем оригинальный цвет при уничтожении
        if (emissionMaterial != null)
        {
            emissionMaterial.SetColor(EmissionProperty, originalEmissionColor);
        }
    }
}