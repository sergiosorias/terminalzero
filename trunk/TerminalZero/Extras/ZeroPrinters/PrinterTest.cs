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
            LoadDummyData();
            LoadEvents();
            LoadActions();
            LoadPrinterParametersData();
        }

        private void LoadDummyData()
        {
            Log("Cargando data de prueba");
            customer = new PrinterCustomer
            {
                Name = "Cliente Dummy",
                DNI = "30710754507",
                DNIType = TiposDeDocumento.TIPO_CUIT,
                TaxPosition = TiposDeResponsabilidades.RESPONSABLE_INSCRIPTO,
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

        private void AddCommand(string name , Action command)
        {
            ActionCommands.Add(ActionCommands.Count, new KeyValuePair<string, Action>(name,command));
        }

        private void LoadActions()
        {
            Log("Cargando acciones");
            ActionCommands = new Dictionary<int, KeyValuePair<string, Action>>();
            
            //Información
            AddCommand("Ver Configuración", PrintPrinterParametersData);
            AddCommand("Autodetectar Controlador", () => printer.AutodetectarControlador(printer.Puerto));
            AddCommand("Autodetectar Modelo", () => printer.AutodetectarModelo());
            AddCommand("Verificar Modelo", () => printer.VerificarModelo());
            AddCommand("Obtener Datos De Inicializacion", LoadInitializationData);
            AddCommand("Obtener Configuracion", LoadConfiguration);
            AddCommand("Pedido De Status", () => printer.PedidoDeStatus());

            //Deprecated
            //AddCommand("Obtener Datos De Configuracion", () => printer.ObtenerDatosDeConfiguracion());
            //AddCommand("Obtener Configuracion Completa", () => printer.ObtenerConfiguracionCompleta());
            
            //Cancelación
            AddCommand("Abortar", () => printer.Abortar());
            AddCommand("Tratar De Cancelar Todo", () => printer.TratarDeCancelarTodo());
            AddCommand("Cancelar Comprobante Fiscal", () => printer.CancelarComprobanteFiscal());
            AddCommand("Cancelar Comprobante", () => printer.CancelarComprobante());
            AddCommand("Cerrar Comprobante Fiscal", () =>
            {
                object intout;
                printer.CerrarComprobanteFiscal(1, out intout);
                Log("-->" + intout);
            });
            AddCommand("Cerrar Comprobante No Fiscal", () => printer.CerrarComprobanteNoFiscal(1));

            //Nuevo / agregar
            AddCommand("Comenzar", () => printer.Comenzar());
            AddCommand("Datos Cliente", () => printer.DatosCliente(customer.Name, customer.DNI, customer.DNIType, customer.TaxPosition, customer.Address));
            AddCommand("Abrir documento fiscal (A)", () => printer.AbrirComprobanteFiscal(DocumentosFiscales.TICKET_FACTURA_A));
            AddCommand("Imprimir Texto Fiscal ", () => printer.ImprimirTextoFiscal("Mi Texto Fiscal"));
            AddCommand("Imprimir Item ", () => printer.ImprimirItem("Dummy item", 1, 1, 10.5, 0));
            AddCommand("Subtotal ", Subtotal);
            AddCommand("Cerrar Documento fiscal (A) ", () =>
                                            {
                                                object number;
                                                printer.CerrarComprobanteFiscal(1,out number);
                                                Log(string.Format("Factura A Numero{0}",number));
                                            });
            AddCommand("Avanzar Papel", () => printer.AvanzarPapel(TiposDePapel.PAPEL_TICKET, 2));

            AddCommand("Hacer una venta", PrintCompleteSale);
            AddCommand("Hacer una venta con descuento", PrintDiscountSale);

        }

        private void PrintDiscountSale()
        {
            PrintCompleteSale();
            printer.DescuentoGeneral("Bonificación por ser cliente", 15, true);
        }

        private void PrintCompleteSale()
        {
            printer.AbrirComprobanteFiscal(DocumentosFiscales.TICKET_C);
            printer.ImprimirItem("Dummy item", 1, 1, 10.5, 0);
            printer.ImprimirItem("Dummy item 1", 1, 1, 21.0, 0);
            printer.ImprimirItem("Dummy item 2", 10, 0.1, 21.0, 0);
        }

        private void Subtotal()
        {
            object o1, o2, o3, o4, o5, o6;
            printer.Subtotal(true, out o1, out o2, out o3, out o4, out o5, out o6);
            Log(string.Format("Cantidad de items: {0}{6} Monto Ventas: {1}{6} Monto IVA: {2}{6} Monto Pagado: {3}{6} Monto IVA No Inscripto: {4}{6} Monto Impuestos internos: {5}", o1, o2, o3, o4, o5, o6,Environment.NewLine));
        }

        private void LoadConfiguration()
        {
            object o1, o2, o3, o4, o5, o6,o7, o8,o9,o10,o11,o12,o13,o14,o15,o16, o17;
            printer.ObtenerConfiguracion(out o1,out o2,out o3,out o4,out o5,out o6,out o7,out o8,out o9,out o10,out o11,out o12,out o13, out o14,out o15, out o16,out o17);
            Log(string.Format("Limite consumidor final: {0}{18}Limite Factura {1}{18}ProcentajeIvaNoInscripto: {2}{18}Numero de copias Maximo: {3}{18}Imprimer Cambio: {4}{18}" +
            "Imprimer leyendas opcionales: {5}{18}Tipo de corte: {6}{18}Imprimer Marco: {7}{18}Re imprimer documentos: {8}{18}" +
            "Descripción del medio de pago: {9}{18}Sonido: {10}{18}Alto hoja: {11}{18}Ancho hoja: {12}{18}Estación impresion reportes XZ: {13}{18}Modo impresion: {14}{18}Chequeo desborde completo: {15}{18}Chequeo tapa abierta: {16}{18}Unknown: {17}", o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o12, o13, o14, o15, o16, o17, Environment.NewLine));
        }

        private void LoadInitializationData()
        {
            object o1, o2, o3, o4, o5, o6,o7, o8;
            printer.ObtenerDatosDeInicializacion(out o1, out o2, out o3, out o4, out o5, out o6, out o7, out o8);
            Log(string.Format("Nro CUIT: {0}{8}Razon Social: {1}{8}Numero de Serie: {2}{8}Fecha Init: {3}{8}Nro De Pos: {4}{8}Fecha Ini Act: {5}{8}Ing Brut: {6}{8}Resp IVA: {7} ", o1, o2, o3, o4, o5, o6, o7, o8, Environment.NewLine));
        }

        private void LoadPrinterParametersData()
        {
            Log("Cargando datos de inicialización");
            initializationList = new List<Action>
            {
                () => Log(string.Format("Baudios - {0}", printer.Baudios)),
                () => Log(string.Format("Cajon Abierto - {0}", printer.CajonAbierto)),
                () => Log(string.Format("Contador Impresora Ocupada {0}",printer.ContadorImpresoraOcupada)),
                () => Log(string.Format("Descripcion Documento En Curso {0}", printer.DescripcionDocumentoEnCurso)),
                () => Log(string.Format("Descripciones Largas {0}", printer.DescripcionesLargas)),
                () => Log(string.Format("Descripcion Estado Controlador {0}", printer.DescripcionEstadoControlador)),
                () => Log(string.Format("Direccion IP - {0}", printer.DireccionIP)),
                () => Log(string.Format("Doble Ancho - {0}", printer.DobleAncho)),
                () => Log(string.Format("Documento De Referencia - {0}", printer.DocumentoDeReferencia[0])),
                () => Log(string.Format("Documento En Curso - {0}", printer.DocumentoEnCurso)),
                () => Log(string.Format("Encabezado - {0}", printer.Encabezado[0])),
                () => Log(string.Format("Estado Controlador - {0}", printer.EstadoControlador)),
                () => Log(string.Format("Eventos Individuales - {0}", printer.EventosIndividuales)),
                () => Log(string.Format("Fecha Hora Fiscal - {0}", printer.FechaHoraFiscal)),
                () => Log(string.Format("Hubo Error Fiscal - {0}", printer.HuboErrorFiscal)),
                () => Log(string.Format("Hubo Error Mecanico - {0}", printer.HuboErrorMecanico)),
                () => Log(string.Format("Hubo Falta Papel - {0}", printer.HuboFaltaPapel)),
                () => Log(string.Format("Hubo Stat Prn - {0}", printer.HuboStatPrn)),
                () => Log(string.Format("Impuesto Interno Fijo - {0}", printer.ImpuestoInternoFijo)),
                () => Log(string.Format("ImpuestoInterno Por Monto - {0}",printer.ImpuestoInternoPorMonto)),
                () => Log(string.Format("Indicador Fiscal - {0}", printer.IndicadorFiscal[0])),
                () => Log(string.Format("Indicador Impresora - {0}", printer.IndicadorImpresora[0])),
                () => Log(string.Format("Interlineado - {0}", printer.Interlineado)),
                () => Log(string.Format("kIVA - {0}", printer.kIVA)),
                () => Log(string.Format("Modelo - {0}", printer.Modelo)),
                () => Log(string.Format("Modo Stat Prn - {0}", printer.ModoStatPrn)),
                () => Log(string.Format("Nombre De Fantasia - {0}", printer.NombreDeFantasia[0])),
                () => Log(string.Format("Paginas De Ultimo Documento - {0}",printer.PaginasDeUltimoDocumento)),
                () => Log(string.Format("Precio Base - {0}", printer.PrecioBase)),
                () => Log(string.Format("Puerto - {0}", printer.Puerto)),
                () => Log(string.Format("Reintento Constante - {0}", printer.ReintentoConstante)),
                () => Log(string.Format("Reintentos - {0}", printer.Reintentos)),
                () => Log(string.Format("Respuesta - {0}", printer.Respuesta[0])),
                () => Log(string.Format("Resumen IVA - {0}", printer.ResumenIVA)),
                () => Log(string.Format("Soporta Stat PRN - {0}", printer.SoportaStatPRN)),
                () => Log(string.Format("Tiempo De Espera - {0}", printer.TiempoDeEspera)),
                () => Log(string.Format("Transporte - {0}", printer.Transporte)),
                () => Log(string.Format("Ultima Nota CreditoA - {0}", printer.UltimaNotaCreditoA)),
                () => Log(string.Format("Ultima Nota CreditoBC - {0}", printer.UltimaNotaCreditoBC)),
                () => Log(string.Format("Ultimo Documento FiscalA - {0}", printer.UltimoDocumentoFiscalA)),
                () => Log(string.Format("Ultimo Documento Fiscal BC - {0}",printer.UltimoDocumentoFiscalBC)),
                () => Log(string.Format("Ultimo Documento Fue Cancelado - {0}",printer.UltimoDocumentoFueCancelado)),
                () => Log(string.Format("Ultimo Remito - {0}", printer.UltimoRemito)),
                () => Log(string.Format("Usar ASCII - {0}", printer.UsarASCII)),
                () => Log(string.Format("Usar Display - {0}", printer.UsarDisplay)),
                () => Log(string.Format("Verificacion Completa De Errores - {0}", printer.VerificacionCompletaDeErrores)),
                () => Log(string.Format("Version - {0}", printer.Version))
            };
        }

        private void PrintPrinterParametersData()
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
            Log(string.Format("{0} - Puerto: {1} - Velocidad: {2}", MethodBase.GetCurrentMethod().Name, Puerto, Velocidad));
        }

        void ImpresoraOK()
        {
            Log(string.Format("{0}", MethodBase.GetCurrentMethod().Name));
        }

        void ImpresoraOcupada()
        {
            Log(string.Format("{0}", MethodBase.GetCurrentMethod().Name));
        }

        void ImpresoraNoResponde(int CantidadReintentos)
        {
            Log(string.Format("{0} - CantidadReintentos: {1}", MethodBase.GetCurrentMethod().Name, CantidadReintentos));
        }

        void FaltaPapel()
        {
            Log(string.Format("{0}", MethodBase.GetCurrentMethod().Name));
        }

        void EventoImpresora(int Flags)
        {
            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", printer.DescripcionStatusImpresor(Flags)));
            Log("----------------------------");
        }

        void ErrorImpresora(int Flags)
        {
            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", printer.DescripcionStatusImpresor(Flags)));
            Log("----------------------------");
        }

        void EventoFiscal(int Flags)
        {
            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", printer.DescripcionStatusFiscal(Flags)));
            Log("----------------------------");
        }
        
        void ErrorFiscal(int Flags)
        {
            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", printer.DescripcionStatusFiscal(Flags)));
            Log("----------------------------");
        }

        #endregion
        
        public void TryExecute(int action)
        {
            if(ActionCommands.ContainsKey(action))
            {
                try
                {
                    Log("******************************");
                    Log(string.Format("Acción {0} ", ActionCommands[action].Key));
                    if (CanExecute(string.Format("Ejecutar {0}?", ActionCommands[action].Key)))
                    {
                        ActionCommands[action].Value();
                    }
                }
                catch (Exception ex)
                {
                    Log(string.Format("Error: {0}", ex.Message));
                }
                finally
                {
                    Log("******************************");
                }
            }
        }
    }
}
