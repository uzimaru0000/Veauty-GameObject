using UnityEngine;

namespace Veauty.GameObject
{
    public class VeautyObject<State> where State : struct 
    {
        private IVTree oldTree;
        private readonly UnityEngine.GameObject mounter;
        private readonly System.Func<State, System.Action<State>, IVTree> renderFunc;
        private UnityEngine.GameObject rootObj;
        private bool isUGUI;

        private State _state;
        private State state
        {
            get => this._state;
            set {
                this._state = value;
                ForceUpdate();
            }
        }
        
        public VeautyObject(UnityEngine.GameObject rootObj, System.Func<State, System.Action<State>, IVTree> renderFunc, State state = default(State))
        {
            this.mounter = rootObj;
            this.renderFunc = renderFunc;
            this._state = state;
            this.oldTree = renderFunc(this.state, state => this.state = state);
            this.isUGUI = rootObj.transform is RectTransform;

            Render();
        }

        public VeautyObject(
            UnityEngine.GameObject rootObj,
            System.Func<State, IVTree> renderFunc,
            State state = default(State)
        ) : this(rootObj, (state, _) => renderFunc(state), state) { }

        public void ForceUpdate()
        {
            var newTree = this.renderFunc(this.state, state => this.state = state);
            var patches = Diff<UnityEngine.GameObject>.Calc(this.oldTree, newTree);

            this.rootObj = Patch.Apply(this.rootObj, this.oldTree, patches, this.isUGUI);
            this.oldTree = newTree;
        }

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