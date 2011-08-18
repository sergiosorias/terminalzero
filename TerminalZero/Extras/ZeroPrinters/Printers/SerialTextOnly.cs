using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace ZeroPrinters.Printers
{
    /// <summary>Manejar impresora ticketera por puerto serie (probado con EPSON)
    /// Limitaciones:
    /// 1) El corte de papel solo está probado con EPSON (pero puede ser que funcione en otras)
    /// 2) Si se envía un texto más largo que la línea de impresión se puede perder lo que se va del ticket
    /// 3) Solo asegura imprimir caracteres ANSI (en EPSON las ñ y acentos las muestra como '?')
    /// </summary>
    public class SerialTextOnly : TextOnlyPrinterBase
    {
        private const string kBouds = "Bouds";
        private const string kDatabits = "DataBits";

        /// <summary>Constructor con parámetros de puerto serie
        /// </summary>
        /// <param name="portName">nombre del port (generalmente "com1")
        /// se pueden obtener los nombres disponibles usando System.IO.Ports.SerialPort.GetPortNames()</param>
        /// <param name="bauds">velocidad (generalmente 9600)</param>
        /// <param name="parity">tipo de paridad (generalmente None)</param>
        /// <param name="dataBits">bits de datos (generalmente 8)</param>
        /// <param name="stopBits">bits de parada (generalmente One)</param>
        /// <param name="info"></param>
        public SerialTextOnly(PrinterInfo info)
            : base(info)
        {
            try
            {
                commPort = new SerialPort(Name, int.Parse(info.Parameters[kBouds]), Parity.None, int.Parse(info.Parameters[kDatabits]), StopBits.One);
                IsOnLine = true;
                LastError = null;
                PaperCut = CutMode.Partial;
                PaperCutJump = 32;
                UseCrLf = false;
            }
            catch (IOException ex)
            {
                IsOnLine = false;
                LastErrorDesc = "Configuración de puerto no válida";
                LastError = ex.ToString();
            }
            catch (Exception ex)
            {
                IsOnLine = false;
                LastErrorDesc = "Error no contemplado";
                LastError = ex.ToString();
            }
        }

        #region Enums
        /// <summary>Modo de corte de papel al terminar el ticket
        /// </summary>
        public enum CutMode
        {
            /// <summary>No cortar el papel al terminar el ticket
            /// </summary>
            None,
            /// <summary>Cortar el papel parcialmente al terminar el ticket
            /// </summary>
            Partial,
            /// <summary>Cortar el papel completamente al terminar el ticket
            /// </summary>
            Full
        }
        #endregion

        #region Instance Data
        private readonly SerialPort commPort;
        
        #endregion

        #region Public Properties

        public override bool IsOnLine { get; protected set; }

        public string LastErrorDesc { get; protected set; }
        /// <summary>Indica si para salto de línea se envía Cr + Lf
        /// </summary>
        public bool UseCrLf { get; set; }

        /// <summary>Indica si se corta el ticket al cerrar el envío
        /// </summary>
        public CutMode PaperCut { get; set; }

        /// <summary>Indica un salto extra de papel antes de cortar el ticket
        /// Es un valor en pixels de 0 a 255 (generalmente 32)
        /// </summary>
        public byte PaperCutJump { get; set; }
        #endregion

        #region Public Methods

        /// <summary>Abrir el puerto con los seteos pasados al constructor
        /// </summary>
        public void Open()
        {
            if (IsOnLine && !commPort.IsOpen)
            {
                LastError = null;
                LastErrorDesc = string.Empty;
                try
                {
                    commPort.Open();
                }
                catch (IOException ex)
                {
                    LastError = ex.ToString();
                    LastErrorDesc = "Error de acceso al puerto";
                }
                catch (UnauthorizedAccessException ex)
                {
                    LastError = ex.ToString();
                    LastErrorDesc = "No hay permisos de acceso al puerto";
                }
            }
        }

        /// <summary>Cerrar el ticket
        /// </summary>
        public void Close()
        {
            if (commPort.IsOpen)
            {
                if (PaperCut != CutMode.None)
                {
                    // cortar ticket
                    var pb = new byte[64];
                    pb[0] = 0x1d; pb[1] = 0x56; pb[2] = (PaperCut == CutMode.Full) ? (byte)0x41 : (byte)0x42;
                    pb[3] = PaperCutJump; // espacio a saltar antes de cortar
                    commPort.Write(pb, 0, 4);
                }

                commPort.Close();
            }
        }

        /// <summary>Imprimir un texto
        /// Conviene siempre enviar un string de largo menor o igual al ancho en columnas
        /// Una vez enviado el string el cursor de la impresora queda parado al final del string
        /// Se imprime cuando la impresora recibe un linefeed (que puede ser un enter en el string)
        /// Es conveniente enviar strings sin enter y usar el método LineFeed() para avanzar de línea
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override void Print()
        {
            if (IsOnLine && commPort.IsOpen)
            {
                try
                {
                    commPort.Write(Data.ToString());
                }
                catch (ArgumentNullException ex)
                {
                    LastError = ex.ToString();
                    LastErrorDesc = "Error en datos enviados";
                }
                catch (Exception ex)
                {
                    LastError = ex.ToString();
                    LastErrorDesc = "Error no contemplado";
                }

            }
        }

        /// <summary>Enviar un line feed
        /// </summary>
        /// <returns></returns>
        public bool LineFeed()
        {
            return LineFeed(1);
        }

        /// <summary>Enviar varios line feed
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool LineFeed(int lines)
        {
            if (IsOnLine && commPort.IsOpen)
            {
                try
                {
                    int datalen;
                    byte[] data;
                    if (UseCrLf)
                    {
                        datalen = lines * 2;
                        data = new byte[datalen];
                        for (int i = 0; i < lines; i++)
                        {
                            data[(i << 1)] = 13;
                            data[(i << 1) + 1] = 10;
                        }
                    }
                    else
                    {
                        datalen = lines;
                        data = new byte[datalen];
                        for (int i = 0; i < lines; i++)
                            data[i] = 10;
                    }
                    commPort.Write(data, 0, datalen);
                    return true;
                }
                catch (ArgumentNullException ex)
                {
                    LastError = ex.ToString();
                    LastErrorDesc = "Error en datos enviados";
                }
                catch (Exception ex)
                {
                    LastError = ex.ToString();
                    LastErrorDesc = "Error no contemplado";
                }

            }
            return false;
        }

        #endregion

        #region IDisposable Members
        
        public override void Dispose()
        {
            base.Dispose();
            if (IsOnLine)
            {
                Close();
            }
        }

        #endregion
    }
}
