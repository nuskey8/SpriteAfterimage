using UnityEngine;

public static class Physics2DUtility {
    public static float Calculate2DDistanceToLayer(Vector2 startPos, Vector2 dir, float maxDistance, int layerMask) {
        RaycastHit2D hit = Physics2D.Raycast (startPos, dir, maxDistance, layerMask);
        float dist = PixelArtPackConstants.NO_HIT_DISTANCE;
        if (null != hit.collider) {
            dist = hit.distance;
        }
        return dist;
    }

}
