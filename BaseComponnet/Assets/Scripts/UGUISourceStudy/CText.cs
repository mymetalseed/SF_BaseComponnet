using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CText : Text
{
    [SerializeField]
    public float rotAlpha
    {
        get
        {
            return _rotAlpha;
        }
        set
        {
            _rotAlpha = value;
            SetVerticesDirty();
        }
    }

    private float _rotAlpha;

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
        float height = 0, width = 0,originSlope = 0;
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

        height = maxY - minY;
        width = maxX - minX;
        originSlope = height / width;
        originSlope = Mathf.Tan(Mathf.Deg2Rad * _rotAlpha);
        Debug.LogError(originSlope);
        float invSlope = 1 / originSlope;
        Vector2 centerPos = new Vector2(width/2,height/2);
        float A = invSlope, B = -1,C=maxY- invSlope*minX,B2=1,A2=invSlope*invSlope;

        float fullDist = 2*Mathf.Abs((A * centerPos.x + B * centerPos.y + C)/Mathf.Sqrt(A2+B2));
        UIVertex tempVert = new UIVertex();
        for(int i = 0; i < toFill.currentVertCount; ++i)
        {
            toFill.PopulateUIVertex(ref tempVert, i);
            float posX = tempVert.position.x, posY = tempVert.position.y;
            float nowDist = 2 * Mathf.Abs((A * posX + B * posY + C) / Mathf.Sqrt(A2 + B2));
            float lv = Mathf.Clamp01(nowDist / fullDist);
            tempVert.color = Color.Lerp(Color.cyan, Color.black, lv);
            toFill.SetUIVertex(tempVert, i);
        }

        TextGenerator underlineText = new TextGenerator();
        underlineText.Populate("*", settings);
        IList<UIVertex> lineVer = underlineText.verts;

        Vector2 uv = Vector2.zero;
        for(int i = 0; i < 4; ++i)
        {
            uv.x = uv.x + lineVer[i].uv0.x;
            uv.y = uv.y + lineVer[i].uv0.y;
        }
        uv /= 4;


        Color tempColor = Color.black;
        minX *= 1.5f;
        maxX *= 1.5f;
        Debug.Log(minX.ToString()+" " +minY.ToString()+" " +maxX.ToString() + " " + maxY.ToString());
        m_Temp2Verts[0] = lineVer[0];
        m_Temp2Verts[0].position = new Vector3(minX, minY, 0);
        m_Temp2Verts[0].color = Color.black;
        m_Temp2Verts[0].uv0 = uv;

        m_Temp2Verts[1] = lineVer[1];
        m_Temp2Verts[1].position = new Vector3(maxX, minY, 0);
        m_Temp2Verts[1].color = Color.black;
        m_Temp2Verts[1].uv0 = uv;

        m_Temp2Verts[2] = lineVer[2];
        m_Temp2Verts[2].position = new Vector3(maxX, minY - 1, 0);
        m_Temp2Verts[2].color = Color.black;
        m_Temp2Verts[2].uv0 = uv;

        m_Temp2Verts[3] = lineVer[3];
        m_Temp2Verts[3].position = new Vector3(minX, minY - 1, 0);
        m_Temp2Verts[3].color = Color.black;
        m_Temp2Verts[3].uv0 = uv;

        toFill.AddUIVertexQuad(m_Temp2Verts);


        m_DisableFontTextureRebuiltCallback = false;
    }




}
