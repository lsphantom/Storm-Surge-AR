﻿using UnityEngine;
using UnityEditor;
using System.Collections;

using System;
using System.Reflection;
#if UNITY_POST_PROCESSING_STACK_V1 && !UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
using UnityEngine.PostProcessing;
#endif
#if UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
using UnityEngine.Rendering.PostProcessing;
#endif

public enum TARGETPLATFORM
{
    desktopWebAndConsole = 0,
    mobile = 1
}

public enum MOBILEPRESETS
{
    lake = 0,
    swamp = 1,
    sewer = 2,
    rtsStyle1 = 3,
    rtsStyle2 = 4
}

public enum PRESETS
{
    clearLake = 0,
    deepLake = 1,
    swamp = 2,
    sewer = 3,
    calmOcean = 4,
    pondsAndPools = 5,
    rtsStyle1 = 6,
    rtsStyle2 = 7
}

public class AQUAS_QuickSetup : EditorWindow {

    public static AQUAS_QuickSetup window;
    public float waterLevel = 0;
    public GameObject terrain = null;
    public GameObject camera = null;

    float terrainBounds;
    Vector3 waterPosition;

    FieldInfo fi = null;

    public PRESETS presets;
    public MOBILEPRESETS mobilePresets;
    public TARGETPLATFORM targetPlatform;

    //Add menu item to show window
    [MenuItem("Window/AQUAS/Quick Setup")]

    public static void OpenWindow()
    {
        window = (AQUAS_QuickSetup)EditorWindow.GetWindow(typeof(AQUAS_QuickSetup));
        window.titleContent.text = "AQUAS";
    }

    void OnGUI()
    {

        if (window == null)
        {
            OpenWindow();
        }    

        GUILayout.Label("Quick Setup", EditorStyles.boldLabel);
        waterLevel = EditorGUI.FloatField(new Rect(5, 30, position.width - 10, 16), "Water Level", waterLevel);
        terrain = (GameObject)EditorGUI.ObjectField(new Rect(5, 50, position.width - 10, 16), "Terrain", terrain, typeof(GameObject), true);
        camera = (GameObject)EditorGUI.ObjectField(new Rect(5, 70, position.width - 10, 16), "Main Camera", camera, typeof(GameObject), true);


        if (GUI.Button(new Rect(5, 100, position.width - 10, 32), "About"))
        {
            About();
        }

        targetPlatform = (TARGETPLATFORM)EditorGUI.EnumPopup(new Rect(5, 150, position.width - 10, 16), "Select target platform", targetPlatform);

        if(targetPlatform == TARGETPLATFORM.desktopWebAndConsole)
        {
            presets = (PRESETS)EditorGUI.EnumPopup(new Rect(5, 170, position.width - 10, 16), "Select a Preset", presets);
        }

        else
        {
            mobilePresets = (MOBILEPRESETS)EditorGUI.EnumPopup(new Rect(5, 170, position.width - 10, 16), "Select a Preset", mobilePresets);
        }

        if (GUI.Button(new Rect(5, 190, position.width - 10, 32), "Add Water"))
        {
            AddWater();
        }

        if (GUI.Button(new Rect(5, 230, position.width - 10, 32), "Add Underwater Effects"))
        {
            AddUnderwaterEffects();
        }

        if (GUI.Button(new Rect(5, 270, position.width/2-5, 32), "Hook Up Camera"))
        {
            HookupCamera();
        }

        if (GUI.Button(new Rect(position.width/2+5, 270, position.width / 2 - 10, 32), "<== Info"))
        {
            HookupCameraInfo();
        }

        if (GUI.Button(new Rect(5, 310, position.width - 10, 32), "Hints"))
        {
            Hints();
        }

    }

    void Update()
    {
        Repaint();
    }

    //<summary>
    //Gives some info on the quick setup feature
    //</summary>
    void About()
    {
        EditorUtility.DisplayDialog("AQUAS Quick Setup", "The quick setup will help you with the basic setup of AQUAS. You can add water and underwater effects. Please keep in mind that it's a basic setup. More advanced setups (e.g. multiple water levels) have to be done manually.", "Got It");
    }

    //<summary>
    //Adds water to the scene
    //based on parameters given
    //</summary>
    void AddWater()
    {

        if (terrain == null || camera == null)
        {
            EditorUtility.DisplayDialog("Specify Parameters", "Please specify the required parameters (terrain, camera & water level)", "Ok");
            return;
        }

        //Get scene info
        terrainBounds = terrain.GetComponent<Terrain>().terrainData.size.x;
        waterPosition = new Vector3(terrain.transform.position.x,0,terrain.transform.position.z) + new Vector3(terrainBounds/2, waterLevel, terrainBounds/2);
        
        //Add the AQUAS prefab to the scene
        
        GameObject aquasPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/AQUASWater.prefab", typeof(GameObject));
        
        if(targetPlatform == TARGETPLATFORM.desktopWebAndConsole)
        {
            switch (presets)
            {
                case PRESETS.clearLake:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/Clear Lake.mat", typeof(Material));
                    break;
                case PRESETS.deepLake:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/Deep Lake.mat", typeof(Material));
                    break;
                case PRESETS.swamp:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/Swamp.mat", typeof(Material));
                    break;
                case PRESETS.sewer:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/Sewer.mat", typeof(Material));
                    break;
                case PRESETS.calmOcean:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/Calm Ocean.mat", typeof(Material));
                    break;
                case PRESETS.pondsAndPools:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/Ponds & Pools.mat", typeof(Material));
                    break;
                case PRESETS.rtsStyle1:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/RTS-Style 1.mat", typeof(Material));
                    break;
                case PRESETS.rtsStyle2:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/Presets/RTS-Style 2.mat", typeof(Material));
                    break;
            }
        }
        else
        {
            switch (mobilePresets)
            {
                case MOBILEPRESETS.lake:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Mobile/Presets/Mobile Lake.mat", typeof(Material));
                    break;
                case MOBILEPRESETS.swamp:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Mobile/Presets/Mobile Swamp.mat", typeof(Material));
                    break;
                case MOBILEPRESETS.sewer:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Mobile/Presets/Mobile Sewer.mat", typeof(Material));
                    break;
                case MOBILEPRESETS.rtsStyle1:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Mobile/Presets/Mobile RTS 1.mat", typeof(Material));
                    break;
                case MOBILEPRESETS.rtsStyle2:
                    aquasPrefab.GetComponent<MeshRenderer>().sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Mobile/Presets/Mobile RTS 2.mat", typeof(Material));
                    break;
            }
        }

        

        //Check if AQUAS is already in the scene
        if (GameObject.Find("AQUAS Waterplane") != null)
        {
            EditorUtility.DisplayDialog("Can't add water", "AQUAS is already in the scene", "Ok");
            return;
        }

        //Try to add AQUAS
        GameObject aquasObj = Instantiate(aquasPrefab);

        if (aquasObj == null)
        {
            Debug.LogWarning("Unable to create AQUAS object - Aborting!");
            return;
        }

        //Position and configure the waterplane correctly
        else
        {
            aquasObj.name = "AQUAS Waterplane";
            aquasObj.transform.position = waterPosition;
            aquasObj.transform.localScale = new Vector3(terrainBounds*75, terrainBounds*75,terrainBounds*75);

            if(camera.gameObject.GetComponent<AQUAS_Camera>()==null)
            {
                camera.gameObject.AddComponent<AQUAS_Camera>();
            }
            

            //Check if Tenkoku is in the scene
            if (GameObject.Find("Tenkoku DynamicSky") != null)
            {

            }

            else
            {
                GameObject refProbe = new GameObject("Reflection Probe");
                refProbe.transform.position = waterPosition;
                refProbe.AddComponent<ReflectionProbe>();
                refProbe.GetComponent<ReflectionProbe>().intensity = 0.3f;
                refProbe.transform.SetParent(aquasObj.transform);
                refProbe.GetComponent<ReflectionProbe>().mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            }

            //Add caustics
            GameObject primaryCausticsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/PrimaryCausticsProjector.prefab", typeof(GameObject));
            GameObject primaryCausticsObj = Instantiate(primaryCausticsPrefab);
            primaryCausticsObj.name = "PrimaryCausticsProjector";
            GameObject secondaryCausticsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/SecondaryCausticsProjector.prefab", typeof(GameObject));
            GameObject secondaryCausticsObj = Instantiate(secondaryCausticsPrefab);
            secondaryCausticsObj.name = "SecondaryCausticsProjector";

            primaryCausticsObj.transform.position = waterPosition;
            secondaryCausticsObj.transform.position = waterPosition;

            primaryCausticsObj.GetComponent<Projector>().orthographicSize = terrainBounds/2;
            secondaryCausticsObj.GetComponent<Projector>().orthographicSize = terrainBounds/2;

            primaryCausticsObj.transform.SetParent(aquasObj.transform);
            secondaryCausticsObj.transform.SetParent(aquasObj.transform);

            //Set some material parameters based on whether Tenkoku is in the scene or not
            if (GameObject.Find("Tenkoku DynamicSky") != null)
            {
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_EnableCustomFog", 1);
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_Specular", 0.5f);
            }

            else
            {
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_EnableCustomFog", 0);
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_Specular", 1);
            }
        }
    }

    //<summary>
    //Adds Underwater Effects to the scene
    //</summary>
    void AddUnderwaterEffects()
    {

        GameObject underwaterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/UnderWaterCameraEffects.prefab", typeof(GameObject));

        //Check if AQUAS is already in the scene
        if (GameObject.Find("AQUAS Waterplane") == null)
        {
            EditorUtility.DisplayDialog("AQUAS is not in the scene", "Please add water first", "Ok");
            return;
        }

        //Try to add underwater effects
        else
        {

            terrainBounds = terrain.GetComponent<Terrain>().terrainData.size.x;

            if (GameObject.Find("UnderWaterCameraEffects") != null)
            {
                EditorUtility.DisplayDialog("Can't add underwater effects", "Underwater Effects are already in the scene", "Ok");
                return;
            }

            GameObject underwaterObj = Instantiate(underwaterPrefab);
            underwaterObj.name = "UnderWaterCameraEffects";

#if UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
            underwaterObj.GetComponent<AQUAS_LensEffects>().underWaterParameters.underwaterProfile = (PostProcessProfile)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Post Processing/AQUAS_Underwater_v2.asset", typeof(PostProcessProfile));
            underwaterObj.GetComponent<AQUAS_LensEffects>().underWaterParameters.defaultProfile = (PostProcessProfile)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Post Processing/DefaultPostProcessing_v2.asset", typeof(PostProcessProfile));
#endif

            camera.GetComponent<Camera>().farClipPlane = terrainBounds;

            //Underwater effects setup
            underwaterObj.transform.SetParent(camera.transform);
            underwaterObj.transform.localPosition = new Vector3(0, 0, 0);
            underwaterObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            underwaterObj.GetComponent<AQUAS_LensEffects>().gameObjects.mainCamera = camera.gameObject;
            underwaterObj.GetComponent<AQUAS_LensEffects>().gameObjects.waterPlanes[0] = GameObject.Find("AQUAS Waterplane");

            //Add and configure image effects neccessary for AQUAS
#if UNITY_POST_PROCESSING_STACK_V1 && !UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
            if (camera.gameObject.GetComponent<PostProcessingBehaviour>() == null)
            {
                camera.gameObject.AddComponent<PostProcessingBehaviour>();
            }
#endif
#if UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
            if (camera.gameObject.GetComponent<PostProcessLayer>() == null)
            {
                camera.gameObject.AddComponent<PostProcessLayer>();

#if UNITY_2017 || UNITY_5_6
                PostProcessResources resources;

                if ((PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources)) != null)
                {
                    resources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources));
                }
                else if ((PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources)) != null)
                {
                    resources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources));
                }
                else if ((PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessResources.asset", typeof(PostProcessResources)) != null)
                {
                    resources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessResources.asset", typeof(PostProcessResources));
                }
                else
                {
                    EditorUtility.DisplayDialog("Could not locate Post Process Resource file.", "Please make sure your post processing folder is at the top level of the assets folder and named either 'PostProcessing' or 'PostProcessing-2'", "Got It!");
                    resources = null;
                }

                camera.gameObject.GetComponent<PostProcessLayer>().Init(resources);
#endif
                camera.gameObject.GetComponent<PostProcessLayer>().volumeLayer = LayerMask.NameToLayer("Everything");
            }

            if (camera.gameObject.GetComponent<PostProcessVolume>() == null)
            {
                camera.gameObject.AddComponent<PostProcessVolume>();
                camera.gameObject.GetComponent<PostProcessVolume>().isGlobal = true;
            }
#endif

                if (camera.gameObject.GetComponent<AudioSource>() == null)
            {
                camera.gameObject.AddComponent<AudioSource>();
            }

            //Add borders to the scene for correct fog

            if (GameObject.Find("Borders") != null)
            {
                return;
            }

            GameObject borders = new GameObject();
            borders.name = "Borders";

            GameObject borderLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderLeft.name = "Left Border";
            borderLeft.transform.localScale = new Vector3(terrainBounds, waterLevel, 0.1f);
            borderLeft.transform.position = new Vector3(waterPosition.x, waterLevel - (waterLevel / 2), waterPosition.z + terrainBounds/2);

            GameObject borderRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderRight.name = "Right Border";
            borderRight.transform.localScale = new Vector3(terrainBounds, waterLevel, 0.1f);
            borderRight.transform.position = new Vector3(waterPosition.x, waterLevel - (waterLevel / 2), waterPosition.z - terrainBounds / 2);

            GameObject borderTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderTop.name = "Top Border";
            borderTop.transform.localScale = new Vector3(0.1f, waterLevel, terrainBounds);
            borderTop.transform.position = new Vector3(waterPosition.x + terrainBounds/2, waterLevel - (waterLevel / 2), waterPosition.z);

            GameObject borderBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderBottom.name = "Bottom Border";
            borderBottom.transform.localScale = new Vector3(0.1f, waterLevel, terrainBounds);
            borderBottom.transform.position = new Vector3(waterPosition.x - terrainBounds / 2, waterLevel - (waterLevel / 2), waterPosition.z);

            borderLeft.transform.SetParent(borders.transform);
            borderRight.transform.SetParent(borders.transform);
            borderTop.transform.SetParent(borders.transform);
            borderBottom.transform.SetParent(borders.transform);

            borders.transform.SetParent(GameObject.Find("AQUAS Waterplane").transform);
        }
    }

    //<summary>
    //Adds image effects to camera and configures them
    //Does NOT add underwater effects & borders
    //</summary>
    void HookupCamera()
    {
#if UNITY_POST_PROCESSING_STACK_V1 && !UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
        camera.gameObject.AddComponent<PostProcessingBehaviour>();
#endif
#if UNITY_POST_PROCESSING_STACK_V2 && AQUAS_PRESENT
        if (camera.gameObject.GetComponent<PostProcessLayer>() == null)
        {
            camera.gameObject.AddComponent<PostProcessLayer>();

            /*PostProcessResources resources;

            if ((PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources)) != null)
            {
                resources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources));
            }
            else if ((PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources)) != null)
            {
                resources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources));
            }
            else if ((PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessResources.asset", typeof(PostProcessResources)) != null)
            {
                resources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Assets/PostProcessing-2/PostProcessResources.asset", typeof(PostProcessResources));
            }
            else
            {
                EditorUtility.DisplayDialog("Could not locate Post Process Resource file.", "Please make sure your post processing folder is at the top level of the assets folder and named either 'PostProcessing' or 'PostProcessing-2'", "Got It!");
                resources = null;
            }

            camera.gameObject.GetComponent<PostProcessLayer>().Init(resources);*/
            camera.gameObject.GetComponent<PostProcessLayer>().volumeLayer = LayerMask.NameToLayer("Everything");
        }

        if (camera.gameObject.GetComponent<PostProcessVolume>() == null)
        {
            camera.gameObject.AddComponent<PostProcessVolume>();
            camera.gameObject.GetComponent<PostProcessVolume>().isGlobal = true;
        }
#endif

        if (camera.gameObject.GetComponent<AudioSource>() == null)
        {
            camera.gameObject.AddComponent<AudioSource>();
        }
    }

    //<summary>
    //Displays info on hooking up the camera
    //</summary>
    void HookupCameraInfo()
    {
        EditorUtility.DisplayDialog("Hook Up Camera", "Hooking up the camera will configure it for underwater effects, but won't import the Effects prefab to the scene, nor will it create borders.","Ok");
    }

    //<summary>
    //Add hints dialog
    //</summary>
    void Hints()
    {
        EditorUtility.DisplayDialog("Hints", "Using AQUAS with custom lighting systems like Tenkoku: When using AQUAS with custom lighting systems like Tenkoku or TOD, you might have to adjust the specular value on the water material. Custom lighting systems sometimes use z-buffer-based fog, which AQUAS cannot receive. If AQUAS doesn't receive fog, enable the custom fog property on the water material.", "Got It!");
    }



    /// <summary>
    /// Get the specified type if it exists
    /// </summary>
    /// <param name="TypeName">Name of the type to load</param>
    /// <returns>Selected type or null</returns>
    Type GetType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains("."))
        {
            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;

                // Ask that assembly to return the proper Type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
            catch (Exception)
            {
                //Debug.Log("Unable to load assemmbly : " + ex.Message);
            }
        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = Assembly.GetCallingAssembly();
        {
            // Load the referenced assembly
            if (currentAssembly != null)
            {
                // See if that assembly defines the named type
                type = currentAssembly.GetType(TypeName);
                if (type != null)
                    return type;
            }

        }

        //All loaded assemblies
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int asyIdx = 0; asyIdx < assemblies.GetLength(0); asyIdx++)
        {
            type = assemblies[asyIdx].GetType(TypeName);
            if (type != null)
            {
                return type;
            }
        }

        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            // Load the referenced assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;
    }

    void HackGlobalFog()
    {
        Type fogType = GetType("UnityStandardAssets.ImageEffects.GlobalFog");
        if (fogType == null)
        {
            EditorUtility.DisplayDialog("OOPS!", "Could not add global fog. Please find import Standard Effects Package : Assets -> Import Package -> Effects.", "OK");
            return;
        }
        else
        {
            var globalFog = camera.GetComponent(fogType);
            if (globalFog == null)
            {
                globalFog = camera.AddComponent(fogType);
            }
            fi = fogType.GetField("heightFog", BindingFlags.Public | BindingFlags.Instance);
            if (fi != null)
            {
                fi.SetValue(globalFog, false);
            }
            fi = fogType.GetField("startDistance", BindingFlags.Public | BindingFlags.Instance);
            if (fi != null)
            {
                fi.SetValue(globalFog, 100);
            }
        }
    }
}

