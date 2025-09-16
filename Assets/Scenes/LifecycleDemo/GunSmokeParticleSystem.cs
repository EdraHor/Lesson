using UnityEngine;

public class GunSmokeParticleSystem : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private bool playOnStart = false;
    
    private ParticleSystem flashSystem;
    private ParticleSystem sparksSystem;
    private ParticleSystem shockwaveSystem;
    private ParticleSystem heatSystem;
    
    private Material additiveMaterial;
    private Material alphaMaterial;

    void Start()
    {
        if (playOnStart)
        {
            PlayMuzzleFlash();
        }
    }

    void CreateMuzzleFlashSystems()
    {
        // Создаем материалы
        CreateMaterials();
        
        // Создаем системы частиц
        CreateFlashSystem();
        CreateSparksSystem();
        CreateShockwaveSystem();
        CreateHeatSystem();
        
        Debug.Log("Muzzle Flash systems created successfully!");
    }

    void CreateMaterials()
    {
        // Additive материал для ярких эффектов
        additiveMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        additiveMaterial.SetFloat("_Surface", 1); // Transparent
        additiveMaterial.SetFloat("_Blend", 1); // Additive
        additiveMaterial.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        additiveMaterial.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        additiveMaterial.SetFloat("_ZWrite", 0);
        additiveMaterial.color = Color.white;
        additiveMaterial.renderQueue = 3000;
        
        // Alpha материал для ударной волны
        alphaMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        alphaMaterial.SetFloat("_Surface", 1); // Transparent
        alphaMaterial.SetFloat("_Blend", 0); // Alpha
        alphaMaterial.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        alphaMaterial.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        alphaMaterial.SetFloat("_ZWrite", 0);
        alphaMaterial.color = Color.white;
        alphaMaterial.renderQueue = 3000;
    }

    void CreateFlashSystem()
    {
        GameObject flashObj = new GameObject("MuzzleFlash");
        flashObj.transform.SetParent(transform);
        flashObj.transform.localPosition = Vector3.zero;
        
        flashSystem = flashObj.AddComponent<ParticleSystem>();
        var flashRenderer = flashObj.GetComponent<ParticleSystemRenderer>();
        flashRenderer.material = additiveMaterial;
        
        var main = flashSystem.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
        main.startColor = new Color(2f, 2f, 1f, 1f); // HDR белый-желтый
        main.maxParticles = 8;
        main.playOnAwake = false;
        main.loop = false;
        
        var emission = flashSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 5)
        });
        
        var shape = flashSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;
        
        var sizeOverLifetime = flashSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve flashSizeCurve = new AnimationCurve();
        flashSizeCurve.AddKey(0f, 0.1f);
        flashSizeCurve.AddKey(0.3f, 3f);
        flashSizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, flashSizeCurve);
        
        var colorOverLifetime = flashSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient flashGradient = new Gradient();
        GradientColorKey[] flashColorKeys = new GradientColorKey[] {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(Color.yellow, 0.3f),
            new GradientColorKey(new Color(1f, 0.5f, 0f), 0.7f),
            new GradientColorKey(Color.red, 1f)
        };
        GradientAlphaKey[] flashAlphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(0.8f, 0.5f),
            new GradientAlphaKey(0f, 1f)
        };
        flashGradient.SetKeys(flashColorKeys, flashAlphaKeys);
        colorOverLifetime.color = flashGradient;
    }

    void CreateSparksSystem()
    {
        GameObject sparksObj = new GameObject("Sparks");
        sparksObj.transform.SetParent(transform);
        sparksObj.transform.localPosition = Vector3.zero;
        
        sparksSystem = sparksObj.AddComponent<ParticleSystem>();
        var sparksRenderer = sparksObj.GetComponent<ParticleSystemRenderer>();
        sparksRenderer.material = additiveMaterial;
        
        var main = sparksSystem.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(8f, 15f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startColor = new Color(2f, 1.5f, 0.5f, 1f); // HDR оранжевый
        main.maxParticles = 30;
        main.playOnAwake = false;
        main.loop = false;
        main.gravityModifier = 0.5f;
        
        var emission = sparksSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 20)
        });
        
        var shape = sparksSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35f;
        shape.radius = 0.05f;
        shape.randomDirectionAmount = 0.3f;
        
        var velocityOverLifetime = sparksSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-5f);
        
        var sizeOverLifetime = sparksSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sparksSizeCurve = new AnimationCurve();
        sparksSizeCurve.AddKey(0f, 1f);
        sparksSizeCurve.AddKey(0.8f, 0.5f);
        sparksSizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sparksSizeCurve);
        
        var colorOverLifetime = sparksSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient sparksGradient = new Gradient();
        GradientColorKey[] sparksColorKeys = new GradientColorKey[] {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0.3f),
            new GradientColorKey(new Color(1f, 0.3f, 0f), 0.7f),
            new GradientColorKey(Color.black, 1f)
        };
        GradientAlphaKey[] sparksAlphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(0.8f, 0.5f),
            new GradientAlphaKey(0f, 1f)
        };
        sparksGradient.SetKeys(sparksColorKeys, sparksAlphaKeys);
        colorOverLifetime.color = sparksGradient;
    }

    void CreateShockwaveSystem()
    {
        GameObject shockwaveObj = new GameObject("Shockwave");
        shockwaveObj.transform.SetParent(transform);
        shockwaveObj.transform.localPosition = Vector3.zero;
        
        shockwaveSystem = shockwaveObj.AddComponent<ParticleSystem>();
        var shockwaveRenderer = shockwaveObj.GetComponent<ParticleSystemRenderer>();
        shockwaveRenderer.material = alphaMaterial;
        
        var main = shockwaveSystem.main;
        main.startLifetime = 0.2f;
        main.startSpeed = 0f;
        main.startSize = 0.1f;
        main.startColor = new Color(0.8f, 0.9f, 1f, 0.5f); // Бледно-голубой
        main.maxParticles = 2;
        main.playOnAwake = false;
        main.loop = false;
        
        var emission = shockwaveSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 1)
        });
        
        var shape = shockwaveSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;
        
        var sizeOverLifetime = shockwaveSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve shockwaveSizeCurve = new AnimationCurve();
        shockwaveSizeCurve.AddKey(0f, 0.1f);
        shockwaveSizeCurve.AddKey(0.8f, 5f);
        shockwaveSizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, shockwaveSizeCurve);
        
        var colorOverLifetime = shockwaveSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient shockwaveGradient = new Gradient();
        GradientColorKey[] shockwaveColorKeys = new GradientColorKey[] {
            new GradientColorKey(new Color(0.8f, 0.9f, 1f), 0f),
            new GradientColorKey(new Color(0.6f, 0.7f, 0.9f), 1f)
        };
        GradientAlphaKey[] shockwaveAlphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(0.5f, 0f),
            new GradientAlphaKey(0.3f, 0.5f),
            new GradientAlphaKey(0f, 1f)
        };
        shockwaveGradient.SetKeys(shockwaveColorKeys, shockwaveAlphaKeys);
        colorOverLifetime.color = shockwaveGradient;
    }

    void CreateHeatSystem()
    {
        GameObject heatObj = new GameObject("HeatWave");
        heatObj.transform.SetParent(transform);
        heatObj.transform.localPosition = Vector3.zero;
        
        heatSystem = heatObj.AddComponent<ParticleSystem>();
        var heatRenderer = heatObj.GetComponent<ParticleSystemRenderer>();
        heatRenderer.material = alphaMaterial;
        
        var main = heatSystem.main;
        main.startLifetime = 0.15f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
        main.startColor = new Color(0.9f, 0.9f, 0.8f, 0.3f); // Очень светло-серый
        main.maxParticles = 12;
        main.playOnAwake = false;
        main.loop = false;
        
        var emission = heatSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0.0f, 8)
        });
        
        var shape = heatSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.03f;
        
        var sizeOverLifetime = heatSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve heatSizeCurve = new AnimationCurve();
        heatSizeCurve.AddKey(0f, 0.5f);
        heatSizeCurve.AddKey(0.5f, 1.5f);
        heatSizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, heatSizeCurve);
        
        var colorOverLifetime = heatSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient heatGradient = new Gradient();
        GradientColorKey[] heatColorKeys = new GradientColorKey[] {
            new GradientColorKey(new Color(0.9f, 0.9f, 0.8f), 0f),
            new GradientColorKey(new Color(0.7f, 0.7f, 0.6f), 1f)
        };
        GradientAlphaKey[] heatAlphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(0.3f, 0f),
            new GradientAlphaKey(0.1f, 0.5f),
            new GradientAlphaKey(0f, 1f)
        };
        heatGradient.SetKeys(heatColorKeys, heatAlphaKeys);
        colorOverLifetime.color = heatGradient;
    }

    // Публичные методы
    public void PlayMuzzleFlash()
    {
        if (flashSystem != null) flashSystem.Play();
        if (sparksSystem != null) sparksSystem.Play();
        if (shockwaveSystem != null) shockwaveSystem.Play();
        if (heatSystem != null) heatSystem.Play();
    }

    public void PlayMuzzleFlashAtPosition(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(direction);
        PlayMuzzleFlash();
    }

    public void StopAllEffects()
    {
        if (flashSystem != null) flashSystem.Stop();
        if (sparksSystem != null) sparksSystem.Stop();
        if (shockwaveSystem != null) shockwaveSystem.Stop();
        if (heatSystem != null) heatSystem.Stop();
    }

#if UNITY_EDITOR
    [ContextMenu("Initialize Muzzle Flash Systems")]
    void InitializeSystems()
    {
        CreateMuzzleFlashSystems();
    }
    
    [ContextMenu("Test Muzzle Flash")]
    void TestMuzzleFlash()
    {
        if (Application.isPlaying)
        {
            PlayMuzzleFlash();
        }
    }
#endif
}