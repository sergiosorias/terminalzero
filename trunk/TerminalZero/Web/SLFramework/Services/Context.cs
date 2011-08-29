using System;
using System.Reactive;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLFramework.Services
{
    public class Context
    {
        public static class ResponseValidator
        {
            public static void Hadle<T>(T operation, Action<T> success)
                where T : OperationBase
            {
                HadleOperation(operation, success, GenericError, null);
            }

            public static void Hadle<T>(T operation, Action<T> success, Action<T> error)
                where T : OperationBase
            {
                HadleOperation(operation, success, error, null);
            }

            public static void Hadle<T>(T operation, Action<T> success, Action<T> error, Action<T> final)
                where T : OperationBase
            {
                HadleOperation(operation, success, error, final);
            }

            private static void HadleOperation<T>(T operation, Action<T> success, Action<T> error, Action<T> final)
                where T : OperationBase
            {
                operation.Completed +=
                    (o, args) =>
                    {
                        if (!operation.HasError)
                        {
                            if (success != null)
                                success(operation);
                        }
                        else
                        {
                            if (error != null)
                                error(operation);
                        }

                        if (final != null)
                            final(operation);
                    };

            }

            private static void GenericError<TOperation>(TOperation operation)
                where TOperation : OperationBase
            {
                operation.MarkErrorAsHandled();
            }
        }
    }
}
