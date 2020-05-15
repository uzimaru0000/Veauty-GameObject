using Veauty.VTree;

namespace Veauty.GameObject
{
    public class VeautyObject
    {
        private IVTree oldTree;
        private readonly UnityEngine.GameObject mounter;
        private readonly System.Func<IVTree> renderFunc;
        private UnityEngine.GameObject rootObj;
        
        public VeautyObject(UnityEngine.GameObject rootObj, System.Func<IVTree> renderFunc)
        {
            this.mounter = rootObj;
            this.renderFunc = renderFunc;
            this.oldTree = renderFunc();
            
            Render();
        }

        public System.Action<T> SetState<T>(System.Action<T> update) =>
            (state) =>
            {
                update(state);
                var newTree = renderFunc();
                var patches = Diff.Calc(oldTree, newTree);
                this.rootObj = Patch.Apply(rootObj, oldTree, patches);
            };

        private void Render()
        {
            this.rootObj = Renderer.Render(this.oldTree);
            this.rootObj.transform.SetParent(this.mounter.transform, false);
        }
    }
}