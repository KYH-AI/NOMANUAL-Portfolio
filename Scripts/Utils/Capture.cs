using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Capture : MonoBehaviour
{
    public Camera cam;
    public RenderTexture rt;
    public Image img;

    public GameObject[] captureObj;
    int nowCount = 0;

    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(AllCaptureImage()); // 수정: AllCaptureImage 코루틴 실행
    }

    void Create()
    {
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        yield return null;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        yield return null;

        var data = tex.EncodeToPNG();
        string name = $"Thumbnail_{captureObj[nowCount].gameObject.name}";
        string extension = ".png";
        string path = @"C:\NOMANUAL_Beta\Assets\!! MAIN CONTENTS !!\6. Textures\Inventory\Capture Image\";

        Debug.Log(path);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        File.WriteAllBytes(path + name + extension, data);
    }

    IEnumerator AllCaptureImage()
    {
        while (nowCount < captureObj.Length)
        {
            var newObj = Instantiate(captureObj[nowCount].gameObject); // 오브젝트 생성

            newObj.SetActive(true);
            yield return null;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            yield return null;

            var data = tex.EncodeToPNG();
            string name = $"Thumbnail_{captureObj[nowCount].gameObject.name}";
            string extension = ".png";
            string path = @"C:\NOMANUAL_Beta\Assets\!! MAIN CONTENTS !!\6. Textures\Inventory\Capture Image\";

            Debug.Log(path);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllBytes(path + name + extension, data); // 캡처 저장

            yield return null;

            newObj.SetActive(false); // 오브젝트 제거

            nowCount++; // 다음 오브젝트로 이동

            yield return null;
        }
    }
}
