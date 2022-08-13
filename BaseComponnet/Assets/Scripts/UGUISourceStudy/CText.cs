using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CText : Text
{

    UIVertex[] m_Temp2Verts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        //base.OnPopulateMesh(toFill);
        if (font == null)
            return;
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }
        float minX = 0, minY = 0, maxX = 0, maxY = 0;
        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if(roundingOffset != Vector2.zero)
        {
            minX = verts[0].position.x;
            maxX = verts[0].position.x;
            minY = verts[0].position.y;
            maxY = verts[0].position.y;
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_Temp2Verts[tempVertsIndex] = verts[i];
                m_Temp2Verts[tempVertsIndex].position *= unitsPerPixel;
                m_Temp2Verts[tempVertsIndex].position.x += roundingOffset.x;
                m_Temp2Verts[tempVertsIndex].position.y += roundingOffset.y;
                minX = Mathf.Min(minX, m_Temp2Verts[tempVertsIndex].position.x);
                maxX = Mathf.Max(maxX, m_Temp2Verts[tempVertsIndex].position.x);
                minY = Mathf.Min(minY, m_Temp2Verts[tempVertsIndex].position.y);
                maxY = Mathf.Max(maxY, m_Temp2Verts[tempVertsIndex].position.y);
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_Temp2Verts);
            }
        }
        else
        {
            UIVertex vert;
            Debug.Log(vertCount);
            minX = verts[0].position.x;
            maxX = verts[0].position.x;
            minY = verts[0].position.y;
            maxY = verts[0].position.y;
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                vert = verts[i];
                vert.color = Color.black;
                
                m_Temp2Verts[tempVertsIndex] = vert;
                m_Temp2Verts[tempVertsIndex].position *= unitsPerPixel;
                minX = Mathf.Min(minX, m_Temp2Verts[tempVertsIndex].position.x);
                maxX = Mathf.Max(maxX, m_Temp2Verts[tempVertsIndex].position.x);
                minY = Mathf.Min(minY, m_Temp2Verts[tempVertsIndex].position.y);
                maxY = Mathf.Max(maxY, m_Temp2Verts[tempVertsIndex].position.y);
                if (tempVertsIndex == 3)
                {
                    //当走到这一步时,是一个有4个顶点的Quad,可以在这里对Quad进行处理

                    //比如加下划线(逐文字),
                    toFill.AddUIVertexQuad(m_Temp2Verts);
                }
                //toFill.AddUIVertexQuad(m_Temp2Verts);
            }
        }

        TextGenerator underlineText = new TextGenerator();
        underlineText.Populate("-", settings);
        IList<UIVertex> lineVer = underlineText.verts;
        Color tempColor = Color.black;
        minX *= 1.5f;
        maxX *= 1.5f;
        Debug.Log(minX.ToString()+" " +minY.ToString()+" " +maxX.ToString() + " " + maxY.ToString());
        m_Temp2Verts[0] = lineVer[0];
        m_Temp2Verts[0].position = new Vector3(minX, minY, 0);
        m_Temp2Verts[0].color = Color.black;
        //m_Temp2Verts[0].uv0 = new Vector4(0.0f, 0.08f, 0, 0);

        m_Temp2Verts[1] = lineVer[1];
        m_Temp2Verts[1].position = new Vector3(maxX, minY, 0);
        m_Temp2Verts[1].color = Color.black;
        //m_Temp2Verts[1].uv0 = new Vector4(0.0f, 0.08f, 0,0);

        m_Temp2Verts[2] = lineVer[2];
        m_Temp2Verts[2].position = new Vector3(maxX, minY - 1, 0);
        m_Temp2Verts[2].color = Color.black;
        //m_Temp2Verts[2].uv0 = new Vector4(0.0f, 0.08f, 0, 0);

        m_Temp2Verts[3] = lineVer[3];
        m_Temp2Verts[3].position = new Vector3(minX, minY - 1, 0);
        m_Temp2Verts[3].color = Color.black;
        //m_Temp2Verts[3].uv0 = new Vector4(0.0f, 0.08f, 0, 0);

        toFill.AddUIVertexQuad(m_Temp2Verts);


        m_DisableFontTextureRebuiltCallback = false;
    }

}
