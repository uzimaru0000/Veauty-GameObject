using System.Collections.Generic;
using UnityEngine;
using Veauty.VTree;
using Veauty.Patch;

namespace Veauty.GameObject
{
    public static class Patch
    {
        public static UnityEngine.GameObject Apply(UnityEngine.GameObject rootGameObject, IVTree oldVTree, IPatch<UnityEngine.GameObject>[] patches, bool isUGUI)
        {
            if (patches.Length == 0)
            {
                return rootGameObject;
            }

            AddGameObjectNodes(rootGameObject, oldVTree, ref patches);

            return Helper(rootGameObject, patches, isUGUI);
        }

        private static void AddGameObjectNodes(in UnityEngine.GameObject gameObjectNode, IVTree vTree, ref IPatch<UnityEngine.GameObject>[] patches)
        {
            AddGameObjectNodesHelper(gameObjectNode, vTree, ref patches, 0, 0, vTree.GetDescendantsCount());
        }

        private static int AddGameObjectNodesHelper(
            in UnityEngine.GameObject gameObjectNode,
            IVTree vTree,
            ref IPatch<UnityEngine.GameObject>[] patches,
            int i,
            int low,
            int high
        )
        {
            var patch = patches[i];
            var index = patch.GetIndex();

            while (index == low)
            {
                switch (patch)
                {
                    case Reorder<UnityEngine.GameObject> reorder:
                        {
                            reorder.SetTarget(gameObjectNode);

                            var subPatches = reorder.patches;
                            if (subPatches.Length > 0)
                            {
                                AddGameObjectNodesHelper(gameObjectNode, vTree, ref subPatches, 0, low, high);
                            }

                            break;
                        }
                    case Remove<UnityEngine.GameObject> remove:
                        {
                            remove.SetTarget(gameObjectNode);

                            if (remove.patches != null && remove.entry != null)
                            {
                                remove.entry.data = gameObjectNode;
                                var subPatches = remove.patches;
                                if (subPatches.Length > 0)
                                {
                                    AddGameObjectNodesHelper(gameObjectNode, vTree, ref subPatches, 0, low, high);
                                }
                            }

                            break;
                        }
                    default:
                        patch.SetTarget(gameObjectNode);
                        break;
                }

                i++;

                if (i < patches.Length)
                {
                    patch = patches[i];
                    index = patch.GetIndex();
                    if (index > high)
                    {
                        return i;
                    }
                }
                else
                {
                    return i;
                }
            }

            if (vTree is IParent parent)
            {
                var kids = parent.GetKids();
                var children = gameObjectNode.transform;
                for (var j = 0; j < kids.Length; j++)
                {
                    low++;
                    var kid = kids[j];
                    var nextLow = low + kid.GetDescendantsCount();
                    if (low <= index && index <= nextLow)
                    {
                        i = AddGameObjectNodesHelper(children.GetChild(j).gameObject, kid, ref patches, i, low, nextLow);

                        if (i < patches.Length)
                        {
                            patch = patches[i];
                            index = patch.GetIndex();
                            if (index > high)
                            {
                                return i;
                            }
                        }
                        else
                        {
                            return i;
                        }
                    }

                    low = nextLow;
                }
            }

            return i;
        }

        private static UnityEngine.GameObject Helper(UnityEngine.GameObject rootGameObject, IPatch<UnityEngine.GameObject>[] patches, bool isUGUI)
        {
            foreach (var patch in patches)
            {
                var localGameObject = patch.GetTarget();
                var newGameObject = ApplyPatch(localGameObject, patch, isUGUI);
                if (localGameObject == rootGameObject)
                {
                    rootGameObject = newGameObject;
                }
            }

            return rootGameObject;
        }

        private static UnityEngine.GameObject ApplyPatch(UnityEngine.GameObject go, IPatch<UnityEngine.GameObject> patch, bool isUGUI)
        {
            switch (patch)
            {
                case Redraw<UnityEngine.GameObject> redraw:
                    {
                        return ApplyPatchRedraw(go, redraw.vTree, isUGUI);
                    }
                case Attrs<UnityEngine.GameObject> attrs:
                    {
                        return ApplyAttrs(go, attrs.attrs);
                    }
                case RemoveLast<UnityEngine.GameObject> removeLast:
                    {
                        for (var i = 0; i < removeLast.diff; i++)
                        {
                            var child = go.transform.GetChild(removeLast.length);
                            UnityEngine.Object.Destroy(child.gameObject);
                        }
                        return go;
                    }
                case Append<UnityEngine.GameObject> append:
                    {
                        var kids = append.kids;
                        var i = append.length;

                        for (; i < kids.Length; i++)
                        {
                            var node = Renderer.Render(kids[i], isUGUI);
                            node.transform.SetParent(go.transform);
                        }
                        return go;
                    }
                case Remove<UnityEngine.GameObject> remove:
                    {
                        if (remove.entry == null && remove.patches == null)
                        {
                            UnityEngine.Object.Destroy(go);
                            return null;
                        }

                        if (remove.entry.index != -1)
                        {
                            go.transform.SetParent(null);
                        }

                        remove.entry.data = Helper(go, remove.patches, isUGUI);
                        return go;
                    }
                case Reorder<UnityEngine.GameObject> reorder:
                    {
                        return ApplyPatchReorder(go, reorder, isUGUI);
                    }
                case Attach<UnityEngine.GameObject> attach:
                    {
                        return ApplyPatchAttach(go, attach);
                    }
            }
            return go;
        }

        private static UnityEngine.GameObject ApplyPatchRedraw(UnityEngine.GameObject go, IVTree vTree, bool isUGUI)
        {
            var parent = go.transform.parent;
            var newNode = Renderer.Render(vTree, isUGUI);

            if (parent && newNode != go)
            {
                UnityEngine.Object.Destroy(go);
                newNode.transform.SetParent(parent, false);
            }

            return newNode;
        }

        private static UnityEngine.GameObject ApplyAttrs(UnityEngine.GameObject go, Dictionary<string, IAttribute<UnityEngine.GameObject>> attrs)
        {
            foreach (var attr in attrs)
            {
                attr.Value.Apply(go);
            }

            return go;
        }

        private static UnityEngine.GameObject ApplyPatchReorder(UnityEngine.GameObject go, Reorder<UnityEngine.GameObject> patch, bool isUGUI)
        {
            var frag = ApplyPatchReorderEndInsertsHelper(patch.endInserts, patch, isUGUI);

            go = Helper(go, patch.patches, isUGUI);

            foreach (var insert in patch.inserts)
            {
                var entry = insert.entry;
                var node = entry.tag == Entry.Type.Move ? entry.data as UnityEngine.GameObject : Renderer.Render(entry.vTree, isUGUI);
                node.transform.SetParent(go.transform);
                node.transform.SetSiblingIndex(insert.index);
            }

            if (frag != null)
            {
                foreach (var child in frag)
                {
                    child.transform.SetParent(go.transform);
                }
            }

            return go;
        }

        private static UnityEngine.GameObject[] ApplyPatchReorderEndInsertsHelper(Reorder<UnityEngine.GameObject>.Insert[] endInserts, Reorder<UnityEngine.GameObject> patch, bool isUGUI)
        {
            if (endInserts.Length == 0)
            {
                return null;
            }

            var frag = new UnityEngine.GameObject[endInserts.Length];
            for (var i = 0; i < endInserts.Length; i++)
            {
                var insert = endInserts[i];
                var entry = insert.entry;
                frag[i] = entry.tag == Entry.Type.Move ? entry.data as UnityEngine.GameObject : Renderer.Render(entry.vTree, isUGUI);
            }

            return frag;
        }

        private static UnityEngine.GameObject ApplyPatchAttach(UnityEngine.GameObject go, Attach<UnityEngine.GameObject> attach)
        {
            var old = go.GetComponent(attach.oldComponent) as MonoBehaviour;
            UnityEngine.Object.Destroy(old);

            go.AddComponent(attach.newComponent);

            return go;
        }
    }
}
