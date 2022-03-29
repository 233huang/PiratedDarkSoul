using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDissolve : MonoBehaviour
{
    public GameObject root;
    public float dissloveTime = 3f;

    //private SkinnedMeshRenderer[] _renderers;
    private Material material;
    void Start()
    {
        //_renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>();
        material = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            StartCoroutine(Dissolve());
        }
    }

    private IEnumerator Dissolve()
    {
        SetDissloveRate(0);

        float time = 0f;
        while (time < dissloveTime)
        {
            time += Time.deltaTime;
            SetDissloveRate(time / dissloveTime);
            yield return null;
        }
    }

    private void SetDissloveRate(float value)
    {
        /*int shaderId = Shader.PropertyToID("_ClipRate");
        foreach(SkinnedMeshRenderer meshRenderer in _renderers)
        {
            foreach(Material material in meshRenderer.materials)
            {
                material.SetFloat(shaderId, value);
            }
        }*/

        material.SetFloat("_ClipRate", value);
    }
}
