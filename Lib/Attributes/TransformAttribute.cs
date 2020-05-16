using UnityEngine;

namespace Veauty.GameObject.Attributes
{
    public interface ITransform : IAttribute { }
    
    public class Position : ITransform
    {
        private Vector3 pos;
        private bool isLocal;
        
        public Position(Vector3 pos, bool isLocal = false)
        {
            this.pos = pos;
            this.isLocal = isLocal;
        }

        public string GetKey() => "Position";

        public void Apply(UnityEngine.GameObject obj)
        {
            if (this.isLocal)
            {
                obj.transform.localPosition = this.pos;
            }
            else
            {
                obj.transform.position = this.pos;
            }
        }

        public bool Equals(IAttribute attr)
        {
            if (attr is Position other)
            {
                return this.pos == other.pos && this.isLocal == other.isLocal;
            }

            return false;
        }
    }

    public class Rotation : ITransform
    {
        private Quaternion rot;
        private bool isLocal;
        
        public Rotation(Quaternion rot, bool isLocal = false)
        {
            this.rot = rot;
            this.isLocal = isLocal;
        }

        public string GetKey() => "Rotation";

        public void Apply(UnityEngine.GameObject obj)
        {
            if (this.isLocal)
            {
                obj.transform.rotation = rot;
            }
            else
            {
                obj.transform.localRotation = rot;
            }
        }

        public bool Equals(IAttribute attr)
        {
            if (attr is Rotation other)
            {
                return this.rot == other.rot;
            }

            return false;
        }
    }

    public class Scale : ITransform
    {
        private Vector3 scale;

        public Scale(Vector3 scale)
        {
            this.scale = scale;
        }

        public string GetKey() => "Scale";

        public void Apply(UnityEngine.GameObject obj)
        {
            obj.transform.localScale = this.scale;
        }

        public bool Equals(IAttribute attr)
        {
            if (attr is Scale other)
            {
                return this.scale == other.scale;
            }

            return false;
        }
    }
}