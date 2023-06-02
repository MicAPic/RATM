using UnityEngine;

namespace Utility
{
    public class ForcedCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // modified from http://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html
            
            // set the desired aspect ratio (the values in this example are
            // hard-coded for 16:9, but you could make them into public
            // variables instead so you can set them at design time)
            const float targetRatio = 16.0f / 9.0f;

            // determine the game window's current aspect ratio
            var currentRatio = (float) Screen.width / Screen.height;

            // current viewport height should be scaled by this amount
            var scaleHeight = currentRatio / targetRatio;

            // obtain camera component so we can modify its viewport
            var cam = GetComponent<Camera>();

            // if scaled height is less than current height, add letterbox
            if (scaleHeight < 1.0f)
            {  
                var rect = cam.rect;

                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
        
                cam.rect = rect;
            }
            else // add pillarbox
            {
                var scaleWidth = 1.0f / scaleHeight;

                var rect = cam.rect;

                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;

                cam.rect = rect;
            }
        }
    }
}
