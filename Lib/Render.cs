using UnityEngine;
using Veauty.VTree;

namespace Veauty.GameObject 
{
    public static class Renderer
    {
        public static UnityEngine.GameObject Render(IVTree vTree, bool isUGUI)
        {
            UnityEngine.GameObject go = null;
            switch (vTree)
            {
                case BaseNode vNode:
                    go = CreateNode(vNode, isUGUI);
                    break;
                case BaseKeyedNode vNode:
                    go = CreateNode(vNode, isUGUI);
                    break;
                case Widget widget:
                    go = widget.Init(Render(widget.Render(), isUGUI)); 
                    break;
            }

            if (isUGUI)
            {
                var rectTransform = go.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }

            return go;
        }

        private static UnityEngine.GameObject CreateGameObject(string name)
        {
            var go = new UnityEngine.GameObject(name);
            return go;
        }

        private static UnityEngine.GameObject CreateNode(BaseNode vNode, bool isUGUI)
        {
            var go = CreateGameObject(vNode.tag);

            if (vNode is ITypedNode node)
            {
                go.AddComponent(node.GetComponentType());
            }
            
            ApplyAttrs(go, vNode.attrs);

            foreach (var kid in vNode.kids)
            {
                AppendChild(go, Render(kid, isUGUI));
            }
            
            return go;
        }

        private static UnityEngine.GameObject CreateNode(BaseKeyedNode vNode, bool isUGUI)
        {
            var go = CreateGameObject(vNode.tag);
            
            if (vNode is ITypedNode node)
            {
                go.AddComponent(node.GetComponentType());
            }
            
            ApplyAttrs(go, vNode.attrs);

            foreach (var kid in vNode.kids)
            {
                AppendChild(go, Render(kid.Item2, isUGUI));
            }

            return go;
        }

        private static void AppendChild(UnityEngine.GameObject parent, UnityEngine.GameObject kid)
        {
            kid.transform.SetParent(parent.transform, false);
        }

        private static void ApplyAttrs(UnityEngine.GameObject go, Attributes attrs)
        {
            foreach (var attr in attrs.attrs)
            {
                attr.Value.Apply(go);
            }
        }

    }
}