using Godot;

public partial class KeepName : Typeable
{
    public KeepName(KeepMono keep) : base(keep.Keep.Name)
    {
        GD.Print("KeepName constructor " + keep.Keep.Name);
    }
}