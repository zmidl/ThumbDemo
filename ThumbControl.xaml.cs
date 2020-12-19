using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
   /// <summary>
   /// ThumbControl.xaml 的交互逻辑
   /// </summary>
   public partial class ThumbControl : ContentControl
    {
        public ThumbControl()
        {
            InitializeComponent();
            this.Template = this.FindResource(nameof(ControlTemplate)) as ControlTemplate;
        }
    }

    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ContentControl designerItem = DataContext as ContentControl;

            if (designerItem != null)
            {
                var dragDelta = new System.Windows.Point(e.HorizontalChange, e.VerticalChange);

                RotateTransform rotateTransform = designerItem.RenderTransform as RotateTransform;
                if (rotateTransform != null)
                {
                    dragDelta = rotateTransform.Transform(dragDelta);
                }

                Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + dragDelta.X);
                Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + dragDelta.Y);
            }
        }
    }

    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control item = this.DataContext as Control;

            if (item != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange,
                            item.ActualHeight - item.MinHeight);
                        item.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange,
                            item.ActualHeight - item.MinHeight);
                        Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                        item.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange,
                            item.ActualWidth - item.MinWidth);
                        Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                        item.Width -= deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange,
                            item.ActualWidth - item.MinWidth);
                        item.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }

    public class RotateThumb : Thumb
    {
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        private System.Windows.Point centerPoint;
        private ContentControl designerItem;
        private Canvas canvas;

        public RotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.RotateThumb_DragStarted);
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.canvas = VisualTreeHelper.GetParent(this.designerItem) as Canvas;

                if (this.canvas != null)
                {
                    this.centerPoint = this.designerItem.TranslatePoint(new System.Windows.Point(this.designerItem.Width * this.designerItem.RenderTransformOrigin.X, this.designerItem.Height * this.designerItem.RenderTransformOrigin.Y), this.canvas);

                    System.Windows.Point startPoint = Mouse.GetPosition(this.canvas);
                    this.startVector = System.Windows.Point.Subtract(startPoint, this.centerPoint);

                    this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                    if (this.rotateTransform == null)
                    {
                        this.designerItem.RenderTransform = new RotateTransform(0);
                        this.initialAngle = 0;
                    }
                    else
                    {
                        this.initialAngle = this.rotateTransform.Angle;
                    }
                }
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null && this.canvas != null)
            {
                var currentPoint = Mouse.GetPosition(this.canvas);
                Vector deltaVector = System.Windows.Point.Subtract(currentPoint, this.centerPoint);

                double angle = Vector.AngleBetween(this.startVector, deltaVector);

                RotateTransform rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                rotateTransform.Angle = this.initialAngle + Math.Round(angle, 0);
                this.designerItem.InvalidateMeasure();
            }
        }

    }
}
