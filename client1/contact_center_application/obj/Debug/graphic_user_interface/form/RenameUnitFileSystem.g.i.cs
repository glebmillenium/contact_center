﻿#pragma checksum "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AC0C0436DE8631F536140A1A487137C9"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using contact_center_application.graphic_user_interface.form;


namespace contact_center_application.graphic_user_interface.form {
    
    
    /// <summary>
    /// RenameUnitFileSystem
    /// </summary>
    public partial class RenameUnitFileSystem : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textView;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox newNameFile;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup warningSymbols;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button activeButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/АРМ оператора контакт-центра;component/graphic_user_interface/form/renameunitfil" +
                    "esystem.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.textView = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.newNameFile = ((System.Windows.Controls.TextBox)(target));
            
            #line 17 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
            this.newNameFile.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.newNameFile_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 3:
            this.warningSymbols = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 4:
            this.activeButton = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
            this.activeButton.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 24 "..\..\..\..\graphic_user_interface\form\RenameUnitFileSystem.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

