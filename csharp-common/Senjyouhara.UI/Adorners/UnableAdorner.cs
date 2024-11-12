using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace Senjyouhara.UI.Adorners;


//控件添加一个蒙层,用于禁用状态无法修改鼠标手势
public class UnableAdorner : Adorner
{
    private readonly Grid _grid;
    private Border _border;

    
    public VisualCollection VisualCollection { get; set; }

    protected override int VisualChildrenCount => VisualCollection.Count;

    protected override Visual GetVisualChild(int index) => VisualCollection[index];

    
    public UnableAdorner(UIElement uIElement) : base(uIElement) {
        VisualCollection = new VisualCollection(this);
        _grid = new Grid();
        _border = new Border()
        {
            Background = Brushes.Transparent,
            Cursor = (Cursor)TryFindResource("UnableCursor") ?? Cursors.No,
        };
        _grid.Children.Add(_border);
        VisualCollection.Add(_grid);
    }
    
    public void SetEnable(bool isEnable) {
        _border.IsHitTestVisible = !isEnable;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _grid.Arrange(new Rect(finalSize));
        return base.ArrangeOverride(finalSize);

    }
}