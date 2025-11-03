using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(CompositeCollider2D))]
public class BoundsHandler : MonoBehaviour
{
    public Mesh ColliderMesh;
    public MeshFilter[] Filters;
    public Rect BoundsRect;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RegenerateMesh();
        PassMeshToFilter();

    }


    private void OnValidate()
    {

        RegenerateMesh();
        PassMeshToFilter();
    }



    [ContextMenu("Regen Mesh")]
    public void RegenerateMesh()
    {
        CompositeCollider2D c = GetComponent<CompositeCollider2D>();

        


        ColliderMesh = c.CreateMesh(false,false);

    }

    [ContextMenu("Pass Mesh")]

    public void PassMeshToFilter()
    {

        for (int i = 0; i < Filters.Length; i++)
        {
            if (Filters[i] != null)
            {
                Filters[i].sharedMesh = ColliderMesh;

            }


        }


    }


}
