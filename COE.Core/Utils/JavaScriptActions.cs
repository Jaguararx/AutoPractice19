using OpenQA.Selenium;

namespace COE.Core.Utils
{
    public static class JavaScriptActions
    {
        /// <summary>
        /// Script for checking that all Angular page operations are complete for the entire document.
        /// </summary>
        public static string AngularIsReady() =>
            @"var testabilities = window.getAllAngularTestabilities();
                   return testabilities.every(x=>x._isZoneStable)
                           && testabilities.every(x=>x.isStable)
                           && testabilities.every(x=>!x.hasPendingMicrotasks)
                           && testabilities.every(x=>!x.hasPendingMacrotasks)";

        /// <summary>
        /// Returns a script for checking that a specific element's location and dimensions are stable on the page.
        /// Useful for situations where elements have animated transitions.
        /// </summary>
        /// <param name="by">The By locator for the target element</param>
        public static string CheckElementLocationIsStable(By by)
        {
            var elementFinderStrategy = GetElementFinderStrategy(by);

            return
                $@"return await (async function() {{
                const el = {elementFinderStrategy}
                if (el == null) {{
                    return false;
                    }}
                const initialRect = el.getBoundingClientRect();

                await new Promise(function(resolve) {{
                    setTimeout(resolve, 50);
                    }});
            
                let currentRect =  el.getBoundingClientRect();
            
                return initialRect.x === currentRect.x &&
                       initialRect.y === currentRect.y &&
                       initialRect.width === currentRect.width &&
                       initialRect.height === currentRect.height
                }})();";
        }

        /// <summary>
        /// Finds the first scrollable node in the DOM for the given child element
        /// then scrolls it vertically by the given offset.
        /// </summary>
        public static string ScrollBy(int y) =>
            $@"function getScrollableParent(node) {{
                        if (node == null) {{
                            return null;
                        }}

                        if (node.scrollHeight > node.clientHeight) {{
                            return node;
                        }} else {{
                            return getScrollableParent(node.parentNode);
                        }}
                    }}

                    let firstScrollableNode = getScrollableParent(arguments[0]);
                    if (firstScrollableNode != null) {{
                        firstScrollableNode.scrollBy(0, {y});
                    }}";

        /// <summary>
        /// Returns the appropriate JavaScript element finder strategy for the selector type specified by the supplied By locator
        /// </summary>
        /// <param name="by">The By locator to be processed</param>
        private static string GetElementFinderStrategy(By by)
        {
            string elementFinderStrategy;
            var strategy = by.GetStrategy();
            var locator = by.GetDescription();

            switch (strategy)
            {
                case "By.XPath:":
                    elementFinderStrategy = $@"document.evaluate(""{locator}"", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue";
                    break;
                default:
                    elementFinderStrategy = $@"document.querySelector(""{locator}"")";
                    break;
            }

            return elementFinderStrategy;
        }

        /// <summary>
        /// Clear the browser's local storage
        /// </summary>
        public static string ClearLocalStorage() => $@"localStorage.clear()";
    }
}