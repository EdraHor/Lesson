using UnityEngine;

public class RealtimeSpectrogram : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    
    [Header("Spectrogram Settings")]
    [SerializeField] private int spectrumSize = 256; // Уменьшил для стабильности
    [SerializeField] private int spectrogramHeight = 128;
    [SerializeField] private float intensityMultiplier = 10f; // Уменьшил значение
    [SerializeField] private bool useLogarithmicScale = true;
    
    [Header("Output")]
    [SerializeField] private Renderer targetRenderer;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGUI = true;
    [SerializeField] private Vector2 debugPosition = new Vector2(10, 10);
    [SerializeField] private Vector2 debugSize = new Vector2(512, 256);
    
    private Texture2D spectrogramTexture;
    private float[] spectrumData;
    private Color[] pixels;
    private bool isInitialized = false;

    void Start()
    {
        InitializeSpectrogram();
    }

    void InitializeSpectrogram()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found!");
            return;
        }

        // Инициализация
        spectrumData = new float[spectrumSize];
        
        spectrogramTexture = new Texture2D(spectrumSize, spectrogramHeight, TextureFormat.RGB24, false);
        spectrogramTexture.wrapMode = TextureWrapMode.Clamp;
        spectrogramTexture.filterMode = FilterMode.Point; // Point для четкости
        
        // Инициализация массива пикселей
        pixels = new Color[spectrumSize * spectrogramHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }
        
        spectrogramTexture.SetPixels(pixels);
        spectrogramTexture.Apply();
        
        if (targetRenderer != null)
        {
            targetRenderer.material.mainTexture = spectrogramTexture;
        }

        isInitialized = true;
        Debug.Log($"Spectrogram initialized: {spectrumSize}x{spectrogramHeight}");
    }

    void Update()
    {
        if (!isInitialized || audioSource == null)
            return;

        // Получаем спектральные данные
        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);
        
        // Сдвигаем все пиксели вниз на одну строку
        for (int y = spectrogramHeight - 1; y > 0; y--)
        {
            for (int x = 0; x < spectrumSize; x++)
            {
                int currentIndex = y * spectrumSize + x;
                int previousIndex = (y - 1) * spectrumSize + x;
                pixels[currentIndex] = pixels[previousIndex];
            }
        }
        
        // Добавляем новую строку сверху (y = 0)
        for (int x = 0; x < spectrumSize; x++)
        {
            float value = spectrumData[x];
            
            // Логарифмическая шкала (опционально)
            if (useLogarithmicScale && value > 0)
            {
                value = Mathf.Log10(value * 100 + 1) / 2f; // Логарифм для лучшей видимости
            }
            
            float intensity = value * intensityMultiplier;
            intensity = Mathf.Clamp01(intensity);
            
            // Простая цветовая схема: черный -> синий -> голубой -> желтый -> красный -> белый
            Color color = GetSpectrogramColor(intensity);
            
            pixels[x] = color; // Первая строка (y=0)
        }
        
        // Применяем изменения
        spectrogramTexture.SetPixels(pixels);
        spectrogramTexture.Apply();
    }

    Color GetSpectrogramColor(float intensity)
    {
        // Классическая палитра спектрограммы
        if (intensity < 0.2f)
        {
            // Черный -> Темно-синий
            return Color.Lerp(Color.black, new Color(0, 0, 0.5f), intensity * 5f);
        }
        else if (intensity < 0.4f)
        {
            // Темно-синий -> Синий
            return Color.Lerp(new Color(0, 0, 0.5f), Color.blue, (intensity - 0.2f) * 5f);
        }
        else if (intensity < 0.6f)
        {
            // Синий -> Голубой
            return Color.Lerp(Color.blue, Color.cyan, (intensity - 0.4f) * 5f);
        }
        else if (intensity < 0.8f)
        {
            // Голубой -> Желтый
            return Color.Lerp(Color.cyan, Color.yellow, (intensity - 0.6f) * 5f);
        }
        else
        {
            // Желтый -> Красный -> Белый
            if (intensity < 0.9f)
                return Color.Lerp(Color.yellow, Color.red, (intensity - 0.8f) * 10f);
            else
                return Color.Lerp(Color.red, Color.white, (intensity - 0.9f) * 10f);
        }
    }

    void OnGUI()
    {
        if (!showDebugGUI || spectrogramTexture == null)
            return;

        GUILayout.BeginArea(new Rect(debugPosition.x, debugPosition.y, debugSize.x + 20, debugSize.y + 200));
        
        // Фон
        GUI.Box(new Rect(0, 0, debugSize.x + 10, debugSize.y + 10), "");
        
        // Спектрограмма
        GUI.DrawTexture(new Rect(5, 5, debugSize.x, debugSize.y), spectrogramTexture);
        
        GUILayout.BeginArea(new Rect(0, debugSize.y + 15, debugSize.x + 10, 180));
        
        // Информация
        GUILayout.Label("=== SPECTROGRAM DEBUG ===");
        if (audioSource != null)
        {
            GUILayout.Label($"Audio Playing: {audioSource.isPlaying}");
            if (audioSource.clip != null)
            {
                GUILayout.Label($"Clip: {audioSource.clip.name}");
                GUILayout.Label($"Time: {audioSource.time:F2}s / {audioSource.clip.length:F2}s");
            }
            
            // Статистика спектра
            float minVal = float.MaxValue;
            float maxVal = float.MinValue;
            float avgVal = 0f;
            
            for (int i = 0; i < spectrumData.Length; i++)
            {
                float val = spectrumData[i];
                if (val < minVal) minVal = val;
                if (val > maxVal) maxVal = val;
                avgVal += val;
            }
            avgVal /= spectrumData.Length;
            
            GUILayout.Label($"Spectrum - Min: {minVal:F6}, Max: {maxVal:F6}, Avg: {avgVal:F6}");
            GUILayout.Label($"Multiplied Max: {(maxVal * intensityMultiplier):F3}");
        }
        
        GUILayout.EndArea();
        
        // Живой спектр
        DrawLiveSpectrum(new Rect(5, debugSize.y + 110, debugSize.x, 80));
        
        GUILayout.EndArea();
    }

    void DrawLiveSpectrum(Rect rect)
    {
        GUI.Box(rect, "");
        
        float barWidth = rect.width / spectrumSize;
        
        for (int i = 0; i < spectrumData.Length; i++)
        {
            float value = spectrumData[i];
            if (useLogarithmicScale && value > 0)
            {
                value = Mathf.Log10(value * 100 + 1) / 2f;
            }
            
            float height = value * intensityMultiplier * rect.height;
            height = Mathf.Clamp(height, 0, rect.height);
            
            float intensity = value * intensityMultiplier;
            intensity = Mathf.Clamp01(intensity);
            
            Color barColor = GetSpectrogramColor(intensity);
            
            GUI.color = barColor;
            GUI.DrawTexture(
                new Rect(rect.x + i * barWidth, rect.y + rect.height - height, barWidth, height),
                Texture2D.whiteTexture
            );
        }
        
        GUI.color = Color.white;
    }

    public Texture2D GetSpectrogramTexture()
    {
        return spectrogramTexture;
    }

    void OnDestroy()
    {
        if (spectrogramTexture != null)
        {
            Destroy(spectrogramTexture);
        }
    }
}