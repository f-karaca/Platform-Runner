using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPB_BaseTexture : MPB
{
    [SerializeField] private Texture2D baseTexture;

    private void Update()
    {
        if(baseTexture != null)
        {
            _materialPropertyBlock.SetTexture("_BaseMap", baseTexture);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }
       
    }
}
