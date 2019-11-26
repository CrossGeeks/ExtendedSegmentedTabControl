using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using System.Collections;
using System.Reflection;
using System.Windows.Input;

namespace ExtendedSegmentedTabControl.Controls
{
    public class SegmentedControl : ContentView
    {
        public static readonly BindableProperty ItemSelectedProperty = BindableProperty.Create(nameof(ItemSelected), typeof(object), typeof(SegmentedControl), null, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) => OnItemSelectedChanged(bindable, oldValue, newValue));
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SegmentedControl), new List<object>(), propertyChanged: (bindable, oldValue, newValue) => OnItemsSourceChanged(bindable, oldValue, newValue));
        public static readonly BindableProperty SelectionIndicatorProperty = BindableProperty.Create(nameof(SelectionIndicator), typeof(string), typeof(SegmentedControl), string.Empty);
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(SegmentedControl), default(DataTemplate));
        public static readonly BindableProperty SelectedItemChangedCommandProperty = BindableProperty.Create(nameof(SelectedItemChangedCommand), typeof(Command<object>), typeof(SegmentedControl), default(Command<object>), BindingMode.TwoWay, null, SelectedItemChangedCommandPropertyChanged);

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public object ItemSelected
        {
            get
            {
                return (object)GetValue(ItemSelectedProperty);
            }
            set
            {
                SetValue(ItemSelectedProperty, value);

            }
        }


        public string SelectionIndicator
        {
            get
            {
                return (string)GetValue(SelectionIndicatorProperty);
            }
            set
            {
                SetValue(SelectionIndicatorProperty, value);

            }
        }

        static void SelectedItemChangedCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var source = bindable as SegmentedControl;
            if (source == null)
            {
                return;
            }
            source.SelectedItemChangedCommandChanged();
        }

        private void SelectedItemChangedCommandChanged()
        {
            OnPropertyChanged("SelectedItemChangedCommand");
        }

        public Command<string> SelectedItemChangedCommand
        {
            get
            {
                return (Command<string>)GetValue(SelectedItemChangedCommandProperty);
            }
            set
            {
                SetValue(SelectedItemChangedCommandProperty, value);
            }
        }
        public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs e);
        public event SelectedItemChangedEventHandler SelectedItemChanged;

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        static void OnItemSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabbedView = bindable as SegmentedControl;

            tabbedView.SelectedItemChanged?.Invoke(tabbedView, new SelectedItemChangedEventArgs(newValue));
            tabbedView.SelectedItemChangedCommand?.Execute(newValue);
            if (!string.IsNullOrEmpty(tabbedView.SelectionIndicator) && newValue != null)
            {
                foreach (object obj in tabbedView.ItemsSource)
                {
                    var prop = obj.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, tabbedView.SelectionIndicator, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        prop.SetValue(obj, obj.Equals(newValue));
                    }
                }
            }

        }
        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabbedView = bindable as SegmentedControl;
            var scrollView = (tabbedView.Content as ScrollView);
            var innerStackLayout = scrollView.Content as StackLayout;

            void newValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    innerStackLayout.Children.Clear();
                    scrollView.ScrollToAsync(0, 0, false);
                    return;
                }


                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                        innerStackLayout.Children.Remove(item as View);


                if (e.NewItems != null)
                    foreach (var item in e.NewItems)
                    {
                        if (!innerStackLayout.Children.Any(prop => prop.BindingContext == item))
                        {
                            var element = tabbedView.CreateNewItem(item);
                            innerStackLayout.Children.Add(element);
                        }

                    }

            }

            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= newValueINotifyCollectionChanged_CollectionChanged;
            }

            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;

            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged -= newValueINotifyCollectionChanged_CollectionChanged;
                newValueINotifyCollectionChanged.CollectionChanged += newValueINotifyCollectionChanged_CollectionChanged;
            }
            else
            {
                innerStackLayout.Children.Clear();
                IEnumerable items = (IEnumerable)newValue;

                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        innerStackLayout.Children.Add(tabbedView.CreateNewItem(item));
                    }

                    if (tabbedView.ItemSelected == null)
                        tabbedView.ItemSelected = items.FirstOrNull();
                }
            }
        }

        public ICommand TapCommand => new Command(OnTap);
        void OnTap(object val)
        {
            ItemSelected = val;
        }

        protected virtual View CreateNewItem(object item)
        {
            View view = null;
            if (ItemTemplate != null)
            {
                var content = ItemTemplate.CreateContent();
                view = (content is View) ? content as View : ((ViewCell)content).View;

                view.BindingContext = item;

                if (TapCommand != null)
                {
                    view.GestureRecognizers.Add(new TapGestureRecognizer { Command = TapCommand, CommandParameter = item });
                }
            }
            return view;
        }

        StackLayout _mainContentLayout = new StackLayout() { Spacing = 10, Orientation = StackOrientation.Horizontal };
        ScrollView _mainLayout = new ScrollView() { HorizontalScrollBarVisibility = ScrollBarVisibility.Never, VerticalOptions = LayoutOptions.Start, Orientation = ScrollOrientation.Horizontal };

        public SegmentedControl()
        {
            _mainLayout.Content = _mainContentLayout;
            Content = _mainLayout;
            Padding = new Thickness(0, 10, 0, 0);
        }
    }
   
	/// <summary>
	/// Extension methods for <see cref="IEnumerable"/>.
	/// </summary>
	public static class NonGenericEnumerableExtensions
    {
        public static bool Any(this IEnumerable source)
        {
            foreach (var item in source)
            {
                return true;
            }

            return false;
        }

        public static object FirstOrNull(this IEnumerable source)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    return item;
                }
            }

            return null;
        }
    }

}