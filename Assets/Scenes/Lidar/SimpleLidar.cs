using UnityEngine;
using System.Collections.Generic;

public class SimpleLidar : MonoBehaviour 
{
    [Header("Префаб и дистанция")]
    public GameObject pointPrefab;
    public float maxDistance = 50f;
    public float pointLifetime = 5f;
    
    [Header("Градиент глубины")]
    public Gradient depthGradient;
    public float emissionIntensity = 5f;
    
    [Header("Количество точек")]
    public int randomScanPoints = 50;
    public int sphereScanPoints = 500;
    public int circleScanStep = 2;
    public int coneScanPoints = 300;
    public int gridScanSize = 10;
    
    [Header("Object Pool")]
    public int poolSize = 200;
    
    private Queue<GameObject> pointPool = new Queue<GameObject>();
    private List<PooledPoint> activePoints = new List<PooledPoint>();
    
    private class PooledPoint
    {
        public GameObject obj;
        public float spawnTime;
    }
    
    void Start()
    {
        // Создаём градиент по умолчанию если не настроен
        if (depthGradient == null || depthGradient.colorKeys.Length == 0)
        {
            depthGradient = new Gradient();
            GradientColorKey[] colors = new GradientColorKey[2];
            colors[0] = new GradientColorKey(Color.white, 0f); // Близко
            colors[1] = new GradientColorKey(Color.cyan, 1f);  // Далеко
            
            GradientAlphaKey[] alphas = new GradientAlphaKey[2];
            alphas[0] = new GradientAlphaKey(1f, 0f);
            alphas[1] = new GradientAlphaKey(1f, 1f);
            
            depthGradient.SetKeys(colors, alphas);
        }
        
        // Создаём пул
        for (int i = 0; i < poolSize; i++)
        {
            GameObject point = Instantiate(pointPrefab);
            point.SetActive(false);
            pointPool.Enqueue(point);
        }
    }
    
    void Update() 
    {
        ScanRandom(randomScanPoints);
        
        // Убираем старые точки
        for (int i = activePoints.Count - 1; i >= 0; i--)
        {
            if (Time.time - activePoints[i].spawnTime > pointLifetime)
            {
                ReturnToPool(activePoints[i].obj);
                activePoints.RemoveAt(i);
            }
        }
    }
    
    GameObject GetFromPool()
    {
        if (pointPool.Count > 0)
        {
            GameObject point = pointPool.Dequeue();
            point.SetActive(true);
            return point;
        }
        
        // Если пул пуст, создаём новый
        return Instantiate(pointPrefab);
    }
    
    void ReturnToPool(GameObject point)
    {
        point.SetActive(false);
        pointPool.Enqueue(point);
    }
    
    public void ScanRandom(int rayCount) 
    {
        for (int i = 0; i < rayCount; i++) 
        {
            Vector3 dir = Random.onUnitSphere;
            ShootRay(dir);
        }
    }
    
    [ContextMenu("ScanSphere")]
    public void ScanSphere() 
    {
        for (int i = 0; i < sphereScanPoints; i++) 
        {
            float y = 1 - (i / (float)sphereScanPoints) * 2;
            float radius = Mathf.Sqrt(1 - y * y);
            float theta = i * Mathf.PI * (3 - Mathf.Sqrt(5));
            
            float x = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * radius;
            
            ShootRay(new Vector3(x, y, z));
        }
    }
    
    [ContextMenu("ScanCircle")]
    public void ScanCircle() 
    {
        for (int i = 0; i < 360; i += circleScanStep) 
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            ShootRay(dir);
        }
    }
    
    [ContextMenu("ScanCone")]
    public void ScanCone() 
    {
        for (int i = 0; i < coneScanPoints; i++) 
        {
            Vector3 dir = Random.onUnitSphere;
            if (Vector3.Dot(dir, transform.forward) > 0.5f) 
            {
                ShootRay(dir);
            }
        }
    }
    
    [ContextMenu("ScanGrid")]
    public void ScanGrid() 
    {
        for (int x = -gridScanSize; x <= gridScanSize; x++) 
        {
            for (int y = -gridScanSize; y <= gridScanSize; y++) 
            {
                Vector3 dir = new Vector3(x, y, 10).normalized;
                ShootRay(dir);
            }
        }
    }
    
    void ShootRay(Vector3 direction) 
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxDistance)) 
        {
            GameObject point = GetFromPool();
            point.transform.position = hit.point;
            
            // Нормализованное расстояние (0 = близко, 1 = далеко)
            float normalizedDistance = hit.distance / maxDistance;
            
            // Получаем цвет из градиента
            Color gradientColor = depthGradient.Evaluate(normalizedDistance);
            
            // Применяем эмиссию
            Renderer renderer = point.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null) 
            {
                Material mat = renderer.material;
                mat.EnableKeyword("_EMISSION");
                
                // Эмиссия с учётом градиента и интенсивности
                Color emissionColor = gradientColor * emissionIntensity;
                mat.SetColor("_EmissionColor", emissionColor);
                mat.color = gradientColor;
            }
            
            // Добавляем в активные
            activePoints.Add(new PooledPoint { obj = point, spawnTime = Time.time });
        }
    }
}