using AutoFixture;
using AutoFixture.Xunit2;

namespace WhatsAppConvertorTests.Common
{
    public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] objects) : base(new AutoMoqDataAttribute(), objects)
        {
        }

        public InlineAutoMoqDataAttribute(Type customizationType, params object[] objects) : base(new AutoMoqDataAttribute(customizationType), objects)
        {
        }

        public InlineAutoMoqDataAttribute(Type[] customizationTypes, params object[] objects) : base(new AutoMoqDataAttribute(customizationTypes), objects)
        {
        }

        public InlineAutoMoqDataAttribute(IFixture fixture, Type customizationType, params object[] objects) : base(new AutoMoqDataAttribute(fixture, customizationType), objects)
        {
        }

        public InlineAutoMoqDataAttribute(IFixture fixture, Type[] customizationTypes, params object[] objects) : base(new AutoMoqDataAttribute(fixture, customizationTypes), objects)
        {
        }                   
    }
}
