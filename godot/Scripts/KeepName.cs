using Godot;

public partial class KeepName : Typeable
{

    public override string CompletedTextColor => "#2a152d";
    public override string UnCompletedTextColor => "#2a152d88";
    public override string OutlineColor => "#2a152d";
    public override int FontSize => 22;

    public KeepName(KeepMono keep) : base(keep.Keep.Name)
    {
        GD.Print("KeepName constructor " + keep.Keep.Name);

        var styleBox = new StyleBoxFlat()
        {
            CornerRadiusTopLeft = 5,
            CornerRadiusBottomRight = 5,
            CornerRadiusBottomLeft = 5,
            CornerRadiusTopRight = 5,
            BgColor = new Color("#fcfbf3"),
            ExpandMarginLeft = 7,
            ExpandMarginRight = 7,
            ExpandMarginTop = 3,
            ExpandMarginBottom = 3,
            BorderColor = new Color("#2e2e43"),
            BorderWidthBottom = 1,
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
        };

        AddThemeStyleboxOverride("normal", styleBox);

    }
}