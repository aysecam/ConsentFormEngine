using Castle.DynamicProxy;
using ConsentFormEngine.Core.Attributes;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsentFormEngine.Business.Interceptors
{
    public class TransactionInterceptor : IInterceptor
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionInterceptor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var hasAttribute = method.GetCustomAttributes(typeof(TransactionalAttribute), true).Any();

            if (!hasAttribute)
            {
                invocation.Proceed();
                return;
            }

            _unitOfWork.BeginTransactionAsync().Wait();

            try
            {
                invocation.Proceed();

                var task = invocation.ReturnValue as Task;
                task?.GetAwaiter().GetResult();

                _unitOfWork.SaveChangesAsync().Wait();
                _unitOfWork.CommitTransactionAsync().Wait();
            }
            catch
            {
                _unitOfWork.RollbackTransactionAsync().Wait();
                throw;
            }
        }
    }
}
