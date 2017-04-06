﻿using Pharos.POS.Retailing.Extensions;
using Pharos.POS.Retailing.Models.PosModels;
using Pharos.POS.Retailing.Models.ViewModels;
using Pharos.Wpf.Controls;
using Pharos.Wpf.Extensions;
using System;
using System.Threading.Tasks;

namespace Pharos.POS.Retailing.ChildWin
{
    /// <summary>
    /// MenberEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MemberEditor : DialogWindow02, IBarcodeControl
    {
        public MemberEditor()
        {
            InitializeComponent();
            this.InitDefualtSettings();
            var model = new MembersViewModel();
            this.ApplyBindings(this, model);
            CurrentIInputElement = txtCard;
        }

        public System.Windows.IInputElement CurrentIInputElement { get; set; }


       


    }
}