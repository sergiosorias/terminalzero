﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZeroStock.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ZeroStock.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Tiene que seleccionar un documento para poder continuar!.
        /// </summary>
        internal static string DeliveryNoteMandatoryMsg {
            get {
                return ResourceManager.GetString("DeliveryNoteMandatoryMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Selección de remitos.
        /// </summary>
        internal static string DeliveryNoteSelection {
            get {
                return ResourceManager.GetString("DeliveryNoteSelection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string Fail {
            get {
                return ResourceManager.GetString("Fail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Importante.
        /// </summary>
        internal static string Important {
            get {
                return ResourceManager.GetString("Important", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ¿Desea guardar los datos ingresados?.
        /// </summary>
        internal static string QuestionSaveCurrentData {
            get {
                return ResourceManager.GetString("QuestionSaveCurrentData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Producto Inexistente.
        /// </summary>
        internal static string UnexistentProduct {
            get {
                return ResourceManager.GetString("UnexistentProduct", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Día incorrecto.
        /// </summary>
        internal static string WrongDay {
            get {
                return ResourceManager.GetString("WrongDay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mes incorrecto.
        /// </summary>
        internal static string WrongMonth {
            get {
                return ResourceManager.GetString("WrongMonth", resourceCulture);
            }
        }
    }
}
