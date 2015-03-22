using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using AV.Cyclone.Sandy.Models;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone
{
    [Export]
    public class ColorProviderService
    {
        private readonly IStandardClassificationService standardClassificationService;
        private readonly IClassificationFormatMapService classificationFormatMapService;
        private readonly Dictionary<IClassificationFormatMap, SandyColorProvider> colorProviders =
            new Dictionary<IClassificationFormatMap, SandyColorProvider>();

        [ImportingConstructor]
        public ColorProviderService(IStandardClassificationService standardClassificationService, IClassificationFormatMapService classificationFormatMapService)
        {
            this.standardClassificationService = standardClassificationService;
            this.classificationFormatMapService = classificationFormatMapService;
        }

        public SandyColorProvider GetColorProvider(IWpfTextView textView)
        {
            var classificationFormatMap = classificationFormatMapService.GetClassificationFormatMap(textView);

            SandyColorProvider sandyColorProvider;
            if (colorProviders.TryGetValue(classificationFormatMap, out sandyColorProvider))
                return sandyColorProvider;

            classificationFormatMap.ClassificationFormatMappingChanged += ClassificationFormatMapOnClassificationFormatMappingChanged;

            sandyColorProvider = new SandyColorProvider();

            SetColors(classificationFormatMap, sandyColorProvider);

            colorProviders.Add(classificationFormatMap, sandyColorProvider);
            return sandyColorProvider;
        }

        private void ClassificationFormatMapOnClassificationFormatMappingChanged(object sender, EventArgs eventArgs)
        {
            var classificationFormatMap = (IClassificationFormatMap)sender;
            SetColors(classificationFormatMap, colorProviders[classificationFormatMap]);
        }

        private void SetColors(IClassificationFormatMap classificationFormatMap, SandyColorProvider sandyColorProvider)
        {
            var keywordProperties = classificationFormatMap.GetTextProperties(standardClassificationService.Keyword);
            var identifierProperties = classificationFormatMap.GetTextProperties(standardClassificationService.Identifier);
            var operatorProperties = classificationFormatMap.GetTextProperties(standardClassificationService.Operator);
            var numberProperties = classificationFormatMap.GetTextProperties(standardClassificationService.NumberLiteral);
            var stringProperties = classificationFormatMap.GetTextProperties(standardClassificationService.StringLiteral);
            var characterProperties = classificationFormatMap.GetTextProperties(standardClassificationService.CharacterLiteral);

            sandyColorProvider.KeywordBrush = keywordProperties.ForegroundBrush;
            sandyColorProvider.IdentifierBrush = identifierProperties.ForegroundBrush;
            sandyColorProvider.OperatorBrush = operatorProperties.ForegroundBrush;
            sandyColorProvider.NumberBrush = numberProperties.ForegroundBrush;
            sandyColorProvider.StringBrush = stringProperties.ForegroundBrush;
            sandyColorProvider.CharacterBrush = characterProperties.ForegroundBrush;
        }
    }
}