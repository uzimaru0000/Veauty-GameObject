using UnityEngine;
using Veauty.VTree;

namespace Veauty.GameObject 
{
    public static class Renderer
    {
        public static UnityEngine.GameObject Render(IVTree vTree)
        {
            switch (vTree)
            {
                case BaseNode vNode:
                    return CreateNode(vNode);
                case BaseKeyedNode vNode:
                    return CreateNode(vNode);
                case Widget widget:
                    return widget.Init(Render(widget.Render())); 
                default:
                    return null;
            }
        }

        private static UnityEngine.GameObject CreateGameObject(string name)
        {
            var go = new UnityEngine.GameObject(name);
            return go;
        }

        private static UnityEngine.GameObject CreateNode(BaseNode vNode)
        {
            var go = CreateGameObject(vNode.tag);

            if (vNode is ITypedNode node)
            {
                go.AddComponent(node.GetComponentType());
            }
            
            ApplyAttrs(go, vNode.attrs);

            foreach (var kid in vNode.kids)
            {
                AppendChild(go, Render(kid));
            }
            
            return go;
        }

        private static UnityEngine.GameObject CreateNode(BaseKeyedNode vNode)
        {
            var go = CreateGameObject(vNode.tag);
            
            if (vNode is ITypedNode node)
            {
                go.AddComponent(node.GetComponentType());
            }
            
            ApplyAttrs(go, vNode.attrs);

            foreach (var kid in vNode.kids)
            {
                AppendChild(go, Render(kid.Item2));
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