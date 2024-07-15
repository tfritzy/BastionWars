using Godot;

public partial class KeepName : Typeable
{
    // public override string CompletedTextColor => "#9d4343";
    // public override string UnCompletedTextColor => "#fceba8";
    // public override string OutlineColor => "#2a152d";

    public KeepName(KeepMono keep) : base(keep.Keep.Name)
    {
        GD.Print("KeepName constructor " + keep.Keep.Name);

        // var styleBox = new StyleBoxFlat()
        // {
        //     CornerRadiusTopLeft = 50,
        //     CornerRadiusBottomRight = 50,
        //     CornerRadiusBottomLeft = 50,
        //     CornerRadiusTopRight = 50,
        //     BgColor = new Color("#2e2e43"),
        //     ExpandMarginLeft = 13,
        //     ExpandMarginRight = 13,
        //     ExpandMarginTop = 3,
        //     ExpandMarginBottom = 3,
        //     BorderColor = new Color("#f5c47c"),
        //     BorderWidthBottom = 2,
        //     BorderWidthLeft = 2,
        //     BorderWidthTop = 2,
        //     BorderWidthRight = 2,
        // };

        // AddThemeStyleboxOverride("normal", styleBox);

    }
}