using UnityEngine;

namespace Coffee.HouseGen
{
    public static class ObjectHelpers
    {
        public static Transform FindParent(string parentName)
        {
            var parents = GameObject.FindGameObjectsWithTag(TagDefnitions.GeneratorParent);
            foreach (var parent in parents)
            {
                if (parent.name == parentName)
                {
                    return parent.transform;
                }
            }

            var newParent = new GameObject(parentName).transform;
            newParent.tag = TagDefnitions.GeneratorParent;
            return newParent;
        }
    }
}