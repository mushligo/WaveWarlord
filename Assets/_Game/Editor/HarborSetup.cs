// AI-authored R0 setup. The learner implements Captain movement in a separate script.
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace WaveWarlord.EditorTools
{
    public static class HarborSetup
    {
        public const string ScenePath = "Assets/_Game/Scenes/HarborLab.unity";
        private const string SettingsPath = "Assets/_Game/Settings";
        private const string MaterialsPath = "Assets/_Game/Art/Materials";

        [MenuItem("WaveWarlord/R0/Create Harbor Lab")]
        public static void CreateHarborLab()
        {
            if (File.Exists(ScenePath))
                throw new InvalidOperationException("HarborLab already exists. Existing work is preserved.");
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            foreach (string directory in new[] { SettingsPath, MaterialsPath, "Assets/_Game/Scenes", "Assets/_Game/Scripts" })
                Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();

            UniversalRendererData renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            ResourceReloader.ReloadAllNullIn(renderer, UniversalRenderPipelineAsset.packagePath);
            AssetDatabase.CreateAsset(renderer, SettingsPath + "/HarborRenderer.asset");
            UniversalRenderPipelineAsset pipeline = UniversalRenderPipelineAsset.Create(renderer);
            AssetDatabase.CreateAsset(pipeline, SettingsPath + "/HarborPipeline.asset");
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;

            Material ground = MakeMaterial("Ground", new Color(0.58f, 0.61f, 0.57f));
            Material stone = MakeMaterial("Stone", new Color(0.30f, 0.36f, 0.41f));
            Material wood = MakeMaterial("Wood", new Color(0.46f, 0.30f, 0.18f));
            Material captain = MakeMaterial("Captain", new Color(0.95f, 0.57f, 0.17f));
            Material sea = MakeMaterial("Sea", new Color(0.08f, 0.35f, 0.46f));
            Material board = MakeMaterial("NoticeBoard", new Color(0.80f, 0.75f, 0.54f));

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Block("Ground", new Vector3(0, -0.25f, 0), new Vector3(20, 0.5f, 16), ground);
            Block("Sea_Visual", new Vector3(0, -0.8f, 15), new Vector3(44, 0.2f, 20), sea);
            Block("Dock", new Vector3(1, -0.15f, 10), new Vector3(4, 0.3f, 8), wood);
            Block("Wall", new Vector3(7, 1, 0), new Vector3(0.5f, 2, 10), stone);
            Block("Warehouse_Placeholder", new Vector3(-6, 1.5f, 4), new Vector3(4, 3, 4), stone);
            Block("NoticeBoard_Post", new Vector3(4, 0.75f, 5), new Vector3(0.18f, 1.5f, 0.18f), wood);
            Block("DepartureBoard_Placeholder", new Vector3(4, 1.6f, 5), new Vector3(1.8f, 1.2f, 0.15f), board);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Captain";
            player.transform.position = new Vector3(-2, 1, -1);
            player.GetComponent<Renderer>().sharedMaterial = captain;
            UnityEngine.Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2;
            controller.radius = 0.5f;
            controller.center = Vector3.zero;

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(12, 15, -18);
            cameraObject.transform.LookAt(new Vector3(0, 0, 2));
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.10f, 0.21f, 0.28f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 150;
            camera.fieldOfView = 50;
            cameraObject.AddComponent<UniversalAdditionalCameraData>();

            GameObject sun = new GameObject("Sun", typeof(Light));
            sun.transform.rotation = Quaternion.Euler(50, -30, 0);
            Light light = sun.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.3f;
            light.shadows = LightShadows.Soft;
            sun.AddComponent<UniversalAdditionalLightData>();
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.5f, 0.53f, 0.57f);

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException("Could not save HarborLab.");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.companyName = "WaveWarlordLearning";
            PlayerSettings.productName = "WaveWarlord";
            PlayerSettings.defaultIsNativeResolution = false;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.runInBackground = true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            AssetDatabase.SaveAssets();
            Validate();
            Debug.Log("R0_SETUP_PASS: HarborLab created; movement is a learner task.");
        }

        [MenuItem("WaveWarlord/R0/Validate Harbor Lab")]
        public static void Validate()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath);
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                    if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject) != 0)
                        throw new InvalidOperationException("Missing script on " + child.name);
            GameObject captain = GameObject.Find("Captain");
            if (captain == null || captain.GetComponent<CharacterController>() == null || Camera.main == null)
                throw new InvalidOperationException("Captain/controller/camera missing.");
            if (!(GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset))
                throw new InvalidOperationException("URP is not configured.");
            if (EditorBuildSettings.scenes.Length != 1 || EditorBuildSettings.scenes[0].path != ScenePath)
                throw new InvalidOperationException("Unexpected build scene configuration.");
            Debug.Log("R0_VALIDATE_PASS: scene, references, URP and build scene verified.");
        }

        [MenuItem("WaveWarlord/R0/Build Windows")]
        public static void BuildWindows()
        {
            Validate();
            Directory.CreateDirectory("Builds/Windows/R0");
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = "Builds/Windows/R0/WaveWarlord.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Build failed: " + report.summary.result);
            Debug.Log("R0_BUILD_PASS: " + report.summary.outputPath);
        }

        public static void CreateAndBuild() { CreateHarborLab(); BuildWindows(); }

        private static Material MakeMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
            Material material = new Material(shader) { name = name };
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.15f);
            AssetDatabase.CreateAsset(material, MaterialsPath + "/" + name + ".mat");
            return material;
        }

        private static void Block(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name = name;
            item.transform.position = position;
            item.transform.localScale = scale;
            item.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}
