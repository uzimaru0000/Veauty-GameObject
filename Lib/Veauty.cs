using UnityEngine;

namespace Veauty.GameObject
{
    public class VeautyObject
    {
        private IVTree oldTree;
        private readonly UnityEngine.GameObject mounter;
        private readonly System.Func<IVTree> renderFunc;
        private UnityEngine.GameObject rootObj;
        private bool isUGUI;
        
        public VeautyObject(UnityEngine.GameObject rootObj, System.Func<IVTree> renderFunc)
        {
            this.mounter = rootObj;
            this.renderFunc = renderFunc;
            this.oldTree = renderFunc();
            this.isUGUI = rootObj.transform is RectTransform;
            
            Render();
        }

        public System.Action<T> SetState<T>(System.Action<T> update) =>
            (state) =>
            {
                update(state);
                var newTree = this.renderFunc();
                var patches = Diff<UnityEngine.GameObject>.Calc(this.oldTree, newTree);
                this.rootObj = Patch.Apply(this.rootObj, this.oldTree, patches, this.isUGUI);
            };

        private void Render()
        {
            this.rootObj = Renderer.Render(this.oldTree, this.isUGUI);
            this.rootObj.transform.SetParent(this.mounter.transform, false);
            if (this.isUGUI)
            {
                var rect = this.rootObj.transform as RectTransform;
                rect.sizeDelta = Vector2.zero;
            }
        }
    }
}