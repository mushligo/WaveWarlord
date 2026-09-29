// AI-authored development-build diagnostic. Inactive during ordinary play.
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace WaveWarlord.Diagnostics
{
    public sealed class R0SmokeCheck : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartIfRequested()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--r0-smoke-test") < 0) return;
            new GameObject("R0 Smoke Check").AddComponent<R0SmokeCheck>();
        }

        private IEnumerator Start()
        {
            // Scene loading can finish while Unity's splash screen still covers it.
            float deadline = Time.realtimeSinceStartup + 15f;
            while (!SplashScreen.isFinished && Time.realtimeSinceStartup < deadline)
                yield return null;
            yield return new WaitForSecondsRealtime(1f);
            yield return new WaitForEndOfFrame();
            string directory = Path.Combine(Application.dataPath, "..", "SmokeEvidence");
            Directory.CreateDirectory(directory);
            GameObject captain = GameObject.Find("Captain");
            bool success = SceneManager.GetActiveScene().name == "HarborLab"
                && captain != null && captain.GetComponent<CharacterController>() != null
                && Camera.main != null && GraphicsSettings.currentRenderPipeline != null;
            // A hidden Windows player may not present its swap chain. Render the
            // actual scene camera through URP to a texture for unattended checks.
            Texture2D capture = null;
            if (Camera.main != null)
            {
                RenderTexture target = RenderTexture.GetTemporary(1280, 720, 24);
                RenderTexture previous = RenderTexture.active;
                try
                {
                    RenderPipeline.SubmitRenderRequest(Camera.main,
                        new UniversalRenderPipeline.SingleCameraRequest { destination = target });
                    RenderTexture.active = target;
                    capture = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                    capture.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
                    capture.Apply();
                }
                finally
                {
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(target);
                }
            }
            if (capture != null)
            {
                Color32[] pixels = capture.GetPixels32();
                bool hasVisibleContent = false;
                Color32 first = pixels[0];
                for (int i = 0; i < pixels.Length; i += 97)
                {
                    Color32 pixel = pixels[i];
                    if (Math.Abs(pixel.r - first.r) + Math.Abs(pixel.g - first.g)
                        + Math.Abs(pixel.b - first.b) > 30)
                    {
                        hasVisibleContent = true;
                        break;
                    }
                }
                success &= SplashScreen.isFinished && hasVisibleContent;
                File.WriteAllBytes(Path.Combine(directory, "HarborLab.png"), capture.EncodeToPNG());
                Destroy(capture);
            }
            else success = false;
            File.WriteAllText(Path.Combine(directory, "result.txt"),
                (success ? "PASS" : "FAIL") + "\nScene=" + SceneManager.GetActiveScene().name
                + "\nUnity=" + Application.unityVersion
                + "\nResolution=" + Screen.width + "x" + Screen.height
                + "\nCapture=URP_MainCamera_RenderTexture_1280x720"
                + "\nMovement=NotImplemented_LearnerTask\n");
            Debug.Log(success ? "R0_PLAYER_SMOKE_PASS" : "R0_PLAYER_SMOKE_FAIL");
            Application.Quit(success ? 0 : 1);
        }
    }
}
#endif
