using System;
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
                case NodeBase vNode:
                    go = CreateGameObject(vNode.tag, isUGUI);
                    AttachComponent(go, vNode, isUGUI);
                    ApplyAttrs(go, vNode);
                    RenderKids(go, vNode, isUGUI);
                    return go;
                case Widget widget:
                    return RenderWidget(widget, isUGUI);          
                default:
                    throw new Exception("Invalid node type");
            }
        }

        private static UnityEngine.GameObject RenderWidget(Widget widget, bool isUGUI)
        {
            var tree = widget.Render();
            switch (tree)
            {
                case Widget nest:
                    return Render(nest, isUGUI);
                case BaseNode node:
                    var go = CreateGameObject(node.tag, isUGUI);
                    AttachComponent(go, node, isUGUI);
                    widget.Init(go);
                    ApplyAttrs(go, node);
                    RenderKids(go, node, isUGUI);
                    return go;
                default:
                    throw new Exception("Invalid node type");
            }
        }

        private static UnityEngine.GameObject CreateGameObject(string name, bool isUGUI)
        {
            var go = new UnityEngine.GameObject(name);
            if (isUGUI)
            {
                var rectTransform = go.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }
            
            return go;
        }

        private static void AttachComponent(UnityEngine.GameObject go, IVTree tree, bool isUGUI)
        {
            if (!(tree is ITypedNode node)) return;
            go.AddComponent(node.GetComponentType());
        }

        private static void RenderKids(UnityEngine.GameObject go, IVTree tree, bool isUGUI)
        {
            if (!(tree is IParent parent)) return;
            foreach (var kid in parent.GetKids())
            {
                AppendChild(go, Render(kid, isUGUI));
            }
        }

        private static void AppendChild(UnityEngine.GameObject parent, UnityEngine.GameObject kid)
        {
            kid.transform.SetParent(parent.transform);
        }

        private static void ApplyAttrs(UnityEngine.GameObject go, IVTree tree)
        {
            if (!(tree is NodeBase node)) return;
            foreach (var attr in node.attrs.attrs)
            {
                attr.Value.Apply(go);
            }
        }

    }
}