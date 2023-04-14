using AutoFixture.Xunit2;

namespace WhatsAppConvertorTests.Common
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {

        public AutoMoqDataAttribute() : base(FixtureFactory.Create)
        {
        }
    }
}
