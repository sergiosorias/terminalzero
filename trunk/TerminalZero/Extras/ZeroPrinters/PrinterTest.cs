using System;
using System.Collections.Generic;
using System.Reflection;
using FiscalPrinterLib;
using System.Linq;

namespace ZeroPrinters
{
    public class PrinterTest
    {
        private HASAR hassar1;

        private Dictionary<int, KeyValuePair<string,Action>> actionCommands;
        private List<Action> initializationList;

        public PrinterTest()
        {
            hassar1 = new HASAR();
            Console.WriteLine("Inicializando impresora");
            hassar1.Transporte = TiposDeTransporte.PUERTO_SERIE;
            hassar1.Puerto = 1;
            LoadEvents();
            LoadActions();
            LoadInitializationData();
        }

        private void LoadEvents()
        {
            Console.WriteLine("Cargando eventos");
            hassar1.ErrorFiscal += hassar1_ErrorFiscal;
            hassar1.ErrorImpresora += hassar1_ErrorImpresora;
            hassar1.EventoFiscal += hassar1_EventoFiscal;
            hassar1.EventoImpresora += hassar1_EventoImpresora;
            hassar1.FaltaPapel += hassar1_FaltaPapel;
            hassar1.ImpresoraNoResponde += hassar1_ImpresoraNoResponde;
            hassar1.ImpresoraOcupada += hassar1_ImpresoraOcupada;
            hassar1.ImpresoraOK += hassar1_ImpresoraOK;
            hassar1.ProgresoDeteccion += hassar1_ProgresoDeteccion;
        }

        private void LoadActions()
        {
            Console.WriteLine("Cargando acciones");
            actionCommands = new Dictionary<int, KeyValuePair<string, Action>>();
            actionCommands.Add(0, new KeyValuePair<string, Action>("Ver Configuración", () => { LoadInfo(); }));
            actionCommands.Add(1, new KeyValuePair<string, Action>("AutodetectarControlador", () => { hassar1.AutodetectarControlador(1); }));
            actionCommands.Add(2, new KeyValuePair<string, Action>("AutodetectarModelo", () => { hassar1.AutodetectarModelo(); }));
            actionCommands.Add(3, new KeyValuePair<string, Action>("VerificarModelo", () =>  { hassar1.VerificarModelo(); }));
            actionCommands.Add(4, new KeyValuePair<string, Action>("TratarDeCancelarTodo", () =>  { hassar1.TratarDeCancelarTodo(); }));
            actionCommands.Add(5, new KeyValuePair<string, Action>("Comenzar", () =>  { hassar1.Comenzar(); } ));
            actionCommands.Add(6, new KeyValuePair<string, Action>("ObtenerDatosDeInicializacion", () =>  { ObtenerDatosDeInicializacion(); }));
            actionCommands.Add(7, new KeyValuePair<string, Action>("ObtenerConfiguracion", () =>  { ObtenerConfiguracion(); }));
            actionCommands.Add(8, new KeyValuePair<string, Action>("ObtenerDatosDeConfiguracion", () =>  { hassar1.ObtenerDatosDeConfiguracion(); }));
            actionCommands.Add(9, new KeyValuePair<string, Action>("VerificarModelo", () => { hassar1.VerificarModelo(); }));
            actionCommands.Add(10, new KeyValuePair<string, Action>("ObtenerConfiguracionCompleta", () => { hassar1.ObtenerConfiguracionCompleta(); }));
            actionCommands.Add(11, new KeyValuePair<string, Action>("TratarDeCancelarTodo", () => { hassar1.TratarDeCancelarTodo(); }));

        }

        private void ObtenerConfiguracion()
        {
            object o1, o2, o3, o4, o5, o6,o7, o8,o9,o10,o11,o12,o13,o14,o15,o16, o17;
            hassar1.ObtenerConfiguracion(out o1,out o2,out o3,out o4,out o5,out o6,out o7,out o8,out o9,out o10,out o11,out o12,out o13, out o14,out o15, out o16,out o17);
            Console.WriteLine("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12} - {13} - {14} - {15} - {16} - {17}", o1, o2, o3, o4, o5, o6, o7, o8,o9,o10,o11,o12,o12,o13,o14,o15,o16,o17);
        }

        private void ObtenerDatosDeInicializacion()
        {
            object o1, o2, o3, o4, o5, o6,o7, o8;
            hassar1.ObtenerDatosDeInicializacion(out o1, out o2, out o3, out o4, out o5, out o6, out o7, out o8);
            Console.WriteLine("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} ", o1, o2, o3, o4, o5, o6, o7, o8);
        }

        private void LoadInitializationData()
        {
            Console.WriteLine("Cargando datos de inicialización");
            initializationList = new List<Action>
            {
                () => Console.WriteLine("Baudios {0}", hassar1.Baudios),
                () => Console.WriteLine("Cajon Abierto {0}", hassar1.CajonAbierto),
                () => Console.WriteLine("ContadorImpresoraOcupada {0}",hassar1.ContadorImpresoraOcupada),
                () => Console.WriteLine("DescripcionDocumentoEnCurso {0}", hassar1.DescripcionDocumentoEnCurso),
                () => Console.WriteLine("DescripcionesLargas {0}", hassar1.DescripcionesLargas),
                () => Console.WriteLine("DireccionIP {0}", hassar1.DireccionIP),
                () => Console.WriteLine("DobleAncho {0}", hassar1.DobleAncho),
                () => Console.WriteLine("DocumentoDeReferencia {0}", hassar1.DocumentoDeReferencia[0]),
                () => Console.WriteLine("DocumentoEnCurso {0}", hassar1.DocumentoEnCurso),
                () => Console.WriteLine("Encabezado {0}", hassar1.Encabezado[0]),
                () => Console.WriteLine("EstadoControlador {0}", hassar1.EstadoControlador),
                () => Console.WriteLine("EventosIndividuales {0}", hassar1.EventosIndividuales),
                () => Console.WriteLine("FechaHoraFiscal {0}", hassar1.FechaHoraFiscal),
                () => Console.WriteLine("HuboErrorFiscal {0}", hassar1.HuboErrorFiscal),
                () => Console.WriteLine("HuboErrorMecanico {0}", hassar1.HuboErrorMecanico),
                () => Console.WriteLine("HuboFaltaPapel {0}", hassar1.HuboFaltaPapel),
                () => Console.WriteLine("HuboStatPrn {0}", hassar1.HuboStatPrn),
                () => Console.WriteLine("ImpuestoInternoFijo {0}", hassar1.ImpuestoInternoFijo),
                () => Console.WriteLine("ImpuestoInternoPorMonto {0}",hassar1.ImpuestoInternoPorMonto),
                () => Console.WriteLine("IndicadorFiscal {0}", hassar1.IndicadorFiscal[0]),
                () => Console.WriteLine("IndicadorImpresora {0}", hassar1.IndicadorImpresora[0]),
                () => Console.WriteLine("Interlineado {0}", hassar1.Interlineado),
                () => Console.WriteLine("kIVA {0}", hassar1.kIVA),
                () => Console.WriteLine("Modelo {0}", hassar1.Modelo),
                () => Console.WriteLine("ModoStatPrn {0}", hassar1.ModoStatPrn),
                () => Console.WriteLine("NombreDeFantasia {0}", hassar1.NombreDeFantasia[0]),
                () => Console.WriteLine("PaginasDeUltimoDocumento {0}",hassar1.PaginasDeUltimoDocumento),
                () => Console.WriteLine("PrecioBase {0}", hassar1.PrecioBase),
                () => Console.WriteLine("Puerto {0}", hassar1.Puerto),
                () => Console.WriteLine("ReintentoConstante {0}", hassar1.ReintentoConstante),
                () => Console.WriteLine("Reintentos {0}", hassar1.Reintentos),
                () => Console.WriteLine("Respuesta {0}", hassar1.Respuesta[0]),
                () => Console.WriteLine("ResumenIVA {0}", hassar1.ResumenIVA),
                () => Console.WriteLine("SoportaStatPRN {0}", hassar1.SoportaStatPRN),
                () => Console.WriteLine("TiempoDeEspera {0}", hassar1.TiempoDeEspera),
                () => Console.WriteLine("Transporte {0}", hassar1.Transporte),
                () => Console.WriteLine("UltimaNotaCreditoA {0}", hassar1.UltimaNotaCreditoA),
                () => Console.WriteLine("UltimaNotaCreditoBC {0}", hassar1.UltimaNotaCreditoBC),
                () => Console.WriteLine("UltimoDocumentoFiscalA {0}", hassar1.UltimoDocumentoFiscalA),
                () => Console.WriteLine("UltimoDocumentoFiscalBC {0}",hassar1.UltimoDocumentoFiscalBC),
                () => Console.WriteLine("UltimoDocumentoFueCancelado {0}",hassar1.UltimoDocumentoFueCancelado),
                () => Console.WriteLine("UltimoRemito {0}", hassar1.UltimoRemito),
                () => Console.WriteLine("UsarASCII {0}", hassar1.UsarASCII),
                () => Console.WriteLine("UsarDisplay {0}", hassar1.UsarDisplay),
                () => Console.WriteLine("VerificacionCompletaDeErrores {0}", hassar1.VerificacionCompletaDeErrores),
                () => Console.WriteLine("Version {0}", hassar1.Version)
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
                    Console.WriteLine(ex.Message);
                }
            }

        }

        #region Handlers
        void hassar1_ProgresoDeteccion(int Puerto, int Velocidad)
        {
            Console.WriteLine(string.Format("Evento {0} - Puerto {1} - Velocidad {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, Puerto, Velocidad));
        }

        void hassar1_ImpresoraOK()
        {
            Console.WriteLine(string.Format("Evento {0} - {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        void hassar1_ImpresoraOcupada()
        {
            Console.WriteLine(string.Format("Evento {0} - {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        void hassar1_ImpresoraNoResponde(int CantidadReintentos)
        {
            Console.WriteLine(string.Format("Evento {0} - CantidadReintentos {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, CantidadReintentos));
        }

        void hassar1_FaltaPapel()
        {
            Console.WriteLine(string.Format("Evento {0} - {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        void hassar1_EventoImpresora(int Flags)
        {
            Console.WriteLine(string.Format("Evento {0} - Flags {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, Flags));
        }

        void hassar1_EventoFiscal(int Flags)
        {
            Console.WriteLine(string.Format("Evento {0} - Flags {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, Flags));
        }

        void hassar1_ErrorImpresora(int Flags)
        {
            Console.WriteLine(string.Format("Evento {0} - Flags {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, Flags));
        }

        void hassar1_ErrorFiscal(int Flags)
        {
            Console.WriteLine(string.Format("Evento {0} - Flags {1} - {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, Flags));
        }

        #endregion
        
        public void TryExecute(int action)
        {
            if(actionCommands.ContainsKey(action))
            {
                Console.WriteLine(string.Format("Acción {0} ", actionCommands[action].Key));
                Console.WriteLine("Execute? y/n"); 
                if(Console.ReadKey().Key == ConsoleKey.Y)
                    actionCommands[action].Value();
            }
        }
    }
}
