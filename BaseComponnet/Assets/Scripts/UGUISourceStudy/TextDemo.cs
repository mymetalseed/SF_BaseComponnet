using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDemo : MonoBehaviour
{
    public Font font;
    // Start is called before the first frame update
    void Start()
    {
        CharacterInfo[] infos = font.characterInfo;
        Font.textureRebuilt += OnFontTextureRebuild;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            CharacterInfo[] infos = font.characterInfo;
            for(int i = 0;i<infos.Length;++i)
            {

            }
        }
    }

    void OnFontTextureRebuild(Font font)
    {
        Debug.Log(font.name + " 字体大小: " + font.fontSize + " 字体数量: " + font.characterInfo.Length);
    }
}
