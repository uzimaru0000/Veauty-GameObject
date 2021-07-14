namespace Veauty.GameObject.Attributes
{
    public class Active : Attribute<UnityEngine.GameObject, bool>
    {
        public Active(bool isActive) : base("Active", isActive) { }
        
        public override void Apply(UnityEngine.GameObject obj)
        {
            obj.SetActive(this.GetValue());
        }
    }

    public class Tag : Attribute<UnityEngine.GameObject, string>
    {
        public Tag(string tag) : base("Tag", tag) {}

        public override void Apply(UnityEngine.GameObject obj)
        {
            obj.tag = this.GetValue();
        }
    }
}