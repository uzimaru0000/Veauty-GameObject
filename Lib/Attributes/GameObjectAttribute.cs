namespace Veauty.GameObject.Attributes
{
    public class Active : Attribute<bool>
    {
        public Active(bool isActive) : base("Active", isActive) { }
        
        public override void Apply(UnityEngine.GameObject obj)
        {
            obj.SetActive(this.GetValue());
        }
    }
}