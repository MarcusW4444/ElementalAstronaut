using System.Collections.Generic;
using UnityEngine;



public class CubedGeometry : MonoBehaviour {

    public class GeoNode
    {
        public enum GeoNodeType {Space,Block,Incline};
        //you can't use an incline unless its connected diagonally.
        public int PositionX=0, PositionY=0;
        public bool Solid = true;
        public GeoCube MyCube;
        public GeoNode NeighborT = null, NeighborTR = null, NeighborR = null, NeighborBR = null, NeighborB = null, NeighborBL = null, NeighborL = null, NeighborTL=null;
    }
    public List<GeoNode> NodeList=new List<GeoNode>();
    public Sprite GeometryTexture;
    public GeoCube GeoCubePrefab, InclinePrefab;
    
    private void Start()
    {

    }

    private void Update()
    {

    }

    public void updateGeoNode(GeoNode n)
    {

    }

    public void addGeoNode(int x,int y,GeoNode.GeoNodeType t)
    {

    }
    public void removeGeoNode(GeoNode n)
    {
        

    }
    public GeoNode getGeoNodeAtPosition(int posx, int posy)
    {
        foreach (GeoNode n in NodeList)
        {
            if ((n.PositionX == posx) && (n.PositionY == posy))
            {
                return n; //There shouldn't be more than one node in the same spot
            }
        }
        return null;
    }
    public void OnButtonTriggered(GeoNode geonode, int offsetx,int offsety) //
    {

        GeoNode n = getGeoNodeAtPosition(geonode.PositionX+offsetx, geonode.PositionY + offsety);

        Debug.Log(""+(geonode != null)+" + "+offsetx+", "+offsety);


    }

}