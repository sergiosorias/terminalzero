namespace ZeroBusiness
{
    public class Actions
    {
        public const string AppBack                         = "Application@Back";
        public const string AppHome                         = "Application@Home";
        public const string AppExit                         = "Application@Exit";
        
        public const string OpenBarcodeGeneratorView        = "Configuración@Generar Códigos de lote";
        
        public const string ExecSync                        = "Configuración@Sincronizar";
        public const string OpenPropertiesView              = "Configuración@Propiedades";
        public const string OpenUserListView                = "Configuración@Usuarios@Lista de Usuarios";
        public const string OpenUserPasswordChangeMessage   = "Configuración@Usuarios@Cambiar contraseña";
        public const string ExecUpgradeProcess              = "Configuración@Actualizar";

        public const string OpenProductsView                = "Tablas Maestras@Productos@Lista de Productos";
        public const string OpenProductMessage              = "Tablas Maestras@Productos@Consulta";
        public const string OpenSupplierView                = "Tablas Maestras@Proveedores";
        public const string OpenCustomersView               = "Tablas Maestras@Clientes";
        public const string ExecExportMasterData            = "Tablas Maestras@Exportar Datos";

        public const string OpenCurrentStockView            = "Operaciones@Stock@Actual";
        public const string OpenNewStockView                = "Operaciones@Stock@Alta";
        public const string OpenModifyStockView             = "Operaciones@Stock@Baja";
        public const string OpenDeliveryNoteView            = "Operaciones@Remitos de salida";
        public const string OpenNewSaleView                 = "Operaciones@Ventas@Nueva";
    }
}
