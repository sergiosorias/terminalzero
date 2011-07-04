using System;
using System.Collections.Generic;
using System.Reflection;
using FiscalPrinterLib;

namespace ZeroPrinters
{
    public class PrinterCustomer
    {

        public TiposDeResponsabilidades TaxPosition { get; set; }

        public TiposDeDocumento DNIType { get; set; }

        public string DNI { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }

    public class PrinterTest
    {
        public Dictionary<int, KeyValuePair<string, Action>> ActionCommands { get; private set; }
        private HASAR printer;
        private List<Action> initializationList;
        private Action<string> Log;
        private Predicate<string> CanExecute;
        private PrinterCustomer customer;
        public PrinterTest(int port, Action<string> log, Predicate<string> canExecute)
        {
            printer = new HASAR();
            Log = log;
            CanExecute = canExecute;
            Log("Inicializando impresora");
            printer.Transporte = TiposDeTransporte.PUERTO_SERIE;
            printer.Puerto = port;
            Log(string.Format("Puerto {0}", printer.Puerto));
            printer.Modelo = ModelosDeImpresoras.MODELO_715;
            LoadDummyData();
            LoadEvents();
            LoadActions();
            LoadInitializationData();
        }

        private void LoadDummyData()
        {
            Log("Cargando data de prueba");
            customer = new PrinterCustomer
            {
                Name = "Cliente Dummy",
                DNI = "31032240",
                DNIType = TiposDeDocumento.TIPO_DNI,
                TaxPosition = TiposDeResponsabilidades.CONSUMIDOR_FINAL,
                Address = "Alguna calle 252"
            };

        }

        private void LoadEvents()
        {
            Log("Cargando eventos");
            printer.ErrorFiscal += ErrorFiscal;
            printer.ErrorImpresora += ErrorImpresora;
            printer.EventoFiscal += EventoFiscal;
            printer.EventoImpresora += EventoImpresora;
            printer.FaltaPapel += FaltaPapel;
            printer.ImpresoraNoResponde += ImpresoraNoResponde;
            printer.ImpresoraOcupada += ImpresoraOcupada;
            printer.ImpresoraOK += ImpresoraOK;
            printer.ProgresoDeteccion += ProgresoDeteccion;
        }

        private void LoadActions()
        {
            Log("Cargando acciones");
            ActionCommands = new Dictionary<int, KeyValuePair<string, Action>>();
            ActionCommands.Add(0, new KeyValuePair<string, Action>("Ver Configuración", LoadInfo));
            ActionCommands.Add(1, new KeyValuePair<string, Action>("Autodetectar Controlador", () => printer.AutodetectarControlador(printer.Puerto)));
            ActionCommands.Add(2, new KeyValuePair<string, Action>("Autodetectar Modelo", () => printer.AutodetectarModelo()));
            ActionCommands.Add(3, new KeyValuePair<string, Action>("Verificar Modelo", () => printer.VerificarModelo()));
            ActionCommands.Add(4, new KeyValuePair<string, Action>("Tratar De CancelarTodo", () => printer.TratarDeCancelarTodo()));
            ActionCommands.Add(5, new KeyValuePair<string, Action>("Comenzar", () => printer.Comenzar()));
            ActionCommands.Add(6, new KeyValuePair<string, Action>("Obtener Datos De Inicializacion", ObtenerDatosDeInicializacion));
            ActionCommands.Add(7, new KeyValuePair<string, Action>("Obtener Configuracion", ObtenerConfiguracion));
            ActionCommands.Add(8, new KeyValuePair<string, Action>("Obtener Datos De Configuracion", () => printer.ObtenerDatosDeConfiguracion()));
            ActionCommands.Add(9, new KeyValuePair<string, Action>("Verificar Modelo", () => printer.VerificarModelo()));
            ActionCommands.Add(10, new KeyValuePair<string, Action>("Obtener Configuracion Completa", () => printer.ObtenerConfiguracionCompleta()));
            ActionCommands.Add(11, new KeyValuePair<string, Action>("Tratar De Cancelar Todo", () => printer.TratarDeCancelarTodo()));
            ActionCommands.Add(12, new KeyValuePair<string, Action>("Datos Cliente", () => printer.DatosCliente(customer.Name, customer.DNI, customer.DNIType, customer.TaxPosition, customer.Address)));
            ActionCommands.Add(13, new KeyValuePair<string, Action>("Abrir documento fiscal (A)", () => printer.AbrirComprobanteFiscal(DocumentosFiscales.TICKET_FACTURA_A)));
            ActionCommands.Add(14, new KeyValuePair<string, Action>("Imprimir Texto Fiscal ", () => printer.ImprimirTextoFiscal("Mi Texto Fiscal")));
            ActionCommands.Add(15, new KeyValuePair<string, Action>("Imprimir Item ", () => printer.ImprimirItem("Dummy item",10,100,0.21,0)));
            ActionCommands.Add(16, new KeyValuePair<string, Action>("Subtotal ", Subtotal));
            ActionCommands.Add(17, new KeyValuePair<string, Action>("Codigo de barras ", ()=>printer.ImprimirCodigoDeBarras(TiposDeCodigoDeBarras.CODIGO_TIPO_EAN_13,"000201102002",false,true)));
            ActionCommands.Add(18, new KeyValuePair<string, Action>("Cerrar Documento fiscal (A) ", ()=>
                                                                                                        {
                                                                                                            object number;
                                                                                                            printer.CerrarComprobanteFiscal(1,out number);
                                                                                                            Log(string.Format("Factura A Numero{0}",number));
                                                                                                        }));
            //ActionCommands.Add(19, new KeyValuePair<string, Action>("", printer.ReporteZ()));
            

        }

        private void Subtotal()
        {
            object o1, o2, o3, o4, o5, o6;
            printer.Subtotal(true, out o1, out o2, out o3, out o4, out o5, out o6);
            Log(string.Format("Cantidad de items: {0} -Monto Ventas: {1} -Monto IVA: {2} -Monto Pagado: {3} -Monto IVA No Inscripto: {4} -Monto Impuestos internos: {5}", o1, o2, o3, o4, o5, o6));
        }

        private void ObtenerConfiguracion()
        {
            object o1, o2, o3, o4, o5, o6,o7, o8,o9,o10,o11,o12,o13,o14,o15,o16, o17;
            printer.ObtenerConfiguracion(out o1,out o2,out o3,out o4,out o5,out o6,out o7,out o8,out o9,out o10,out o11,out o12,out o13, out o14,out o15, out o16,out o17);
            Log(string.Format("Limite consumidor final: {0} - Limite Factura {1} -ProcentajeIvaNoInscripto: {2} -Numero de copias Maximo: {3} -Imprimer Cambio: {4} -"+
            "Imprimer leyendas opcionales: {5} -Tipo de corte: {6} -Imprimer Marco: {7} -Re imprimer documentos: {8} -"+
            "Descripción del medio de pago: {9} -Sonido: {10} -Alto hoja: {11} -Ancho hoja: {12} - Estación impresion reportes XZ: {13} -Modo impresion: {14} - Chequeo desborde cmpleto: {15} - Chequeo tapa abierta: {16} - {17}", o1, o2, o3, o4, o5, o6, o7, o8,o9,o10,o11,o12,o12,o13,o14,o15,o16,o17));
        }

        private void ObtenerDatosDeInicializacion()
        {
            object o1, o2, o3, o4, o5, o6,o7, o8;
            printer.ObtenerDatosDeInicializacion(out o1, out o2, out o3, out o4, out o5, out o6, out o7, out o8);
            Log(string.Format("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} ", o1, o2, o3, o4, o5, o6, o7, o8));
        }

        private void LoadInitializationData()
        {
            Log("Cargando datos de inicialización");
            initializationList = new List<Action>
            {
                () => Log(string.Format("Baudios {0}", printer.Baudios)),
                () => Log(string.Format("Cajon Abierto {0}", printer.CajonAbierto)),
                () => Log(string.Format("Contador Impresora Ocupada {0}",printer.ContadorImpresoraOcupada)),
                () => Log(string.Format("Descripcion Documento En Curso {0}", printer.DescripcionDocumentoEnCurso)),
                () => Log(string.Format("Descripciones Largas {0}", printer.DescripcionesLargas)),
                () => Log(string.Format("Direccion IP {0}", printer.DireccionIP)),
                () => Log(string.Format("Doble Ancho {0}", printer.DobleAncho)),
                () => Log(string.Format("Documento De Referencia {0}", printer.DocumentoDeReferencia[0])),
                () => Log(string.Format("Documento En Curso {0}", printer.DocumentoEnCurso)),
                () => Log(string.Format("Encabezado {0}", printer.Encabezado[0])),
                () => Log(string.Format("Estado Controlador {0}", printer.EstadoControlador)),
                () => Log(string.Format("Eventos Individuales {0}", printer.EventosIndividuales)),
                () => Log(string.Format("Fecha Hora Fiscal {0}", printer.FechaHoraFiscal)),
                () => Log(string.Format("Hubo Error Fiscal {0}", printer.HuboErrorFiscal)),
                () => Log(string.Format("Hubo Error Mecanico {0}", printer.HuboErrorMecanico)),
                () => Log(string.Format("Hubo Falta Papel {0}", printer.HuboFaltaPapel)),
                () => Log(string.Format("Hubo Stat Prn {0}", printer.HuboStatPrn)),
                () => Log(string.Format("Impuesto Interno Fijo {0}", printer.ImpuestoInternoFijo)),
                () => Log(string.Format("ImpuestoInterno Por Monto {0}",printer.ImpuestoInternoPorMonto)),
                () => Log(string.Format("Indicador Fiscal {0}", printer.IndicadorFiscal[0])),
                () => Log(string.Format("Indicador Impresora {0}", printer.IndicadorImpresora[0])),
                () => Log(string.Format("Interlineado {0}", printer.Interlineado)),
                () => Log(string.Format("kIVA {0}", printer.kIVA)),
                () => Log(string.Format("Modelo {0}", printer.Modelo)),
                () => Log(string.Format("Modo Stat Prn {0}", printer.ModoStatPrn)),
                () => Log(string.Format("Nombre De Fantasia {0}", printer.NombreDeFantasia[0])),
                () => Log(string.Format("Paginas De Ultimo Documento {0}",printer.PaginasDeUltimoDocumento)),
                () => Log(string.Format("Precio Base {0}", printer.PrecioBase)),
                () => Log(string.Format("Puerto {0}", printer.Puerto)),
                () => Log(string.Format("Reintento Constante {0}", printer.ReintentoConstante)),
                () => Log(string.Format("Reintentos {0}", printer.Reintentos)),
                () => Log(string.Format("Respuesta {0}", printer.Respuesta[0])),
                () => Log(string.Format("Resumen IVA {0}", printer.ResumenIVA)),
                () => Log(string.Format("Soporta Stat PRN {0}", printer.SoportaStatPRN)),
                () => Log(string.Format("Tiempo De Espera {0}", printer.TiempoDeEspera)),
                () => Log(string.Format("Transporte {0}", printer.Transporte)),
                () => Log(string.Format("Ultima Nota CreditoA {0}", printer.UltimaNotaCreditoA)),
                () => Log(string.Format("Ultima Nota CreditoBC {0}", printer.UltimaNotaCreditoBC)),
                () => Log(string.Format("Ultimo Documento FiscalA {0}", printer.UltimoDocumentoFiscalA)),
                () => Log(string.Format("Ultimo Documento Fiscal BC {0}",printer.UltimoDocumentoFiscalBC)),
                () => Log(string.Format("Ultimo Documento Fue Cancelado {0}",printer.UltimoDocumentoFueCancelado)),
                () => Log(string.Format("Ultimo Remito {0}", printer.UltimoRemito)),
                () => Log(string.Format("Usar ASCII {0}", printer.UsarASCII)),
                () => Log(string.Format("Usar Display {0}", printer.UsarDisplay)),
                () => Log(string.Format("Verificacion Completa De Errores {0}", printer.VerificacionCompletaDeErrores)),
                () => Log(string.Format("Version {0}", printer.Version))
            };
        }

        private void LoadInfo()
        {
            foreach (Action action in initializationList)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
            }

        }

        #region Handlers
        void ProgresoDeteccion(int Puerto, int Velocidad)
        {
            Log(string.Format("Evento {0} - Puerto {1} - Velocidad {2}", MethodBase.GetCurrentMethod().Name, Puerto, Velocidad));
        }

        void ImpresoraOK()
        {
            Log(string.Format("Evento {0}", MethodBase.GetCurrentMethod().Name));
        }

        void ImpresoraOcupada()
        {
            Log(string.Format("Evento {0}", MethodBase.GetCurrentMethod().Name));
        }

        void ImpresoraNoResponde(int CantidadReintentos)
        {
            Log(string.Format("Evento {0} - CantidadReintentos {1}", MethodBase.GetCurrentMethod().Name, CantidadReintentos));
        }

        void FaltaPapel()
        {
            Log(string.Format("Evento {0}", MethodBase.GetCurrentMethod().Name));
        }

        void EventoImpresora(int Flags)
        {
            Log(string.Format("Evento {0} - Flags {1}", MethodBase.GetCurrentMethod().Name, Flags));
        }

        void EventoFiscal(int Flags)
        {
            Log(string.Format("Evento {0} - Flags {1}", MethodBase.GetCurrentMethod().Name, Flags));
        }

        void ErrorImpresora(int Flags)
        {
            Log(string.Format("Evento {0} - Flags {1}", MethodBase.GetCurrentMethod().Name, Flags));
        }

        void ErrorFiscal(int Flags)
        {
            Log(string.Format("Evento {0} - Flags {1} - {2}", MethodBase.GetCurrentMethod().Name, Flags));
        }

        #endregion
        
        public void TryExecute(int action)
        {
            if(ActionCommands.ContainsKey(action))
            {
                try
                {
                    Log(string.Format("Acción {0} ", ActionCommands[action].Key));
                    if (CanExecute(string.Format("Ejecutar {0}?", ActionCommands[action].Key)))
                        ActionCommands[action].Value();
                }
                catch (Exception ex)
                {
                    Log(string.Format("Error: {0}", ex.Message));
                }
                
            }
        }
    }
}
