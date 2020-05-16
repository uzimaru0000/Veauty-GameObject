namespace Veauty.GameObject.Attributes
{
    public interface IGameObject : IAttribute { }

    public class Active : IGameObject
    {
        private bool isActive;

        public Active(bool isActive)
        {
            this.isActive = isActive;
        }
        
        public string GetKey() => "Active";

        public void Apply(UnityEngine.GameObject obj)
        {
            obj.SetActive(this.isActive);
        }

        public bool Equals(IAttribute attr)
        {
            if (attr is Active other)
            {
                return this.isActive == other.isActive;
            }

            return false;
        }
    }
}