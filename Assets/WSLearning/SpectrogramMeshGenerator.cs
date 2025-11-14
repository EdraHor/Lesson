using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SpectrogramMeshGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RealtimeSpectrogram spectrogramSource;
    
    [Header("Mesh Settings")]
    [SerializeField] private Vector2 meshSize = new Vector2(10f, 5f);
    [SerializeField] private int widthSegments = 128;
    [SerializeField] private int heightSegments = 64;
    [SerializeField] private float maxHeight = 3f;
    
    [Header("Visual Settings")]
    [SerializeField] private Gradient heightGradient;
    [SerializeField] [Range(0f, 10f)] private float emissionIntensity = 2f;
    [SerializeField] private Gradient emissionGradient;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private Material material;
    private Vector3[] vertices;
    private Color[] vertexColors;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (spectrogramSource == null)
            spectrogramSource = GetComponent<RealtimeSpectrogram>();
        
        InitializeGradients();
    }

    void Start()
    {
        SetupMaterial();
        GenerateMesh();
    }

    void Update()
    {
        if (spectrogramSource == null || mesh == null) return;
        
        UpdateMesh();
        UpdateColors();
        UpdateEmission();
    }

    void InitializeGradients()
    {
        if (heightGradient == null || heightGradient.colorKeys.Length == 0)
        {
            heightGradient = new Gradient();
            heightGradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0f),
                    new GradientColorKey(Color.blue, 0.25f),
                    new GradientColorKey(Color.cyan, 0.5f),
                    new GradientColorKey(Color.yellow, 0.75f),
                    new GradientColorKey(Color.red, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
        }
        
        if (emissionGradient == null || emissionGradient.colorKeys.Length == 0)
        {
            emissionGradient = new Gradient();
            emissionGradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.black, 0f),
                    new GradientColorKey(Color.blue, 0.33f),
                    new GradientColorKey(Color.cyan, 0.66f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
        }
    }

    [ContextMenu("Setup Material")]
    void SetupMaterial()
    {
        Shader shader = Shader.Find("Custom/Spectrogram");
        if (shader == null)
        {
            Debug.LogError("Shader 'Custom/SpectrogramShader' not found! Create the shader first.");
            return;
        }
        
        material = new Material(shader);
        material.name = "Spectrogram Material";
        
        if (spectrogramSource != null)
        {
            material.mainTexture = spectrogramSource.GetSpectrogramTexture();
        }
        
        material.SetFloat("_UseVertexColors", 1f);
        meshRenderer.material = material;
        
        Debug.Log("Material setup complete!");
    }

    [ContextMenu("Generate Mesh")]
    void GenerateMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Spectrogram Mesh";

        int vertexCount = (widthSegments + 1) * (heightSegments + 1);
        vertices = new Vector3[vertexCount];
        vertexColors = new Color[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[widthSegments * heightSegments * 6];

        // Генерация вершин
        float stepX = meshSize.x / widthSegments;
        float stepZ = meshSize.y / heightSegments;
        int vertIndex = 0;

        for (int z = 0; z <= heightSegments; z++)
        {
            for (int x = 0; x <= widthSegments; x++)
            {
                float posX = x * stepX - meshSize.x * 0.5f;
                float posZ = z * stepZ - meshSize.y * 0.5f;
                
                vertices[vertIndex] = new Vector3(posX, 0, posZ);
                vertexColors[vertIndex] = Color.white;
                uvs[vertIndex] = new Vector2((float)x / widthSegments, (float)z / heightSegments);
                
                vertIndex++;
            }
        }

        // Генерация треугольников
        int triIndex = 0;
        for (int z = 0; z < heightSegments; z++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                int bottomLeft = z * (widthSegments + 1) + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + (widthSegments + 1);
                int topRight = topLeft + 1;

                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = bottomRight;
                
                triangles[triIndex++] = bottomRight;
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = topRight;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.colors = vertexColors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        
        Debug.Log($"Mesh generated: {vertices.Length} vertices");
    }

    void UpdateMesh()
    {
        Texture2D tex = spectrogramSource.GetSpectrogramTexture();
        if (tex == null) return;

        for (int z = 0; z <= heightSegments; z++)
        {
            for (int x = 0; x <= widthSegments; x++)
            {
                int vertIndex = z * (widthSegments + 1) + x;
                
                float uvX = (float)x / widthSegments;
                float uvZ = (float)z / heightSegments;
                
                int pixelX = Mathf.RoundToInt(uvX * (tex.width - 1));
                int pixelY = Mathf.RoundToInt(uvZ * (tex.height - 1));
                
                Color pixel = tex.GetPixel(pixelX, pixelY);
                float height = pixel.grayscale * maxHeight;
                
                vertices[vertIndex].y = height;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void UpdateColors()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float normalizedHeight = Mathf.Clamp01(vertices[i].y / maxHeight);
            vertexColors[i] = heightGradient.Evaluate(normalizedHeight);
        }
        
        mesh.colors = vertexColors;
    }

    void UpdateEmission()
    {
        if (material == null) return;
        
        // Средняя высота для эмиссии
        float avgHeight = 0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            avgHeight += vertices[i].y;
        }
        avgHeight /= vertices.Length;
        
        float normalized = Mathf.Clamp01(avgHeight / maxHeight);
        Color emission = emissionGradient.Evaluate(normalized);
        
        material.SetColor("_EmissionColor", emission);
        material.SetFloat("_EmissionIntensity", emissionIntensity);
    }
}