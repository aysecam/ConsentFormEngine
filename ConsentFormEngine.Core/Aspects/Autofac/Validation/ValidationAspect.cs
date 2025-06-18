using Castle.DynamicProxy;
using FluentValidation;
using ConsentFormEngine.Core.CrossCuttingConcerns.Validation;
using ConsentFormEngine.Core.Utilities.Interceptors;

namespace ConsentFormEngine.Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception
    {
        private readonly Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("bu bir doğrulama sınıfı");
            }

            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType);
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];
            var entity = invocation.Arguments.FirstOrDefault(t => t.GetType().Name == entityType.Name);
            ValidationTool.Validate(validator, entity);
        }
    }
}
