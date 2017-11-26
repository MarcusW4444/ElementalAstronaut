using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubedGeometry)), CanEditMultipleObjects]
class ButtonExampleEditor : Editor
{
    private CubedGeometry.GeoNode.GeoNodeType lastSetType = CubedGeometry.GeoNode.GeoNodeType.Block;
    protected virtual void OnSceneGUI()
    {
        CubedGeometry geometry = (CubedGeometry)target;

        //Vector3 position = buttonExample.transform.position + Vector3.up * 2f;
        float size = Mathf.Min(geometry.transform.localScale.x, geometry.transform.localScale.y, geometry.transform.localScale.z);
        float pickSize = size * 1f;

        CubedGeometry.GeoNode nodepressed = null;
        bool nonodes = true;
        int offsetx = 0;
        int offsety = 0;
        foreach (CubedGeometry.GeoNode node in geometry.NodeList)
        {
            if (Handles.Button(node.MyCube.transform.position-Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
            {
                nodepressed = node;
                offsetx = 0;
                offsety = 0;
            }


            if (node.NeighborT == null)
                if (Handles.Button((node.MyCube.transform.position+new Vector3(0f, size, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = 0;
                    offsety = 1;
                }

            if (node.NeighborTR == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(size, size, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = 1;
                    offsety = 1;
                }

            if (node.NeighborR == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(size, 0f, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = 1;
                    offsety = 0;
                }

            if (node.NeighborBR == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(size, -size, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = 1;
                    offsety = -1;
                }
            if (node.NeighborB == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(0f, -size, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = 0;
                    offsety = -1;
                }

            if (node.NeighborBL == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(-size, -size, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = -1;
                    offsety = -1;
                }

            if (node.NeighborL == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(-size, 0f, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = -1;
                    offsety = 0;
                }

            if (node.NeighborTL == null)
                if (Handles.Button((node.MyCube.transform.position + new Vector3(-size, size, 0f)) - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
                {
                    nodepressed = node;
                    offsetx = -1;
                    offsety = 1;
                }

            //Debug.Log("The button was pressed!");

            nonodes = false;
        }

        if (nodepressed != null)
        {
            geometry.OnButtonTriggered(nodepressed, offsetx, offsety);
        } else if (nonodes)
        {

            if (Handles.Button(geometry.transform.position - Vector3.forward, Quaternion.identity, size, pickSize, Handles.RectangleHandleCap))
            {
                geometry.OnButtonTriggered(null, 0, 0);
            }
        }

        
        

    }
}