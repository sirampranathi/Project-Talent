using MVPStudio.Framework.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MVPStudio.Framework.Base
{
    public class BasePage
    {
        public BasePage CurrentPage { get; set; }

        public TPage GetInstance<TPage>(IWebDriver driver) where TPage : BasePage
        {
            return Activator.CreateInstance(typeof(TPage), driver) as TPage;
        }

        public TPage As<TPage>() where TPage : BasePage
        {
            return (TPage)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object CreateLambdaExpressionForPageProperty(string propertyName)
        {
            // the purpose of this method is to get around the fact that 
            // when we are trying to retrieve page elements,
            // using reflection DOES NOT reveal them if they were
            // declared as expresion bodied properties
            // thus, expression like below needs to be created as a workaround
            // page => page.[PROPERTY_NAME]
            if (propertyName == null)
            {
                throw new ArgumentNullException($"{propertyName} is null");
            }

            ParameterExpression parameter = Expression.Parameter(GetType(), "page");
            MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
            var expression = Expression.Lambda<Func<BasePage, object>>(property, parameter);
            var compiledExpressoin = expression.Compile();
            return compiledExpressoin(this);
        }

        public string GetFieldValidationMessage(string fieldName)
        {
            var element = CreateLambdaExpressionForPageProperty(fieldName);
            if (element == null)
            {
                // no such element exists
                return null;
            }

            if (element.GetType().IsGenericType && typeof(IEnumerable).IsAssignableFrom(element.GetType()))
            {
                // excluding duplicate message
                HashSet<string> validationMessages = new HashSet<string>();

                foreach (var field in (IList<IWebElement>)element)
                {
                    var value = field.GetFieldValidationMessage();
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    validationMessages.Add(value);
                }
                return string.Join(",", validationMessages);
            }

            return ((IWebElement)element).GetFieldValidationMessage();
        }

        public string GetFieldValue(string fieldName)
        {
            var element = CreateLambdaExpressionForPageProperty(fieldName);

            if (element == null)
            {
                // no such element exists
                return null;
            }
            try
            {
                // if returned type is collections
                if (element.GetType().IsGenericType && typeof(IEnumerable).IsAssignableFrom(element.GetType()))
                {
                    string value = null;
                    foreach (var field in (IList<IWebElement>)element)
                    {
                        value += field.GetFieldValue() ?? "";
                    }
                    return value;
                }

                return ((IWebElement)element).GetFieldValue();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}