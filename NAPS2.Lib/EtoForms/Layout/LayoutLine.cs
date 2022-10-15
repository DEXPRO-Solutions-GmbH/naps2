using Eto.Drawing;

namespace NAPS2.EtoForms.Layout;

/// <summary>
/// Abstract base class for LayoutColumn and LayoutRow. We use this class to generalize column and row layout logic.
/// </summary>
/// <typeparam name="TOrthogonal">The orthogonal type (e.g. LayoutRow if this is LayoutColumn).</typeparam>
public abstract class LayoutLine<TOrthogonal> : LayoutContainer
    where TOrthogonal : LayoutContainer
{
    protected int? Spacing { get; init; }

    protected abstract PointF UpdatePosition(PointF position, float delta);

    protected abstract SizeF UpdateTotalSize(SizeF size, SizeF childSize, int spacing);

    public override void DoLayout(LayoutContext context, RectangleF bounds)
    {
        if (DEBUG_LAYOUT)
        {
            Debug.WriteLine($"{new string(' ', context.Depth)}{GetType().Name} layout with bounds {bounds}");
        }
        var childContext = GetChildContext(context, bounds);
        GetInitialCellLengthsAndScaling(context, childContext, bounds, out var cellLengths, out var cellScaling);

        var spacing = Spacing ?? context.DefaultSpacing;
        UpdateCellLengthsForAvailableSpace(cellLengths, cellScaling, bounds, spacing);

        var cellOrigin = bounds.Location;
        for (int i = 0; i < Children.Length; i++)
        {
            var cellSize = GetSize(cellLengths[i], GetBreadth(bounds.Size));
            Children[i].DoLayout(childContext, new RectangleF(cellOrigin, cellSize));
            cellOrigin = UpdatePosition(cellOrigin, GetLength(cellSize) + spacing);
        }
    }

    public override SizeF GetPreferredSize(LayoutContext context, RectangleF parentBounds)
    {
        var childContext = GetChildContext(context, parentBounds);
        var size = SizeF.Empty;
        GetInitialCellLengthsAndScaling(context, childContext, parentBounds, out var cellLengths, out var cellScaling);
        UpdateCellLengthsWithPreferredLength(cellLengths, cellScaling);
        var spacing = Spacing ?? context.DefaultSpacing;
        for (int i = 0; i < Children.Length; i++)
        {
            var childSize = Children[i].GetPreferredSize(childContext, parentBounds);
            var childLayoutSize = GetSize(cellLengths[i], GetBreadth(childSize));
            size = UpdateTotalSize(size, childLayoutSize, spacing);
        }
        size = UpdateTotalSize(size, SizeF.Empty, -spacing);
        return size;
    }

    private LayoutContext GetChildContext(LayoutContext context, RectangleF bounds)
    {
        return context with
        {
            CellLengths = GetChildCellLengths(context, bounds),
            CellScaling = GetChildCellScaling(),
            Depth = context.Depth + 1
        };
    }

    private void GetInitialCellLengthsAndScaling(LayoutContext context, LayoutContext childContext, RectangleF bounds,
        out List<float> cellLengths, out List<bool> cellScaling)
    {
        // If this line is supposed to be aligned with adjacent lines (e.g. 2 rows in a parent column or vice versa),
        // then our parent will have pre-calculated our cell sizes and scaling for us.
        cellLengths = Aligned ? context.CellLengths : null;
        cellScaling = Aligned ? context.CellScaling : null;
        // If we aren't aligned or we don't have a parent to do that pre-calculation, then we just determine our cell
        // sizes and scaling directly without any special alignment constraints.
        if (cellLengths == null || cellScaling == null)
        {
            cellLengths = new List<float>();
            cellScaling = new List<bool>();
            foreach (var child in Children)
            {
                cellLengths.Add(GetLength(child.GetPreferredSize(childContext, bounds)));
                cellScaling.Add(DoesChildScale(child));
            }
        }
    }

    private void UpdateCellLengthsWithPreferredLength(List<float> cellLengths, List<bool> cellScaling)
    {
        if (!cellScaling.Any(scales => scales))
        {
            return;
        }
        // If multiple cells scale, then they will end up with the same length. Therefore, the biggest initial length
        // defines the preferred length for all scaled cells.
        float maxScaledLength = 0;
        for (int i = 0; i < cellLengths.Count; i++)
        {
            if (cellScaling[i])
            {
                maxScaledLength = Math.Max(maxScaledLength, cellLengths[i]);
            }
        }
        for (int i = 0; i < cellLengths.Count; i++)
        {
            if (cellScaling[i])
            {
                cellLengths[i] = maxScaledLength;
            }
        }
    }

    private void UpdateCellLengthsForAvailableSpace(List<float> cellLengths, List<bool> cellScaling, RectangleF bounds,
        int spacing)
    {
        var scaleCount = cellScaling.Count(scales => scales);
        if (scaleCount == 0)
        {
            return;
        }
        // If no controls scale, then they will all take up their preferred length.
        // If some controls scale, then we take [excess = remaining space + length of all scaling controls],
        // and divide that evenly among all scaling controls so they all have equal length.
        var excess = GetLength(bounds.Size) - spacing * (Children.Length - 1);
        for (int i = 0; i < Children.Length; i++)
        {
            if (!cellScaling[i])
            {
                excess -= cellLengths[i];
            }
        }
        // Update the lengths of scaling controls
        var scaleAmount = Math.DivRem((int) excess, scaleCount, out int scaleExtra);
        for (int i = 0; i < Children.Length; i++)
        {
            if (cellScaling[i])
            {
                cellLengths[i] = scaleAmount + (scaleExtra-- > 0 ? 1 : 0);
            }
        }
    }

    private List<float> GetChildCellLengths(LayoutContext context, RectangleF bounds)
    {
        var cellLengths = new List<float>();
        foreach (var child in Children)
        {
            if (child is TOrthogonal { Aligned: true } opposite)
            {
                for (int i = 0; i < opposite.Children.Length; i++)
                {
                    if (cellLengths.Count <= i) cellLengths.Add(0);
                    // TODO: We should probably shrink the bounds if needed
                    var preferredLength = GetBreadth(opposite.Children[i].GetPreferredSize(context, bounds));
                    cellLengths[i] = Math.Max(cellLengths[i], preferredLength);
                }
            }
        }
        return cellLengths;
    }

    private List<bool> GetChildCellScaling()
    {
        var cellScaling = new List<bool>();
        foreach (var child in Children)
        {
            if (child is TOrthogonal { Aligned: true } opposite)
            {
                for (int i = 0; i < opposite.Children.Length; i++)
                {
                    if (cellScaling.Count <= i) cellScaling.Add(false);
                    cellScaling[i] = cellScaling[i] || opposite.DoesChildScale(opposite.Children[i]);
                }
            }
        }
        return cellScaling;
    }
}