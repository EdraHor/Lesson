using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour
{
    [Header("Куб настройки")]
    public Transform spawnTransform;
    
    [Header("Вращение")]
    public Vector3 rotationSpeed = new Vector3(0, 90, 0);
    
    [Header("Эффекты")]
    public ParticleSystem effectsParent;
    public float destroyDelay = 2f;
    
    [Header("Пистолет")]
    public Transform gunTransform;
    public float gunRiseHeight = 2f;
    public float gunRiseSpeed = 5f;
    public float gunLowerSpeed = 3f;
    public float gunHoldDelay = 0.5f; // Задержка после подъема
    
    [Header("Отдача пистолета")]
    public Vector3 recoilOffset = new Vector3(0, -0.2f, -0.3f);
    public float recoilDuration = 0.1f;
    public float recoilRecoverySpeed = 8f;
    
    [Header("Встряска камеры")]
    public Camera targetCamera;
    public float shakeIntensity = 0.5f;
    public float shakeDuration = 0.3f;
    public float shakeFrequency = 30f;
    [Range(0f, 1f)]
    public float shakeFadeInTime = 0.1f; // Время плавного нарастания тряски
    [Range(0f, 1f)]
    public float shakeFadeOutTime = 0.2f; // Время плавного затухания тряски
    
    private GameObject currentCube;
    private bool isRotating = false;
    private Vector3 gunOriginalPosition;
    private Vector3 cameraOriginalPosition;
    private bool isShootingSequenceActive = false;
    
    void Start()
    {
        if (gunTransform != null)
        {
            gunOriginalPosition = gunTransform.localPosition;
        }
        
        if (targetCamera != null)
        {
            cameraOriginalPosition = targetCamera.transform.localPosition;
        }
    }
    
    void Update()
    {
        HandleInput();
        
        if (isRotating && currentCube != null)
        {
            RotateCube();
        }
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CreateCube();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleRotation();
        }
        
        if (Input.GetKeyDown(KeyCode.F3) && !isShootingSequenceActive)
        {
            StartCoroutine(ShootingSequence());
        }
    }
    
    void CreateCube()
    {
        if (currentCube != null)
        {
            DestroyImmediate(currentCube);
        }
        
        currentCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        if (spawnTransform != null)
        {
            currentCube.transform.position = spawnTransform.position;
            currentCube.transform.rotation = spawnTransform.rotation;
            currentCube.transform.localScale = spawnTransform.localScale;
        }
        else
        {
            currentCube.transform.position = transform.position;
        }
        
        currentCube.AddComponent<LifecycleDemo>();
        isRotating = false;
    }
    
    void ToggleRotation()
    {
        if (currentCube == null)
        {
            return;
        }
        
        isRotating = !isRotating;
    }
    
    void RotateCube()
    {
        currentCube.transform.Rotate(rotationSpeed * Time.deltaTime);
    }
    
    IEnumerator ShootingSequence()
    {
        if (currentCube == null)
        {
            yield break;
        }
        
        isShootingSequenceActive = true;
        
        // 1. Поднимаем пистолет
        yield return StartCoroutine(RaiseGun());
        
        // 2. Задержка после подъема
        yield return new WaitForSeconds(gunHoldDelay);
        
        // 3. Эмулируем отдачу
        yield return StartCoroutine(GunRecoil());
        
        // 4. Встряска камеры с плавностью
        StartCoroutine(ShakeCameraSmooth());
        
        // 5. Запускаем эффекты
        StartEffects();
        
        // 6. Опускаем пистолет
        StartCoroutine(LowerGun());
        
        // 7. Удаляем куб с задержкой
        yield return StartCoroutine(DestroyCubeAfterDelay());
        
        isShootingSequenceActive = false;
    }
    
    IEnumerator RaiseGun()
    {
        if (gunTransform == null) yield break;
        
        Vector3 targetPosition = gunOriginalPosition + Vector3.up * gunRiseHeight;
        
        while (Vector3.Distance(gunTransform.localPosition, targetPosition) > 0.01f)
        {
            gunTransform.localPosition = Vector3.MoveTowards(
                gunTransform.localPosition, 
                targetPosition, 
                gunRiseSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        gunTransform.localPosition = targetPosition;
    }
    
    IEnumerator LowerGun()
    {
        if (gunTransform == null) yield break;
        
        while (Vector3.Distance(gunTransform.localPosition, gunOriginalPosition) > 0.01f)
        {
            gunTransform.localPosition = Vector3.MoveTowards(
                gunTransform.localPosition, 
                gunOriginalPosition, 
                gunLowerSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        gunTransform.localPosition = gunOriginalPosition;
    }
    
    IEnumerator GunRecoil()
    {
        if (gunTransform == null) yield break;
        
        Vector3 originalPos = gunTransform.localPosition;
        Vector3 recoilPos = originalPos + recoilOffset;
        
        // Быстрое движение в позицию отдачи
        float elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            float t = elapsed / recoilDuration;
            gunTransform.localPosition = Vector3.Lerp(originalPos, recoilPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Возврат в исходную позицию
        while (Vector3.Distance(gunTransform.localPosition, originalPos) > 0.01f)
        {
            gunTransform.localPosition = Vector3.MoveTowards(
                gunTransform.localPosition, 
                originalPos, 
                recoilRecoverySpeed * Time.deltaTime
            );
            yield return null;
        }
        
        gunTransform.localPosition = originalPos;
    }
    
    IEnumerator ShakeCameraSmooth()
    {
        if (targetCamera == null) yield break;
        
        float elapsed = 0f;
        float intensityMultiplier = 1f;
        
        while (elapsed < shakeDuration)
        {
            // Вычисляем множитель интенсивности для плавности
            if (elapsed < shakeFadeInTime)
            {
                // Плавное нарастание
                intensityMultiplier = elapsed / shakeFadeInTime;
            }
            else if (elapsed > shakeDuration - shakeFadeOutTime)
            {
                // Плавное затухание
                float fadeOutProgress = (shakeDuration - elapsed) / shakeFadeOutTime;
                intensityMultiplier = fadeOutProgress;
            }
            else
            {
                // Полная интенсивность
                intensityMultiplier = 1f;
            }
            
            // Применяем тряску с учетом множителя
            float currentIntensity = shakeIntensity * intensityMultiplier;
            float x = Random.Range(-1f, 1f) * currentIntensity;
            float y = Random.Range(-1f, 1f) * currentIntensity;
            
            targetCamera.transform.localPosition = cameraOriginalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(1f / shakeFrequency);
        }
        
        // Возвращаем камеру в исходную позицию
        targetCamera.transform.localPosition = cameraOriginalPosition;
    }
    
    void StartEffects()
    {
        if (effectsParent != null && currentCube != null)
        {
            effectsParent.transform.position = currentCube.transform.position;
            effectsParent.Play();
            
            ParticleSystem[] childEffects = effectsParent.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem effect in childEffects)
            {
                effect.Play();
            }
        }
    }
    
    IEnumerator DestroyCubeAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        
        if (currentCube != null)
        {
            Destroy(currentCube);
            currentCube = null;
            isRotating = false;
        }
    }
    
    void OnValidate()
    {
        if (destroyDelay < 0)
            destroyDelay = 0;
        if (gunRiseHeight < 0)
            gunRiseHeight = 0;
        if (gunRiseSpeed < 0.1f)
            gunRiseSpeed = 0.1f;
        if (gunLowerSpeed < 0.1f)
            gunLowerSpeed = 0.1f;
        if (gunHoldDelay < 0)
            gunHoldDelay = 0;
        if (recoilDuration < 0.01f)
            recoilDuration = 0.01f;
        if (shakeDuration < 0)
            shakeDuration = 0;
        if (shakeIntensity < 0)
            shakeIntensity = 0;
        if (shakeFrequency < 1)
            shakeFrequency = 1;
        
        // Проверяем что время fade не превышает общую длительность тряски
        float maxFadeTime = shakeDuration / 2f;
        if (shakeFadeInTime > maxFadeTime)
            shakeFadeInTime = maxFadeTime;
        if (shakeFadeOutTime > maxFadeTime)
            shakeFadeOutTime = maxFadeTime;
    }
}