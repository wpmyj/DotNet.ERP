﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Pharos.Wpf.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Pharos.Wpf.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Pharos.Wpf.Controls;assembly=Pharos.Wpf"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class LoginWindow : Window
    {
        static LoginWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoginWindow), new FrameworkPropertyMetadata(typeof(LoginWindow)));
        }

        #region Click events
        protected void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected void RestoreClick(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        protected void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #endregion

        public static readonly DependencyProperty SettingsCommandProperty = DependencyProperty.Register("SettingsCommand", typeof(ICommand), typeof(LoginWindow));

        public ICommand SettingsCommand
        {
            get { return (ICommand)GetValue(SettingsCommandProperty); }

            set { SetValue(SettingsCommandProperty, value); }
        }

        public override void OnApplyTemplate()
        {

            Rectangle moveRectangle = GetTemplateChild("moveRectangle") as Rectangle;
            if (moveRectangle != null)
                moveRectangle.PreviewMouseDown += moveRectangle_PreviewMouseDown;
            Border moveRectangleBorder = GetTemplateChild("moveRectangleBorder") as Border;
            if (moveRectangleBorder != null)
                moveRectangleBorder.PreviewMouseDown += moveRectangle_PreviewMouseDown;

            Button minimizeButton = GetTemplateChild("minimizeButton") as Button;
            if (minimizeButton != null)
                minimizeButton.Click += MinimizeClick;


            Button closeButton = GetTemplateChild("closeButton") as Button;
            if (closeButton != null)
                closeButton.Click += CloseClick;

            base.OnApplyTemplate();
        }




    }
}