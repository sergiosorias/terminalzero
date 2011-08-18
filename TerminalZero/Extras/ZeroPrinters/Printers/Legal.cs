using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FiscalPrinterLib;
using ZeroPrinters.Extras;

namespace ZeroPrinters.Printers
{
    /// <summary>
    /// Wrapper for Legal printers
    /// </summary>
    public class Legal : SystemPrinter
    {
        private HASAR printer;
        public string LastDocumentNumber { get; private set; }

        public Legal(PrinterInfo info)
            : base(info)
        {
            printer = new HASAR();
            printer.Transporte = TiposDeTransporte.PUERTO_SERIE;
            if (info.Parameters.ContainsKey("Port"))
            {
                printer.Puerto = int.Parse(info.Parameters["Port"]);
                printer.AutodetectarControlador(printer.Puerto);
            }
            else
            {
#if RELEASE
                var names = System.IO.Ports.SerialPort.GetPortNames();
                foreach (string name in names)
                {
                    int port = int.Parse(name.Remove(0, 3));
                    try
                    {
                        printer.AutodetectarControlador(port);
                    }
                    catch (IOException ex)
                    {
                        LastError = "Error de acceso al puerto";
                        LogError(ex);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        LastError = "No hay permisos de acceso al puerto";
                        LogError(ex);
                    }
                    catch(Exception ex)
                    {
                        LastError = "Error no identificado";
                        LogError(ex);
                    }
                }
#endif
            }
            LoadEvents();

        }

        #region Overrides of SystemPrinter

        public override bool IsOnLine
        {
            get;
            protected set;
        }

        public override void Clear()
        {
            base.Clear();
            printer.TratarDeCancelarTodo();
        }

        public override void Print()
        {
            base.Print();
            if (!HasError)
            {
                var obj = new object();
                printer.CerrarComprobanteFiscal(1, out obj);
                LastDocumentNumber = obj.ToString();
            }
        }

        #endregion

        #region Private Methods

        private DocumentosFiscales GetFiscalDocument(CustomerInfo customer, DocumentosFiscales fiscalDocument, out TiposDeDocumento docType, out TiposDeResponsabilidades respType)
        {
            docType = TiposDeDocumento.TIPO_NINGUNO;
            respType = TiposDeResponsabilidades.NO_CATEGORIZADO;

            if (customer != null)
            {
                switch (customer.DNIType)
                {
                    case IdentificationType.DNI:
                        docType = TiposDeDocumento.TIPO_DNI;
                        break;
                    case IdentificationType.CUIL:
                        docType = TiposDeDocumento.TIPO_CUIL;
                        break;
                    case IdentificationType.CUIT:
                        docType = TiposDeDocumento.TIPO_CUIT;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                switch (customer.TaxPosition)
                {
                    case CustomerResponsibility.NoTaxPayer:
                        respType = TiposDeResponsabilidades.CONSUMIDOR_FINAL;
                        fiscalDocument = DocumentosFiscales.TICKET_FACTURA_B;
                        break;
                    case CustomerResponsibility.STS_TaxPayer:
                        respType = TiposDeResponsabilidades.MONOTRIBUTO;
                        fiscalDocument = DocumentosFiscales.TICKET_C;
                        break;
                    case CustomerResponsibility.TaxPayer:
                        respType = TiposDeResponsabilidades.RESPONSABLE_INSCRIPTO;
                        fiscalDocument = DocumentosFiscales.TICKET_FACTURA_A;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return fiscalDocument;
        }

        private void LoadConfiguration()
        {
            object o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16, o17;
            printer.ObtenerConfiguracion(out o1, out o2, out o3, out o4, out o5, out o6, out o7, out o8, out o9, out o10, out o11, out o12, out o13, out o14, out o15, out o16, out o17);
            Log(string.Format("Limite consumidor final: {0}{18}Limite Factura {1}{18}Procentaje Iva No Inscripto: {2}{18}Numero de copias Maximo: {3}{18}Imprimer Cambio: {4}{18}" +
            "Imprimer leyendas opcionales: {5}{18}Tipo de corte: {6}{18}Imprimer Marco: {7}{18}Re imprime documentos: {8}{18}" +
            "Descripción del medio de pago: {9}{18}Sonido: {10}{18}Alto hoja: {11}{18}Ancho hoja: {12}{18}Estación impresion reportes XZ: {13}{18}Modo impresion: {14}{18}Chequeo desborde completo: {15}{18}Chequeo tapa abierta: {16}{18}Unknown: {17}", o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o12, o13, o14, o15, o16, o17, Environment.NewLine));
        }

        private void LoadInitializationData()
        {
            object o1, o2, o3, o4, o5, o6, o7, o8;
            printer.ObtenerDatosDeInicializacion(out o1, out o2, out o3, out o4, out o5, out o6, out o7, out o8);
            Log(string.Format("Nro CUIT: {0}{8}Razón Social: {1}{8}Numero de Serie: {2}{8}Fecha Init: {3}{8}Nro De Pos: {4}{8}Fecha Inicio Actividades: {5}{8}Ingresos Brutos: {6}{8}Responsabilidad IVA: {7} ", o1, o2, o3, o4, o5, o6, o7, o8, Environment.NewLine));
        }

        private IEnumerable<Action> LoadPrinterParametersData()
        {
            Log("Cargando datos de inicialización");
            List<Action> initializationList = new List<Action>
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

            return initializationList;
        }

        private void PrintPrinterParametersData()
        {
            foreach (Action action in LoadPrinterParametersData())
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
            }

        }

        private void Log(string message)
        {

        }

        private void LogError(Exception exception)
        {
            Log(string.Format("{0} - Descripción: {1}", LastError, exception));
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

        private void ProgresoDeteccion(int puerto, int velocidad)
        {

        }

        #region Handlers

        private void ImpresoraOcupada()
        {
            Log(string.Format("{0}", MethodBase.GetCurrentMethod().Name));
        }

        private void ImpresoraNoResponde(int CantidadReintentos)
        {
            Log(string.Format("{0} - CantidadReintentos: {1}", MethodBase.GetCurrentMethod().Name, CantidadReintentos));
            LastError = "Impresora No Responde";
        }

        private void FaltaPapel()
        {
            Log(string.Format("{0}", MethodBase.GetCurrentMethod().Name));
            LastError = "Falta Papel";
        }

        private void EventoImpresora(int Flags)
        {
            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", printer.DescripcionStatusImpresor(Flags)));
            Log("----------------------------");
        }

        private void ErrorImpresora(int Flags)
        {
            LastError = printer.DescripcionStatusImpresor(Flags);

            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", LastError));
            Log("----------------------------");

        }

        private void EventoFiscal(int Flags)
        {
            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", printer.DescripcionStatusFiscal(Flags)));
            Log("----------------------------");
        }

        private void ErrorFiscal(int Flags)
        {
            LastError = printer.DescripcionStatusFiscal(Flags);

            Log("----------------------------");
            Log(string.Format("{0} - {1}", MethodBase.GetCurrentMethod().Name, Flags));
            Log(string.Format("Descripción {0}", LastError));
            Log("----------------------------");
        }

        private void ImpresoraOK()
        {
            IsOnLine = true;
        }

        #endregion

        #endregion

        #region Public Methods

        public void OpenInvoice(CustomerInfo customer)
        {
            DocumentosFiscales fiscalDocument = DocumentosFiscales.TICKET_FACTURA_B;
            TiposDeDocumento docType;
            TiposDeResponsabilidades respType;

            fiscalDocument = GetFiscalDocument(customer, fiscalDocument, out docType, out respType);

            if (fiscalDocument != DocumentosFiscales.TICKET_FACTURA_B)
            {
                if (!HasError)
                    printer.DatosCliente(customer.Name, customer.UniqueID, docType, respType, customer.Address);
            }
            if (!HasError)
                printer.AbrirComprobanteFiscal(fiscalDocument);
        }

        public void PrintItem(string name, int quantity, double price, double taxPrecentage, double internalTax)
        {
            if (!HasError)
                printer.ImprimirItem(name, quantity, price, taxPrecentage, internalTax);
        }

        public ZReport PrintZReport()
        {
            ZReport res = null;
            try
            {
                res = InternalPrintZReport();
            }
            catch (Exception ex)
            {
                LastError = "Error al imprimir el reporte";
                LogError(ex);
            }
            
            return res;
        }

        public ZReport PrintZReport(DateTime date)
        {
            ZReport res = null;
            try
            {
                res = InternalPrintZReport(date);
            }
            catch (Exception ex)
            {
                LastError = "Error al imprimir el reporte del dia " + date.ToShortDateString();
                LogError(ex);
            }
            
            return res;
        }

        private ZReport InternalPrintZReport(DateTime? date = null)
        {
            ZReport res = null;
            if (!HasError)
            {
                object fechaZeta = DateTime.Now.Date;
                object numeroZeta;
                object cantidadDfCancelados = -1;
                object cantidadDnfhEmitidos = -1;
                object cantidadDnfEmitidos = -1;
                object cantidadDfEmitidos = -1;
                object ultimoDocFiscalBc;
                object ultimoDocFiscalA;
                object montoVentasDocFiscal;
                object montoIvaDocFiscal;
                object montoImpInternosDocFiscal;
                object montoPercepcionesDocFiscal;
                object montoIvaNoInscriptoDocFiscal;
                object ultimaNotaCreditoBc;
                object ultimaNotaCreditoA;
                object montoVentasNotaCredito;
                object montoIvaNotaCredito;
                object montoImpInternosNotaCredito;
                object montoPercepcionesNotaCredito;
                object montoIvaNoInscriptoNotaCredito;
                object ultimoRemito;
                object cantidadNcCanceladas = -1;
                object cantidadDfbcEmitidos = -1;
                object cantidadDfaEmitidos = -1;
                object cantidadNcbcEmitidas = -1;
                object cantidadNcaEmitidas = -1;

                if (!date.HasValue)
                {
                    printer.ReporteZ(out numeroZeta, out cantidadDfCancelados, out cantidadDnfhEmitidos, out cantidadDnfEmitidos, out cantidadDfEmitidos, out ultimoDocFiscalBc, out ultimoDocFiscalA, out montoVentasDocFiscal, out montoIvaDocFiscal,
                                     out montoImpInternosDocFiscal, out montoPercepcionesDocFiscal, out montoIvaNoInscriptoDocFiscal, out ultimaNotaCreditoBc, out ultimaNotaCreditoA, out montoVentasNotaCredito,
                                     out montoIvaNotaCredito, out montoImpInternosNotaCredito, out montoPercepcionesNotaCredito, out montoIvaNoInscriptoNotaCredito, out ultimoRemito, out cantidadNcCanceladas,
                                     out cantidadDfbcEmitidos, out cantidadDfaEmitidos, out cantidadNcbcEmitidas, out cantidadNcaEmitidas);
                }
                else
                {
                    printer.ReporteZIndividualPorFecha(date.Value, out fechaZeta, out numeroZeta, out ultimoDocFiscalBc, out ultimoDocFiscalA,
                                                       out montoVentasDocFiscal, out montoIvaDocFiscal, out montoImpInternosDocFiscal,
                                                       out montoPercepcionesDocFiscal, out montoIvaNoInscriptoDocFiscal, out ultimaNotaCreditoBc,
                                                       out ultimaNotaCreditoA, out montoVentasNotaCredito,
                                                       out montoIvaNotaCredito, out montoImpInternosNotaCredito, out montoPercepcionesNotaCredito,
                                                       out montoIvaNoInscriptoNotaCredito, out ultimoRemito);
                }

                res = new ZReport(fechaZeta, numeroZeta,
                                  cantidadDfCancelados,cantidadDnfhEmitidos,
                                  cantidadDnfEmitidos,cantidadDfEmitidos,
                                  ultimoDocFiscalBc,ultimoDocFiscalA,
                                  montoVentasDocFiscal,montoIvaDocFiscal,
                                  montoImpInternosDocFiscal,montoPercepcionesDocFiscal,
                                  montoIvaNoInscriptoDocFiscal,ultimaNotaCreditoBc,
                                  ultimaNotaCreditoA,montoVentasNotaCredito,
                                  montoIvaNotaCredito,montoImpInternosNotaCredito,
                                  montoPercepcionesNotaCredito,montoIvaNoInscriptoNotaCredito,
                                  ultimoRemito,cantidadNcCanceladas,
                                  cantidadDfbcEmitidos,cantidadDfaEmitidos,
                                  cantidadNcbcEmitidas,cantidadNcaEmitidas);
            }
            return res;
        }

        #endregion

    }
}