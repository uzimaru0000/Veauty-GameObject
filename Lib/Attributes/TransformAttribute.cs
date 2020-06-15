using UnityEngine;

namespace Veauty.GameObject.Attributes
{
    public class Position : Attribute<Vector3>
    {
        private bool isLocal;
        
        public Position(Vector3 pos, bool isLocal = false) : base("Position", pos)
        {
            this.isLocal = isLocal;
        }

        public override void Apply(UnityEngine.GameObject obj)
        {
            if (this.isLocal)
            {
                obj.transform.localPosition = this.GetValue();
            }
            else
            {
                obj.transform.position = this.GetValue();
            }
        }
    }

    public class Rotation : Attribute<Quaternion>
    {
        private bool isLocal;
        
        public Rotation(Quaternion rot, bool isLocal = false) : base("Rotation", rot)
        {
            this.isLocal = isLocal;
        }

        public override void Apply(UnityEngine.GameObject obj)
        {
            if (this.isLocal)
            {
                obj.transform.rotation = this.GetValue();
            }
            else
            {
                obj.transform.localRotation = this.GetValue();
            }
        }
    }

    public class Scale : Attribute<Vector3>
    {
        public Scale(Vector3 scale) : base("Scale", scale)
        {
        }

        public override void Apply(UnityEngine.GameObject obj)
        {
            obj.transform.localScale = this.GetValue();
        }
    }
}