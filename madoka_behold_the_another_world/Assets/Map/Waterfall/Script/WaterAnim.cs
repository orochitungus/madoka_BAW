using UnityEngine;
using System.Collections;

public class WaterAnim : MonoBehaviour
{
    private int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2( 0.0f, 1.0f );
    public string textureName = "_MainTex";

    Vector2 uvOffset = Vector2.zero;

    void LateUpdate()
    {
        uvOffset += ( uvAnimationRate * Time.deltaTime );
        if( GetComponent<Renderer>().enabled )
        {
            GetComponent<Renderer>().materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
        }
    }
}