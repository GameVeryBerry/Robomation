using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilePalette : MonoBehaviour
{
    public Prefabs prefabs;
    public GameObject buttonPrefabs;
    public Vector2 buttonSize;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (GameObject tilePrefab in prefabs.prefabs)
        {
            var obj = Instantiate(buttonPrefabs, transform);
            var meshRenderer = tilePrefab.GetComponentInChildren<MeshRenderer>();
            var texture = meshRenderer.sharedMaterial.mainTexture;
            var sprite = Sprite.Create(texture.ToTexture2D(), new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            obj.transform.Find("Image").GetComponent<Image>().sprite = sprite;
            obj.transform.localPosition = new Vector3(i / 4, i % 4, 0) * buttonSize;
            var rot = obj.transform.localEulerAngles;
            rot.z = meshRenderer.gameObject.transform.localEulerAngles.y;
            obj.transform.localEulerAngles = rot;
            i++;

        }
    }

    // Update is called once per frame
    void Update()
    {
               
    }

    //透明にする
    private void SetCanvasGroupEnable(CanvasGroup canvasGroup, bool enable)
    {
        if (enable)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}

public static class TextureExt
{
    public static Texture2D ToTexture2D(this Texture self)
    {
        var sw = self.width;
        var sh = self.height;
        var format = TextureFormat.RGBA32;
        var result = new Texture2D(sw, sh, format, false);
        var currentRT = RenderTexture.active;
        var rt = new RenderTexture(sw, sh, 32);
        Graphics.Blit(self, rt);
        RenderTexture.active = rt;
        var source = new Rect(0, 0, rt.width, rt.height);
        result.ReadPixels(source, 0, 0);
        result.Apply();
        RenderTexture.active = currentRT;
        return result;
    }
}