using AccessibilityUnitTest;
using Axe.Windows.Automation;
using Axe.Windows.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Automation;

namespace AccessibilityUnitTest
{
    [TestClass]
    public class UnitTest : GallerySession
    {
        private static AutomationElement GalleryWindow;
        private static IScanner scanner;

        [TestMethod]
        public void TestMethod()
        {
            Condition nameCondition = new PropertyCondition(
                    AutomationElement.NameProperty,
                    "Navigation menu");
            var basicInput = GalleryWindow.FindFirst(TreeScope.Subtree, nameCondition);
            Assert.IsNotNull(basicInput);
            if (basicInput != null)
            {
                basicInput.TryGetCurrentPattern(System.Windows.Automation.InvokePattern.Pattern, out object pattern);
                Assert.IsNotNull(pattern);
                pattern.GetType().GetMethod("Invoke").Invoke(pattern, null);
                PrintScanResults(GetScanResults(scanner), false);
                Assert.IsFalse(IsErrorPresent(GetScanResults(scanner)));
            }
        }

        [TestMethod]
        public void TestAccessibilityElement()
        {
            Condition nameCondition = new PropertyCondition(
                    AutomationElement.NameProperty,
                    "Accessibility");
            var accessibilityElement = GalleryWindow.FindFirst(TreeScope.Subtree, nameCondition);
            Assert.IsNotNull(accessibilityElement, "Element with name 'accessibility' was not found");

            if (accessibilityElement != null)
            {
                Console.WriteLine($"Found accessibility element: {accessibilityElement.Current.Name}");
                Console.WriteLine($"Control Type: {accessibilityElement.Current.ControlType.LocalizedControlType}");

                // Try to get the ExpandCollapse pattern
                if (accessibilityElement.TryGetCurrentPattern(System.Windows.Automation.ExpandCollapsePattern.Pattern, out object expandCollapsePattern))
                {
                    var expandCollapse = (System.Windows.Automation.ExpandCollapsePattern)expandCollapsePattern;
                    Console.WriteLine($"Current ExpandCollapse State: {expandCollapse.Current.ExpandCollapseState}");

                    // Expand if collapsed, collapse if expanded
                    if (expandCollapse.Current.ExpandCollapseState == ExpandCollapseState.Collapsed)
                    {
                        Console.WriteLine("Expanding accessibility element...");
                        expandCollapse.Expand();
                        System.Threading.Thread.Sleep(1000); // Wait for expansion
                        Console.WriteLine($"New state after expand: {expandCollapse.Current.ExpandCollapseState}");
                    }
                    else if (expandCollapse.Current.ExpandCollapseState == ExpandCollapseState.Expanded)
                    {
                        Console.WriteLine("Collapsing accessibility element...");
                        expandCollapse.Collapse();
                        System.Threading.Thread.Sleep(1000); // Wait for collapse
                        Console.WriteLine($"New state after collapse: {expandCollapse.Current.ExpandCollapseState}");
                    }
                    else
                    {
                        Console.WriteLine("Element is in a partially expanded state or doesn't support expand/collapse");
                    }
                }
                else
                {
                    Console.WriteLine("ExpandCollapse pattern not supported on this element");
                }

                PrintScanResults(GetScanResults(scanner), false);
            }
        }
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            try
            {
                int processId = GetProcessId();
                scanner = CreateScanner(processId);
                //var scanResults = GetScanResults(scanner);
                //PrintScanResults(scanResults, false);

                AutomationElement desktop = AutomationElement.RootElement;

                Condition windowCondition = new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                System.Windows.Automation.ControlType.Window);

                Condition nameCondition = new PropertyCondition(
                    AutomationElement.NameProperty,
                    "React Native Gallery");

                Condition condition = new AndCondition(windowCondition, nameCondition);

                GalleryWindow = desktop.FindFirst(
                    TreeScope.Children,
                    condition);
                Assert.IsNotNull(GalleryWindow);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during accessibility scan: {ex.Message}");
                throw;
            }
        }
    }
}
