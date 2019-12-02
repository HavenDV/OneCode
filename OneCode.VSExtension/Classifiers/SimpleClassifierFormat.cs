using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace OneCode.VsExtension.Classifiers
{
    /// <summary>
    /// Defines an editor format for the SimpleClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "SimpleClassifier")]
    [Name("SimpleClassifier")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class SimpleClassifierFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClassifierFormat"/> class.
        /// </summary>
        public SimpleClassifierFormat()
        {
            DisplayName = "SimpleClassifier"; // Human readable version of the name
            //BackgroundColor = Colors.BlueViolet;
            //TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
}
