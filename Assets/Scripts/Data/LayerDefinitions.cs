using System.Collections.Generic;
using UnityEngine;

namespace Coffee
{
    public enum Layers
    {
        Wall,
        Interactible,
        Player,
    }
    
    public static class LayerDefinitions
    {

        public static LayerMask GetLayerMask(IEnumerable<Layers> layers)
        {
            int mask = 0;
            foreach (var layer in layers)
            {
                mask |= 1 << LayerMask.NameToLayer(layer.ToString());
            }

            return mask;
        }
    }
}