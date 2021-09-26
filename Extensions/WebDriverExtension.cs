using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVPStudio.Framework.Extensions
{
    public static class WebDriverExtension
    {
        /// <summary>
        ///  This method applies 10s default wait for "FindElement" command
        ///  Recommended to use instead of "FindElement"
        ///  <example> For example:
        ///  <code>
        ///  var element = Driver.WaitForElement(By.("Locator"));
        ///  var element = Driver.WaitForElement(By.("Locator"), 20);
        ///  </code>
        ///  </example>
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by">Locator</param>
        /// <param name="timeOutinSeconds">WaitTimeOut(default as 10, assign other value as needed)</param>
        /// <returns>IWebElement</returns>
        public static IWebElement WaitForElement(this IWebDriver driver, By by, int timeOutinSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
                return wait.Until(d => d.FindElement(by));
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverException($"Wait for Element {by} failed");
            }
        }

        /// <summary>
        /// This method waits for the "Title" of HTML Page loaded
        /// And waits for the document state to be "complete"
        /// And waits for all the AJAX calls to be completed
        /// <example> For example:
        ///  <code>
        ///  Driver.WaitForPageLoaded("title")
        ///  </code>
        ///  </example>
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="pageTitle"></param>
        /// <param name="timeOutinSeconds"></param>
        public static void WaitForPageLoaded(this IWebDriver driver, string pageTitle, int timeOutinSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
                wait.Until((x) =>
                {

                    bool ajaxComplete;
                    bool jsReady;
                    bool titleDisplayed = false;

                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    jsReady = (bool)js.ExecuteScript("return (document.readyState == \"complete\" || document.readyState == \"interactive\")"); ;

                    try
                    {
                        ajaxComplete = (bool)js.ExecuteScript("var result = true; try { result = (typeof jQuery != 'undefined') ? jQuery.active == 0 : true } catch (e) {}; return result;");
                    }
                    catch (Exception)
                    {
                        ajaxComplete = true;
                    }

                    try
                    {
                        titleDisplayed = x.Title.Contains(pageTitle, StringComparison.OrdinalIgnoreCase);
                    }
                    catch (Exception)
                    {
                        // do nothing
                    }
                    return ajaxComplete && jsReady && titleDisplayed;
                });
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException($"Wait for the title of Page {pageTitle} failed");
            }
        }
        /// <summary>
        /// Wait for all elements specified to be loaded and displayed
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeOutinSeconds"></param>
        /// <returns>IWebElement</returns>
        public static IList<IWebElement> WaitForElements(this IWebDriver driver, By ele, int timeOutinSeconds = 20)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
            return wait.Until(x =>
            {
                try
                {
                    var elements = driver.FindElements(ele);
                    if (elements.Any(element => !element.Displayed))
                    {
                        return null;
                    }

                    return elements.Any() ? elements : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Wait for all elements specified to be enabled
        /// This is to handle weird issues exhibited with checkboxes
        /// where checkbox element seems to be 'not displayed'
        /// and yet is 'enabled'
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeOutinSeconds"></param>
        /// <returns>IWebElement</returns>
        public static IList<IWebElement> WaitForElementsToBeClickable(this IWebDriver driver, By ele, int timeOutinSeconds = 20)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
            return wait.Until(x =>
            {
                try
                {
                    var elements = driver.FindElements(ele);
                    if (elements.Any(element => !element.Enabled))
                    {
                        return null;
                    }

                    return elements.Any() ? elements : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Wait for element to be displayed
        /// </summary>
        /// <example>For example
        /// <code>
        /// Driver.WaitForDisplayed(by)
        /// </code>
        /// </example>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeOutinSeconds"></param>
        /// <returns>IWebElement</returns>
        public static IWebElement WaitForDisplayed(this IWebDriver driver, By by, int timeOutinSeconds = 10)
        {
            try
            {
                var element = driver.WaitForElement(by);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
                wait.Until(d => element.Displayed);
                return element;

            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverException($"Wait for element {by}  to be displayed failed");
            }
        }

        /// <summary>
        /// Wait for element to be clickable
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeOutInSeconds"></param>
        /// <returns></returns>
        public static IWebElement WaitForClickable(this IWebDriver driver, By by, int timeOutInSeconds = 10)
        {
            try
            {
                var element = driver.WaitForElement(by);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutInSeconds));
                wait.Until(d => element.Enabled);
                return element;
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException($"Wait for element {by} to be clickable failed ");
            }
        }

        /// <summary>
        /// This method clears localstorage through executing javascript
        /// <example> For Example
        /// <code>
        /// Driver.ResetDriver()
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="driver"></param>
        public static void ResetDriver(this IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("localStorage.clear();");
        }

        /// <summary>
        /// This method identifies if the success message is returned
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>true or false</returns>
        public static bool IsSuccessMessageReturned(this IWebDriver driver)
        {
            try
            {
                var element = driver.WaitForElement(By.CssSelector("div.ant-message-success"));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// This method gets the message text
        /// if no error message is returned, it returns "Empty"
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>message text or "String.Empty"</returns>
        public static string GetMessage(this IWebDriver driver)
        {
            var element = driver.WaitForElement(By.CssSelector("div.ant-message span"));
            return element.GetInnerText(driver);
        }

        /// <summary>
        /// This method enables navigating to the specified page by page number
        /// <example>For example
        /// <code>
        /// Driver.GoToPageNumber(6);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="pageNumber"></param>
        public static void GoToPageNumber(this IWebDriver driver, int pageNumber)
        {
            IList<IWebElement> Pages = driver.FindElements(By.CssSelector("ul.ant-pagination li.ant-pagination-item"));
            if (pageNumber > Pages.Count || pageNumber < 1)
            {
                throw (new ArgumentOutOfRangeException("The specified page number exceed the range"));
            }
            else
            {
                Pages[pageNumber - 1].Click();
            }
        }

        /// <summary>
        /// This method iterates each page and verifies the filtered results
        /// if the results satisfy the condition, return true
        /// otherwise return false
        /// <example>For example
        /// <code>
        /// bool result = Driver.VerifyFilteredResults<string>(VerifyByTitle, "title");
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="IDictionary<TKey, TValue>">The filter type can be any type</typeparam>
        /// <param name="driver"></param>
        /// <param name="verifyOnEachPage">a method to verify the results in each page according to the filter</param>
        /// <param name="filter"></param>
        /// <returns>true or false</returns>
        public static bool VerifyFilteredResults<TKey, TValue>(this IWebDriver driver, Func<IDictionary<TKey, TValue>, bool> verifyOnEachPage, IDictionary<TKey, TValue> filter)
        {
            IList<IWebElement> Pages = driver.FindElements(By.CssSelector("ul.ant-pagination li.ant-pagination-item"));
            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].Click();
                if (verifyOnEachPage(filter) == true)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method iterates each page and searches the specified dataset
        /// if the item is found, return page number
        /// otherwise searching failed, return 0
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="searchOnEachPage">a method to search the item on each page</param>
        /// <param name="dataset">key value pair dataset</param>
        /// <returns>pageNumber or 0</returns>
        public static int SearchDataThroughPages<TKey, TValue>(this IWebDriver driver, Func<IWebDriver, IDictionary<TKey, TValue>, bool> searchOnEachPage, IDictionary<TKey, TValue> dataset)
        {
            IList<IWebElement> Pages = driver.FindElements(By.CssSelector("ul.ant-pagination li.ant-pagination-item"));
            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].Click();
                if (searchOnEachPage(driver, dataset) == true)
                {
                    return i + 1;
                }
                else
                {
                    continue;
                }
            }
            return 0;
        }

        /// <summary>
        /// This method iterated each page and check if the data entry exists according to the data set condition
        /// if yes, return true
        /// otherwise return false
        /// <example>For Example
        /// <code>
        /// bool result = Driver.HasDataExisted(searchOnEachPage, dataset);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="driver"></param>
        /// <param name="searchOnEachPage"></param>
        /// <param name="dataset"></param>
        /// <returns>true or false</returns>
        public static bool HasDataExisted<TKey, TValue>(this IWebDriver driver, Func<IWebDriver, IDictionary<TKey, TValue>, bool> searchOnEachPage, IDictionary<TKey, TValue> dataset)
        {
            if (driver.SearchDataThroughPages(searchOnEachPage, dataset) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Search table data through pages according to the specified data set
        /// if the table data is found, return page number
        /// otherwise, return 0
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="dataset"></param>
        /// <returns>pageNumber or 0</returns>
        public static int SearchTableDataThroughPages(this IWebDriver driver, IDictionary<int, string> dataset)
        {
            var tbodyElement = driver.WaitForElement(By.TagName("tbody"));
            return driver.SearchDataThroughPages(tbodyElement.IsDataPresentInTable, dataset);
        }

        /// <summary>
        /// This method iterates each page and check if the table data exists according to the specified data set
        /// if the table data is found, return true
        /// otherwise, return false
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="dataset"></param>
        /// <returns>true or false</returns>
        public static bool HasTableDataExisted(this IWebDriver driver, IDictionary<int, string> dataset)
        {
            var tbodyElement = driver.WaitForElement(By.TagName("tbody"));
            return driver.HasDataExisted(tbodyElement.IsDataPresentInTable, dataset);
        }

        /// <summary>
        /// This method finds all the elements with css class of 'ant-form-explain'<br/>
        /// And returns their texts i.e. this method returns all field validation messages
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static List<string> GetAllFieldValidationMessages(this IWebDriver driver)
        {
            return driver.FindElements(By.CssSelector(".ant-form-explain")).Select(e => e.Text).ToList();
        }

        /// <summary>
        /// This method finds all the checkboxes based on <paramref name="locator"/><br/>
        /// And iterates through them and ticks if it matches one of <paramref name="optionsToSelect"/>.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator">Locator for checkbox group</param>
        /// <param name="optionsToSelect">List of checkbox values/labels</param>

        public static void SelectFromCheckBoxGroup(this IWebDriver driver, By locator, IList<string> optionsToSelect)
        {
            if (locator == null)
            {
                // check for size too?
                throw new ArgumentNullException($"{nameof(locator)} is null");
            }

            if (optionsToSelect == null || optionsToSelect.Count == 0)
            {
                return;
            }

            IList<IWebElement> checkBoxGroup = driver.WaitForElementsToBeClickable(locator);
            foreach (var checkBox in checkBoxGroup)
            {
                string text = checkBox.FindElement(By.XPath("parent::span/following-sibling::span")).Text;
                if (text == null)
                {
                    continue;
                }
                if (optionsToSelect.Contains(text))
                {
                    checkBox.Click();
                }
            }
        }
    }
}