﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TerminalZeroClient.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TerminalZeroClient.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aplicación minizada.
        /// </summary>
        internal static string AppMinimized {
            get {
                return ResourceManager.GetString("AppMinimized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OwnerTerminal Zero.
        /// </summary>
        internal static string AppName {
            get {
                return ResourceManager.GetString("AppName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Click para abrir.
        /// </summary>
        internal static string ClickToOpen {
            get {
                return ResourceManager.GetString("ClickToOpen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cerrando.
        /// </summary>
        internal static string Closing {
            get {
                return ResourceManager.GetString("Closing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inicio.
        /// </summary>
        internal static string Home {
            get {
                return ResourceManager.GetString("Home", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ¿Esta seguro que desea salir de la aplicacion?.
        /// </summary>
        internal static string QuestionAreYouSureAppClosing {
            get {
                return ResourceManager.GetString("QuestionAreYouSureAppClosing", resourceCulture);
            }
        }
        
        internal static System.Drawing.Icon ZeroAppIcon {
            get {
                object obj = ResourceManager.GetObject("ZeroAppIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
