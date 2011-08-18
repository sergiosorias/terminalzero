using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroPrinters.Extras
{
    public class ZReport
    {
        private object fechaZeta;
        private object numeroZeta;
        private object cantidadDfCancelados;
        private object cantidadDnfhEmitidos;
        private object cantidadDnfEmitidos;
        private object cantidadDfEmitidos;
        private object ultimoDocFiscalBc;
        private object ultimoDocFiscalA;
        private object montoVentasDocFiscal;
        private object montoIvaDocFiscal;
        private object montoImpInternosDocFiscal;
        private object montoPercepcionesDocFiscal;
        private object montoIvaNoInscriptoDocFiscal;
        private object ultimaNotaCreditoBc;
        private object ultimaNotaCreditoA;
        private object montoVentasNotaCredito;
        private object montoIvaNotaCredito;
        private object montoImpInternosNotaCredito;
        private object montoPercepcionesNotaCredito;
        private object montoIvaNoInscriptoNotaCredito;
        private object ultimoRemito;
        private object cantidadNcCanceladas;
        private object cantidadDfbcEmitidos;
        private object cantidadDfaEmitidos;
        private object cantidadNcbcEmitidas;
        private object cantidadNcaEmitidas;

        public ZReport(object fechaZeta, object numeroZeta, object cantidadDfCancelados, object cantidadDnfhEmitidos, object cantidadDnfEmitidos, object cantidadDfEmitidos, object ultimoDocFiscalBc, object ultimoDocFiscalA, object montoVentasDocFiscal, object montoIvaDocFiscal, object montoImpInternosDocFiscal, object montoPercepcionesDocFiscal, object montoIvaNoInscriptoDocFiscal, object ultimaNotaCreditoBc, object ultimaNotaCreditoA, object montoVentasNotaCredito, object montoIvaNotaCredito, object montoImpInternosNotaCredito, object montoPercepcionesNotaCredito, object montoIvaNoInscriptoNotaCredito, object ultimoRemito, object cantidadNcCanceladas, object cantidadDfbcEmitidos, object cantidadDfaEmitidos, object cantidadNcbcEmitidas, object cantidadNcaEmitidas)
        {
            this.numeroZeta = numeroZeta;
            this.fechaZeta = fechaZeta;
            this.cantidadNcaEmitidas = cantidadNcaEmitidas;
            this.cantidadNcbcEmitidas = cantidadNcbcEmitidas;
            this.cantidadDfaEmitidos = cantidadDfaEmitidos;
            this.cantidadDfbcEmitidos = cantidadDfbcEmitidos;
            this.cantidadNcCanceladas = cantidadNcCanceladas;
            this.ultimoRemito = ultimoRemito;
            this.montoIvaNoInscriptoNotaCredito = montoIvaNoInscriptoNotaCredito;
            this.montoPercepcionesNotaCredito = montoPercepcionesNotaCredito;
            this.montoImpInternosNotaCredito = montoImpInternosNotaCredito;
            this.montoIvaNotaCredito = montoIvaNotaCredito;
            this.montoVentasNotaCredito = montoVentasNotaCredito;
            this.ultimaNotaCreditoA = ultimaNotaCreditoA;
            this.ultimaNotaCreditoBc = ultimaNotaCreditoBc;
            this.montoIvaNoInscriptoDocFiscal = montoIvaNoInscriptoDocFiscal;
            this.montoPercepcionesDocFiscal = montoPercepcionesDocFiscal;
            this.montoImpInternosDocFiscal = montoImpInternosDocFiscal;
            this.montoIvaDocFiscal = montoIvaDocFiscal;
            this.montoVentasDocFiscal = montoVentasDocFiscal;
            this.ultimoDocFiscalA = ultimoDocFiscalA;
            this.ultimoDocFiscalBc = ultimoDocFiscalBc;
            this.cantidadDfEmitidos = cantidadDfEmitidos;
            this.cantidadDnfEmitidos = cantidadDnfEmitidos;
            this.cantidadDnfhEmitidos = cantidadDnfhEmitidos;
            this.cantidadDfCancelados = cantidadDfCancelados;
        }

        #region Properties

        public object FechaZeta
        {
            get { return fechaZeta; }
        }
        
        public object CantidadNcaEmitidas
        {
            get { return cantidadNcaEmitidas; }
        }

        public object CantidadNcbcEmitidas
        {
            get { return cantidadNcbcEmitidas; }
        }

        public object CantidadDfaEmitidos
        {
            get { return cantidadDfaEmitidos; }
        }

        public object CantidadDfbcEmitidos
        {
            get { return cantidadDfbcEmitidos; }
        }

        public object CantidadNcCanceladas
        {
            get { return cantidadNcCanceladas; }
        }

        public object UltimoRemito
        {
            get { return ultimoRemito; }
        }

        public object MontoIvaNoInscriptoNotaCredito
        {
            get { return montoIvaNoInscriptoNotaCredito; }
        }

        public object MontoPercepcionesNotaCredito
        {
            get { return montoPercepcionesNotaCredito; }
        }

        public object MontoImpInternosNotaCredito
        {
            get { return montoImpInternosNotaCredito; }
        }

        public object MontoIvaNotaCredito
        {
            get { return montoIvaNotaCredito; }
        }

        public object MontoVentasNotaCredito
        {
            get { return montoVentasNotaCredito; }
        }

        public object UltimaNotaCreditoA
        {
            get { return ultimaNotaCreditoA; }
        }

        public object UltimaNotaCreditoBc
        {
            get { return ultimaNotaCreditoBc; }
        }

        public object MontoIvaNoInscriptoDocFiscal
        {
            get { return montoIvaNoInscriptoDocFiscal; }
        }

        public object MontoPercepcionesDocFiscal
        {
            get { return montoPercepcionesDocFiscal; }
        }

        public object MontoImpInternosDocFiscal
        {
            get { return montoImpInternosDocFiscal; }
        }

        public object MontoIvaDocFiscal
        {
            get { return montoIvaDocFiscal; }
        }

        public object MontoVentasDocFiscal
        {
            get { return montoVentasDocFiscal; }
        }

        public object UltimoDocFiscalA
        {
            get { return ultimoDocFiscalA; }
        }

        public object UltimoDocFiscalBc
        {
            get { return ultimoDocFiscalBc; }
        }

        public object CantidadDfEmitidos
        {
            get { return cantidadDfEmitidos; }
        }

        public object CantidadDnfEmitidos
        {
            get { return cantidadDnfEmitidos; }
        }

        public object CantidadDnfhEmitidos
        {
            get { return cantidadDnfhEmitidos; }
        }

        public object CantidadDfCancelados
        {
            get { return cantidadDfCancelados; }
        }

        public object NumeroZeta
        {
            get { return numeroZeta; }
        } 
        #endregion
    }
}
