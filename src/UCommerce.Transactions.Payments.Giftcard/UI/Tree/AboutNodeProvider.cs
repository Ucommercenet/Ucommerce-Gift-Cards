using System.Collections.Generic;
using UCommerce;
using UCommerce.Tree;
using UCommerce.Tree.Impl;

namespace SampleApp.Extensions.UI.Tree
{
	/// <summary>
	/// Adds an About node to the Settings tree in uCommerce.
	/// </summary>
    /// <remarks>
    /// To show configuration the provider accepts a list of supported node types in the construcutor,
    /// which will enable it to easily hook into multiple areas of the tree via config.
    /// Configuration is set up in the SampleApp.config file.
    /// Ditto for node name which is the text displayed in the tree for the node.
    /// </remarks>
	public class AboutNodeProvider : ITreeContentProvider
	{
		private readonly string _nodeName;
		private readonly IList<string> _supportTypes;

		/// <summary>
		/// Constructs a new AboutNodeProvider instance.
		/// </summary>
		/// <remarks>
		/// For a list of default nodeType values you can pass to "supportTypes", please <see cref="Constants.DataProvider.NodeType"/>
		/// </remarks>
		public AboutNodeProvider(string nodeName, IList<string> supportTypes)
		{
			_nodeName = nodeName;
			_supportTypes = supportTypes;
		}

		/// <summary>
		/// Given a parent node type and possibly an id, the provider returnes a list of child nodes.
		/// </summary>
		/// <param name="nodeType">The type of the parent node.</param>
		/// <param name="id">The id of the parent node.</param>
		/// <returns>A list of child nodes for the parent node.</returns>
		public IList<ITreeNodeContent> GetChildren(string nodeType, string id)
		{
            // nodeType and id can be used to determine which parent
            // we're currently generating children for, e.g. a list of sub categories
            // for a parent category.

            // In this case we do not need the detailed context to generate the About
            // node as the Supports method has previously qualified this node provider
            // for the specific area in the tree to hook into.
			return new List<ITreeNodeContent>
			{
				new TreeNodeContent("version", -1)
				{
					Name = _nodeName, // the back-end is multilingual so this has to change based on UCommerce.Globalization.ILocalizationContext
					HasChildren = false,
					Action = "/Apps/SampleApp/About.aspx",
					Icon = "/Apps/SampleApp/Media/about.png"
				}
			};
		}

		/// <summary>
		/// Returns true, if the provider supports a specific tree node type.
		/// </summary>
		/// <param name="nodeType">The type of the node.</param>
		/// <returns>true, if the node type is supported by the provider.</returns>
		public bool Supports(string nodeType)
		{
			return _supportTypes.Contains(nodeType);
		}
	}
}
