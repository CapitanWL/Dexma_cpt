﻿using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace Dexma_cpt_ClientSide.Behaviors
{
    public class ScrollToEndBehavior : Behavior<ListBox>
    {
        
        private bool _shouldScrollToEnd = true;
        private bool _scrolling;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is { })
            {
                AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
                AssociatedObject.TemplateApplied += AssociatedObjectOnTemplateApplied;
            }
        }

        private void AssociatedObjectOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
        {
            var sw = e.NameScope.Get<ScrollViewer>("PART_ScrollViewer");
            sw.ScrollChanged += SwOnScrollChanged;

            
            AssociatedObject.ScrollIntoView(AssociatedObject.Items.Count - 1);
        }

        private void SwOnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer sw && !_scrolling)
            {
                _shouldScrollToEnd = Math.Abs(sw.Offset.Y - sw.Extent.Height + sw.Viewport.Height) < 5;
            }
        }

        private void AssociatedObjectOnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (_shouldScrollToEnd && AssociatedObject?.Items.Count > 0)
            {
                _scrolling = true;
                AssociatedObject.ScrollIntoView(AssociatedObject.Items.Count - 1);
                _scrolling = false;
            }
        }
        
    }
}
