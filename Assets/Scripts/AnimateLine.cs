using UnityEngine;

public class AnimateLine : MonoBehaviour
{
    public float Speed = 2.0f;

    private Material _material;

    protected void Start()
    {
        _material = GetComponent<LineRenderer>().material;
    }

	protected void Update()
	{
        var offset = _material.mainTextureOffset;
	    offset.x = Mathf.Repeat(offset.x - Speed * Time.deltaTime, 1f);
	    _material.mainTextureOffset = offset;
	}
}
