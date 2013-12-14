#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of SharpIRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace SharpIRC.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public sealed partial class AboutWindow
    {
        /// <summary>
        /// Default constructor is protected so callers must use one with a parent.
        /// </summary>
        private AboutWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor that takes a parent for this AboutWindow dialog.
        /// </summary>
        /// <param name="parent">Parent window for this dialog.</param>
        public AboutWindow(Window parent)
            : this()
        {
            Owner = parent;
        }

        /// <summary>
        /// Handles click navigation on the hyperlink in the About dialog.
        /// </summary>
        /// <param name="sender">Object the sent the event.</param>
        /// <param name="e">Navigation events arguments.</param>
        private void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }

        #region AboutData Provider
        #region Member data
        private XmlDocument _xmlDoc;

        private const string PropertyNameTitle = "Title";
        private const string PropertyNameProduct = "Product";
        private const string PropertyNameCopyright = "Copyright";
        private const string XPathRoot = "ApplicationInfo/";
        private const string XPathTitle = XPathRoot + PropertyNameTitle;
        private const string XPathVersion = XPathRoot + "Version";
        private const string XPathProduct = XPathRoot + PropertyNameProduct;
        private const string XPathCopyright = XPathRoot + PropertyNameCopyright;
        private const string XPathLink = XPathRoot + "Link";
        private const string XPathLinkUri = XPathRoot + "Link/@Uri";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        public string ProductTitle
        {
            get
            {
                string result = CalculatePropertyValue<AssemblyTitleAttribute>(PropertyNameTitle, XPathTitle);
                if (string.IsNullOrEmpty(result))
                {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        public string Version
        {
            get
            {
                // first, try to get the version string from the assembly.
                Version value = Assembly.GetExecutingAssembly().GetName().Version;
                return value != null ? value.ToString() : GetLogicalResourceString(XPathVersion);
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            //get { return CalculatePropertyValue<AssemblyDescriptionAttribute>(PropertyNameDescription, XPathDescription); }
            get
            {
                return "This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software. \n\n" +
                        "1.	Definitions\n" +
                            "The terms \"reproduce,\" \"reproduction,\" \"derivative works,\" and \"distribution\" have the same meaning here as under U.S. copyright law.\n" +
                            "A \"contribution\" is the original software, or any additions or changes to the software.\n" +
                            "A \"contributor\" is any person that distributes its contribution under this license.\n" +
                            "Licensed patents\" are a contributor's patent claims that read directly on its contribution.\n\n" +
                        "2.	Grant of Rights\n" +
                            "(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.\n" +
                            "(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.\n\n" +
                        "3.	Conditions and Limitations\n" +
                            "(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.\n" +
                            "(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.\n" +
                            "(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.\n" +
                            "(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.\n" +
                            "(E) The software is licensed \"as-is.\" You bear the risk of using it. The contributors give no express warranties, guarantees, or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.\n\n";
            }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return CalculatePropertyValue<AssemblyProductAttribute>(PropertyNameProduct, XPathProduct); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return CalculatePropertyValue<AssemblyCopyrightAttribute>(PropertyNameCopyright, XPathCopyright); }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText
        {
            get { return GetLogicalResourceString(XPathLink); }
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUri
        {
            get { return GetLogicalResourceString(XPathLinkUri);}
        }
        #endregion

        #region Resource location methods
        /// <summary>
        /// Gets the specified property value either from a specific attribute, or from a resource dictionary.
        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <param name="xpathQuery">XPath to the element in the XML data resource.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        private string CalculatePropertyValue<T>(string propertyName, string xpathQuery)
        {
            string result = string.Empty;
            // first, try to get the property value from an attribute.
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                var attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    result = property.GetValue(attributes[0], null) as string;
                }
            }

            // if the attribute wasn't found or it did not have a value, then look in an xml resource.
            if (result == string.Empty)
            {
                // if that fails, try to get it from a resource.
                result = GetLogicalResourceString(xpathQuery);
            }
            return result;
        }

        /// <summary>
        /// Gets the XmlDataProvider's document from the resource dictionary.
        /// </summary>
        private XmlDocument ResourceXmlDocument
        {
            get
            {
                if (_xmlDoc == null)
                {
                    // if we haven't already found the resource XmlDocument, then try to find it.
                    var provider = TryFindResource("aboutProvider") as XmlDataProvider;
                    if (provider != null)
                    {
                        // save away the XmlDocument, so we don't have to get it multiple times.
                        _xmlDoc = provider.Document;
                    }
                }
                return _xmlDoc;
            }
        }

        /// <summary>
        /// Gets the specified data element from the XmlDataProvider in the resource dictionary.
        /// </summary>
        /// <param name="xpathQuery">An XPath query to the XML element to retrieve.</param>
        /// <returns>The resulting string value for the specified XML element. 
        /// Returns empty string if resource element couldn't be found.</returns>
        private string GetLogicalResourceString(string xpathQuery)
        {
            string result = string.Empty;
            // get the About xml information from the resources.
            XmlDocument doc = ResourceXmlDocument;
            if (doc != null)
            {
                // if we found the XmlDocument, then look for the specified data. 
                XmlNode node = doc.SelectSingleNode(xpathQuery);
                if (node != null)
                {
                    if (node is XmlAttribute)
                    {
                        // only an XmlAttribute has a Value set.
                        result = node.Value;
                    }
                    else
                    {
                        // otherwise, need to just return the inner text.
                        result = node.InnerText;
                    }
                }
            }
            return result;
        }
        #endregion
        #endregion
    }
}

