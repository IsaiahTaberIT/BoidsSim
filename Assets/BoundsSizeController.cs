using UnityEngine;

public class BoundsSizeController : MonoBehaviour
{
    public Vector2 Size = Vector2.one;
    public BoundsHandler Handler;
    public float EdgeWidth;
    public GameObject InnerPanel;




    private void OnValidate()
    {
        Handler.BoundsRect = new(transform.position - (Vector3)Size / 2f, Size);
        Handler.transform.localScale = Size;
        Handler.RegenerateMesh();
        Handler.PassMeshToFilter();
        Handler.GetComponent<CompositeCollider2D>().edgeRadius = EdgeWidth/2;
        InnerPanel.transform.localScale = (Vector2.one - (Vector2.one * EdgeWidth) / Size * 2);

    }
}
