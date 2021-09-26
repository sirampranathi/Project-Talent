using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace MVPStudio.Framework.Extensions
{
    public static class WebElementExtension
    {
        /// <summary>
        /// Wait the element to be clickable
        /// <example>For example
        /// <code>
        /// Element.WaitForClickable(Driver);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="timeOutinSeconds"></param>
        public static void WaitForClickable(this IWebElement element, IWebDriver driver, int timeOutinSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException("Waiting for element to be clickable failed");
            }
        }

        /// <summary>
        /// Wait for element displayed
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="timeOutinSeconds"></param>
        public static void WaitForDisplayed(this IWebElement element, IWebDriver driver, int timeOutinSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
                wait.Until(x => element.Displayed == true);
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException("Waiting for element to display failed");
            }
        }

        /// <summary>
        /// Wait for the inner text of element loaded
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="timeOutinSeconds"></param>
        public static void WaitForTextLoaded(this IWebElement element, IWebDriver driver, int timeOutinSeconds = 10)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutinSeconds));
                wait.Until(x => !string.IsNullOrEmpty(element.Text));
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException("Wait for text loaded failed");
            }
        }

        /// <summary>
        /// Wait for menu item displayed and Select menu item from dropdown list
        /// <example>For example
        /// <code>
        /// DropdownMenu.SelectFromDropdownList("menu")
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="element">The parent element of menu item</param>
        /// <param name="driver"></param>
        /// <param name="menuItem"></param>
        public static void SelectFromDropdownList(this IWebElement element, IWebDriver driver, string menuItem)
        {
            try
            {
                var option = element.FindElement(By.XPath("//li[contains(.,'" + menuItem + "')]"));
                option.WaitForDisplayed(driver);
                option.Click();
            }
            catch (NoSuchElementException)
            {
                throw new NoSuchElementException("The menu item doesn't exist in drop down list");
            }
        }

        /// <summary>
        /// Hover on element
        /// <example>For example
        /// <code>
        /// Element.Hover(Driver)
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        public static void Hover(this IWebElement element, IWebDriver driver)
        {
            var action = new Actions(driver);
            action.MoveToElement(element).Perform();
        }

        /// <summary>
        /// Clear and send Text to textbox or textarea
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text">The text to input to the control</param>
        public static void EnterText(this IWebElement element, string text)
        {
            if (element == null)
            {
                throw new ArgumentNullException($"{nameof(element)} is null");
            }

            if (string.IsNullOrEmpty(text))
            {
                // do nothing
                return;
            }
            element.SendKeys(text);
        }

        /// <summary>
        /// Get Inner Text of element
        /// if the text is loaded, return the text
        /// otherwise return String.Empty
        /// <example>
        /// Element.GetInnerText(Driver)
        /// </example>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="timeOutInSeconds"></param>
        /// <returns>text or String.Empty</returns>
        public static string GetInnerText(this IWebElement element, IWebDriver driver)
        {
            try
            {
                element.WaitForTextLoaded(driver);
                return element.Text;
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if the header has been expanded 
        /// if yes, do nothing
        /// if no, Expand the header
        /// <example>For example
        /// In Profile Page, each section has an expandable header.
        /// In order to view or edit the section, user need to expand the header first.
        /// <code>
        /// HeaderElement.ExpandHeader();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="element"></param>
        public static void ExpandHeader(this IWebElement element)
        {
            if (element.GetAttribute("aria-expanded") == "false")
            {
                element.Click();
            }
        }

        /// <summary>
        /// Search data from a single page table by data set condition (columnNumber, columnValue), return row number
        /// if the data entry is found, return row number
        /// otherwise return 0, searching failed
        /// </summary>
        /// <param name="element">The table body element</param>
        /// <param name="driver"></param>
        /// <param name="dataset">key value pair dataset. Key is the column number, Value is the column value</param>
        /// <returns>row number or 0</returns>
        public static int SearchDataFromTable(this IWebElement element, IWebDriver driver, IDictionary<int, string> dataset)
        {
            IList<IWebElement> rows = element.FindElements(By.TagName("tr"));

            for (int i = 0; i < rows.Count; i++)
            {
                IList<IWebElement> columns = rows[i].FindElements(By.TagName("td"));
                int j = 0;
                foreach (KeyValuePair<int, string> kvp in dataset)
                {
                    int columnNumber = kvp.Key;
                    string columnValue = kvp.Value;
                    if (columns[columnNumber - 1].GetInnerText(driver) == columnValue)
                    {
                        j++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if (j == dataset.Count)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Verify if the data entry exists in a single page table according to the data set condition
        /// if yes, return true
        /// otherwise return false
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="dataset"></param>
        /// <returns>true or false</returns>
        public static bool IsDataPresentInTable(this IWebElement element, IWebDriver driver, IDictionary<int, string> dataset)
        {
            if (element.SearchDataFromTable(driver, dataset) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method returns the validation message for particular <paramref name="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetFieldValidationMessage(this IWebElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException($"{element} is null");
            }
            return element.FindElement(By.XPath("ancestor::div[contains(@class, 'ant-form-item-control') and contains(@class,'has-error')]/div[@class='ant-form-explain']"))?.Text;
        }

        /// <summary>
        /// This method returns the value of a particular <paramref name="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetFieldValue(this IWebElement element)
        {
            if (string.Equals(element.TagName, "textarea", StringComparison.OrdinalIgnoreCase))
            {
                return element.Text;
            }
            else if (element.GetAttribute("type") == "checkbox")
            {
                // handling checkbox field
                var parentElement = element.FindElement(By.XPath("parent::span"));
                if (parentElement != null)
                {
                    string classes = parentElement.GetAttribute("class");
                    if (string.IsNullOrEmpty(classes))
                    {
                        return "";
                    }
                    else if (classes.Contains("ant-checkbox-checked", StringComparison.Ordinal))
                    {
                        return element.FindElement(By.XPath("parent::span/following-sibling:span"))?.Text ?? "";
                    }
                }
                return "";
            }
            else if (element.GetAttribute("value") != null)
            {
                // normal input fields like textfields should be caught here
                return element.GetAttribute("value");
            }
            else if (!string.IsNullOrEmpty(element.GetAttribute("class")))
            {
                string classes = element.GetAttribute("class");
                if (string.IsNullOrEmpty(classes))
                {
                    return "";
                }
                else if (classes.Contains("ant-select", StringComparison.Ordinal))
                {
                    // dropdown/select 
                    return element.FindElement(By.CssSelector("ant-select-selection-selected-value"))?.Text;
                }
                else if (classes.Contains("ql-editor", StringComparison.Ordinal))
                {
                    // similar to textarea but seems to be slighly different
                    return element.FindElement(By.CssSelector("p"))?.Text;
                }
                else if (classes.Contains("ant-slider-handle", StringComparison.Ordinal))
                {
                    // handling sliders
                    return element.GetAttribute("aria-valuenow");
                }
            }
            return null;
        }

        /// <summary>
        /// This method, as the name suggests, drags and drops <paramref name="element"/> based on 
        /// <paramref name="offsetX"/> and <paramref name="offsetY"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="offsetX">How much to move in X axis</param>
        /// <param name="offsetY">How much to move in Y axis</param>
        public static void DragAndDrop(this IWebElement element, IWebDriver driver,  int offsetX = 0, int offsetY = 0)
        {
            new Actions(driver).DragAndDropToOffset(element, offsetX, offsetY).Build().Perform();
        }

        /// <summary>
        /// This method selects a particular dropdown option with a particular <paramref name="value"/>
        /// </summary>
        /// <param name="dropDownElement"></param>
        /// <param name="driver"></param>
        /// <param name="value"></param>
        public static void SelectDropDownValue(this IWebElement dropDownElement, IWebDriver driver, string value)
        {
            // with the way things are with ANT Design UI Framework
            // to select a drop down option, you first must obtain 'aria-controls' attribute
            // then use that attribute to get all the options

            if (dropDownElement == null)
            {
                throw new ArgumentNullException($"{nameof(dropDownElement)} is null");
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            string optionAttributeKey = "aria-controls";
            string optionsId = dropDownElement.GetAttribute(optionAttributeKey);
            if (optionsId == null)
            {
                throw new ArgumentException($"Element does not contain attribute '{optionAttributeKey}'");
            }

            new Actions(driver).MoveToElement(dropDownElement)
                .Click()
                .SendKeys(value)
                .Build()
                .Perform(); // click, search for value

            // get options list
            IWebElement option = driver.WaitForClickable(By.XPath($"//div[@id='{optionsId}']/ul[@role='listbox']/li[@role='option' and .='{value}']"));
            option.Click();
        }

        /// <summary>
        /// This method clicks <paramref name="date"/> and types out <paramref name="date"/>.
        /// </summary>
        /// <param name="dateElement"></param>
        /// <param name="driver"></param>
        /// <param name="date">String representation of Date</param>
        public static void SelectDate(this IWebElement dateElement, IWebDriver driver, string date)
        {
            if (dateElement == null)
            {
                throw new ArgumentNullException($"{nameof(dateElement)} is null");
            }

            if (string.IsNullOrEmpty(date))
            {
                return;
            }

            // wait for element to be actionable
            dateElement.WaitForClickable(driver);
            new Actions(driver).MoveToElement(dateElement).Click().Build().Perform();

            IWebElement calendar = driver.WaitForClickable(By.CssSelector("input.ant-calendar-input"));
            calendar.EnterText(date);
            // clicking body element so that field value is saved
            driver.FindElement(By.CssSelector("body")).Click();
        }

    }
}