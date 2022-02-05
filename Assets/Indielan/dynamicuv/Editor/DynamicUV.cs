using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshRenderer))]

public class DynamicUV : Editor
{


    List<Color> colors = new List<Color>();
    List<Color> oldColors = new List<Color>();
    Texture2D texture;
   

    string sharedMeshName;
    UvColors uvColors;
    UvMeshInfo uvMesh;
    //int currentMeshIndex = -1;

    MeshRenderer targetRenderer;
    MeshFilter targetFilter;

    int colorSize = 2;
    int textureSize = 256;
    int devidedSize = 128;
    bool isUsingSharedMaterial = false;


    string textureFileName = "shared_texture.png";
    string meshJsonFileName = "";
    //AbsPath 
    string applicationPath, resourcePath, texturePath, currentFolderPath,
        colorJsonFilePath, indielanResourcePath, meshJsonFilePath, editorOnlyResrourcePath;
    //RelativePath
    string absCurrentFolderPath, absEditorOnlyResrourcePath;

    //UV Points
    List<PointGroup> currentPointGroup;


    bool isGenereatedMesh = false;

    void DefineUnAffectedPaths()
    {
        var dynamicUVGUID = AssetDatabase.FindAssets("DynamicUV t:Script");
        absCurrentFolderPath = AssetDatabase.GUIDToAssetPath(dynamicUVGUID[0]).Replace("DynamicUV.cs", "").Substring(7);
        currentFolderPath = (Application.dataPath + "/" + absCurrentFolderPath).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        editorOnlyResrourcePath = (currentFolderPath + "Resources/").Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar); ;
        applicationPath = (Application.dataPath + "/").Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

        resourcePath = (applicationPath + "Resources/").Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        indielanResourcePath = (resourcePath + "indielan/").Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        texturePath = (indielanResourcePath + "uv/" + textureFileName).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        colorJsonFilePath = editorOnlyResrourcePath + "shared_texture.json";

        absEditorOnlyResrourcePath = absCurrentFolderPath + "Resources/";

    }
    bool CalcStartInfo()
    {

        targetRenderer = (MeshRenderer)target;
        targetFilter = targetRenderer.gameObject.GetComponent<MeshFilter>();
        if (targetFilter == null || targetFilter.sharedMesh == null) return false;
        sharedMeshName = targetFilter.sharedMesh.name;
        var meshFilePath = AssetDatabase.GetAssetPath(targetFilter.sharedMesh);
        if (meshFilePath == "") return false;
        meshJsonFileName = sharedMeshName.Replace(".", "") + "_" + Path.GetFileName(meshFilePath).Replace(".", "") + ".json";
        meshJsonFilePath = editorOnlyResrourcePath + meshJsonFileName;
        isGenereatedMesh = sharedMeshName.Contains("_indiegen_");
        return true;
    }

    void CreateFolders()
    {
        if (!Directory.Exists(editorOnlyResrourcePath))
        {
            
            Directory.CreateDirectory(editorOnlyResrourcePath);
        }

        if (!Directory.Exists(resourcePath))
        {
            Directory.CreateDirectory(resourcePath);
        }


        if (!Directory.Exists(indielanResourcePath))
        {
            Directory.CreateDirectory(indielanResourcePath);
        }


        if (!Directory.Exists(indielanResourcePath + "uv/"))
        {
            Directory.CreateDirectory(indielanResourcePath + "uv/");
        }

        if (!Directory.Exists(indielanResourcePath + "uv/models/"))
        {
            Directory.CreateDirectory(indielanResourcePath + "uv/models/");
        }

        AssetDatabase.Refresh();
    }
    private void Awake()
    {

        DefineUnAffectedPaths();

        if (!CalcStartInfo()) return;

        
        CheckSharedMaterial();

        if (isUsingSharedMaterial)
        {
            OnUseSharedMaterial();
        }
    }


    void OnUseSharedMaterial()
    {

        InitTexutre();
        LoadTexture();
        LoadJsonColorData();
        LoadJsonMeshData();
        CalcluteUVPoint();
        ProccessColorDataFromJson();
        UseSharedMaterial();

    }


    void InitTexutre()
    {

        if (!File.Exists(texturePath))
        {
            texture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();

            File.WriteAllBytes(texturePath, bytes);

            AssetDatabase.Refresh();
        }
    }


    void LoadTexture()
    {

        texture = Resources.Load<Texture2D>("indielan/uv/" + textureFileName.Replace(".png", ""));

        if (!texture) return;
        if (!texture.isReadable)
        {
            var tImporter = AssetImporter.GetAtPath("Assets/Resources/indielan/uv/" + textureFileName) as TextureImporter;

            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.Default;
                tImporter.textureCompression = TextureImporterCompression.Uncompressed;
                tImporter.isReadable = true;
                tImporter.filterMode = FilterMode.Point;
                tImporter.SaveAndReimport();
            }
        }
    }


    void LoadJsonColorData()
    {

        if (!isGenereatedMesh) return;
        if (File.Exists(colorJsonFilePath))
        {
         
            TextAsset textAsset = (TextAsset)Resources.Load(textureFileName.Replace(".png", ""), typeof(TextAsset));

            if (textAsset != null && textAsset.text != null && textAsset.text != "")
            {
                uvColors = JsonUtility.FromJson<UvColors>(textAsset.text);
            }
        }
    }
    void LoadJsonMeshData()
    {
        if (File.Exists(meshJsonFilePath))
        {
            TextAsset textAsset = (TextAsset)Resources.Load(meshJsonFileName.Replace(".json", ""), typeof(TextAsset));
            if (textAsset != null && textAsset.text != null && textAsset.text != "")
            {
                uvMesh = JsonUtility.FromJson<UvMeshInfo>(textAsset.text);
            }
        }
    }


    int CalcluteUVPoint()
    {
   
        colors.Clear();
        oldColors.Clear();
        Vector2[] _uvs = targetFilter.sharedMesh.uv;
        currentPointGroup = new List<PointGroup>();
        if (uvMesh == null)
        {
           
            for (var i = 0; i < _uvs.Length; i++)
            {

                bool makeANewPointGroup = true;
                for (var ai = 0; ai < currentPointGroup.Count; ai++)
                {
                    Vector2 _savedPoint = currentPointGroup[ai].mainPoint;

                    if (Vector2.Distance(_savedPoint, _uvs[i]) < 0.02f)
                    {
                        currentPointGroup[ai].pointIndexes.Add(i);
                        makeANewPointGroup = false;
                        break;
                    }
                }

                if (!makeANewPointGroup) continue;
                Vector2 _point = _uvs[i];
                PointGroup _pointGroup = new PointGroup();
                _pointGroup.pointIndexes.Add(i);
                _pointGroup.mainPoint = _point;
                currentPointGroup.Add(_pointGroup);
                colors.Add(Color.white);
                oldColors.Add(Color.white);

            }
        }
        else
        {
            currentPointGroup = uvMesh.pointGroups;

            for (var i = 0; i < currentPointGroup.Count; i++)
            {
                colors.Add(Color.white);
                oldColors.Add(Color.white);
            }
        }

        return currentPointGroup.Count;
    }


    void ProccessColorDataFromJson()
    {
        if (uvColors == null || uvMesh == null) return;

        


        for (var ui = 0; ui < colors.Count; ui++)
        {
            colors[ui] = uvColors.colors[uvMesh.colorIdOnTexture[ui]];
            oldColors[ui] = colors[ui];
        }
    }


    void UseSharedMaterial()
    {
      
        if (!File.Exists(indielanResourcePath + "uv/" + "shared.mat"))
        {
            CreateSharedMaterial();
            //LoadJsonData();
        }
        else
        {
            Material _currentMaterial = Resources.Load<Material>("indielan/uv/shared");

            if (
                (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null && _currentMaterial.shader != UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.defaultShader)
                || (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset == null && _currentMaterial.shader != Shader.Find("Standard"))

                )
            {
                CreateSharedMaterial();
            }
        }

        Material _materialToUse = Resources.Load<Material>("indielan/uv/shared");


        targetRenderer.sharedMaterial = _materialToUse;



        //Check if The Pipeline Changed 

    }


    void CheckSharedMaterial()
    {
       

        var _matPath = AssetDatabase.GetAssetPath(targetRenderer.sharedMaterial);
        isUsingSharedMaterial = _matPath == "Assets/Resources/indielan/uv/shared.mat";

    }


    void CreateNewMesh()
    {
        var _blenderFilePath = AssetDatabase.GetAssetPath(targetFilter.sharedMesh);
        if (_blenderFilePath == "") return;

        if (targetFilter == null || targetFilter.sharedMesh == null) return;


        Mesh _tempMesh = (Mesh)UnityEngine.Object.Instantiate(targetFilter.sharedMesh);

        var _savePath = "";
  
        if (_blenderFilePath.Contains("_indiegen_"))
        {
            return;
        }
        else
        {
           // var _indexOfDot = _blenderFilePath.LastIndexOf(".");
            _savePath = "Assets/Resources/indielan/uv/models/" + meshJsonFileName.Replace(".json", "") + "_indiegen_.asset";
        }


        
        _tempMesh.name = sharedMeshName;

        AssetDatabase.CreateAsset(_tempMesh, _savePath);

        targetFilter.mesh = _tempMesh;
    }

    void CreateSharedMaterial()
    {

        Shader _shader = Shader.Find("Standard");
        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            _shader = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.defaultShader;


        Material _mat = new Material(_shader);
        _mat.mainTexture = texture;
        _mat.SetFloat("_Glossiness", 0f);
        _mat.SetFloat("_Smoothness", 0f);
        AssetDatabase.CreateAsset(_mat, "Assets/Resources/indielan/uv/shared.mat");
        AssetDatabase.Refresh();
    }



    void InitColorJsonFile()
    {
        if (!File.Exists(colorJsonFilePath))
        {
            var _uvColors = new UvColors();
            for (var i = 0; i < colors.Count; i++)
            {
                CreateColorOnTexture(i, Color.black);
                _uvColors.colors.Add(Color.black);
            }
            StreamWriter writer = new StreamWriter(colorJsonFilePath);
            writer.WriteLine(JsonUtility.ToJson(_uvColors));
            writer.Close();
        }
        else
        {
   

            for (var i = 0; i < colors.Count; i++)
            {
                uvColors.colors.Add(Color.black);
            }
            StreamWriter writer = new StreamWriter(colorJsonFilePath);
            writer.WriteLine(JsonUtility.ToJson(uvColors));
            writer.Close();
        }

        AssetDatabase.ImportAsset("Assets/" + absEditorOnlyResrourcePath + textureFileName.Replace(".png", ".json"));

    }


    void InitMeshJsonFile()
    {

        if (!File.Exists(meshJsonFilePath))
        {
            var _uvMeshInfo = new UvMeshInfo();

            _uvMeshInfo.name = sharedMeshName;
            _uvMeshInfo.pointGroups = currentPointGroup;


            if (uvColors != null)
            {
                for (var i = 0; i < colors.Count; i++)
                {
                    _uvMeshInfo.colorIdOnTexture.Add(uvColors.colors.Count + i);
                }
            }
            else
            {
                for (var i = 0; i < colors.Count; i++)
                {
                    _uvMeshInfo.colorIdOnTexture.Add(i);
                }
            }



            StreamWriter writer2 = new StreamWriter(meshJsonFilePath);
            writer2.WriteLine(JsonUtility.ToJson(_uvMeshInfo));
            writer2.Close();
        }


        AssetDatabase.ImportAsset("Assets/" + absEditorOnlyResrourcePath + meshJsonFileName);
    }


    void UpdateJsonFiles()
    {

        if (File.Exists(colorJsonFilePath) && uvColors != null)
        {
            StreamWriter writer = new StreamWriter(colorJsonFilePath);
            writer.WriteLine(JsonUtility.ToJson(uvColors));
            writer.Close();
        }


        if (File.Exists(meshJsonFilePath) && meshJsonFilePath != null)
        {
            StreamWriter writer2 = new StreamWriter(meshJsonFilePath);
            writer2.WriteLine(JsonUtility.ToJson(uvMesh));
            writer2.Close();
        }




    }
    int times = 0;
    public override void OnInspectorGUI()
    {
        if (sharedMeshName == "" || sharedMeshName == null)
        {
            DrawDefaultInspector();
            return;
        }

        if (!isUsingSharedMaterial || !isGenereatedMesh)
        {
            if (GUILayout.Button("Apply Shared Material"))
            {
                CreateFolders();
                CalcStartInfo();
                CreateNewMesh();
                CalcStartInfo();
                if (CalcluteUVPoint() > 10)
                {
                    Debug.LogError("Please use this tool only on supported model");
                    return;
                }

                
                InitTexutre();
                LoadTexture();
                LoadJsonColorData();
                InitMeshJsonFile();
                InitColorJsonFile();
                LoadJsonMeshData();

                UseSharedMaterial();
                //OnUseSharedMaterial();
                CheckSharedMaterial();

               
                Repaint();
                return;
            }
            DrawDefaultInspector();
            return;
        }

        if ((uvMesh == null || uvColors == null))
        {
            string message = "Preparing the uv";

            for (var i = 0; i < Mathf.FloorToInt(times / colorSize); i++)
            {
                message += ".";
            }
            GUILayout.Label(message);


            times++;

        
            LoadJsonColorData();
            LoadJsonMeshData();
            ProccessColorDataFromJson();
            Repaint();
            return;
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        for (var i = 0; i < colors.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            colors[i] = EditorGUILayout.ColorField(colors[i]);


            EditorGUILayout.EndHorizontal();
            if (oldColors[i] != colors[i])
            {
                oldColors[i] = colors[i];
                OnColorChanges(i);
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();

    }


    void OnColorChanges(int whichColorChanged)
    {

        CreateColorOnTexture(uvMesh.colorIdOnTexture[whichColorChanged], colors[whichColorChanged]);
        uvColors.colors[uvMesh.colorIdOnTexture[whichColorChanged]] = colors[whichColorChanged];
        ChangeUV(whichColorChanged);
        SaveTexture();
        AssetDatabase.Refresh();
    }


    void CreateColorOnTexture(int colorIndex, Color colorToCreate)
    {
  
     

        int yEvery = Mathf.FloorToInt(256 / colorSize);
        int _yRealIndex = Mathf.FloorToInt(colorIndex / yEvery);

        for (var i = colorIndex * colorSize; i < (colorIndex + 1) * colorSize; i++)
        {
            for (var y = _yRealIndex * colorSize; y < (_yRealIndex + 1) * colorSize; y++)
            {
                texture.SetPixel(i, y, colorToCreate);
            }
        }



    }

    void ChangeUV(int groupIndex)
    {
        if (uvMesh == null) return;
        var _colorIndex = uvMesh.colorIdOnTexture[groupIndex];

        float _xPos = Mathf.Repeat(_colorIndex, devidedSize);
        float _yPos = Mathf.FloorToInt(_colorIndex / devidedSize);

        _xPos /= devidedSize;
        _yPos /= devidedSize;

        _xPos += 0.00390625f;
        _yPos += 0.00390625f;



        List<Vector2> _uvs = new List<Vector2>();
        int _count = uvMesh.pointGroups[groupIndex].pointIndexes.Count;
        List<int> indexes = uvMesh.pointGroups[groupIndex].pointIndexes;
        for (var _i = 0; _i < targetFilter.sharedMesh.vertexCount; _i++)
        {
            if (indexes.Contains(_i))
            {
                _uvs.Add(new Vector2(_xPos, _yPos));
            }
            else
            {
                _uvs.Add(targetFilter.sharedMesh.uv[_i]);
            }

        }






        targetFilter.sharedMesh.SetUVs(0, _uvs.ToArray());
        targetFilter.sharedMesh.uv = _uvs.ToArray();
        targetFilter.sharedMesh.RecalculateNormals();
        targetFilter.sharedMesh.RecalculateBounds();

        targetFilter.sharedMesh.Optimize();
    }


    void SaveTexture()
    {

        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(texturePath, bytes);
        AssetDatabase.Refresh();
    }


    private void OnDisable()
    {

        

        if (!isUsingSharedMaterial) return;
        if (uvColors != null && uvMesh != null && isGenereatedMesh)
        {
            UpdateJsonFiles();
        }
       

        AssetDatabase.Refresh();

    }
}
