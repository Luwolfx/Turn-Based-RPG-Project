using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;

public class GridVisualQuad : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }

    public void ChangeMaterial(Material material)
    {
        meshRenderer.material = material;
    }
}
