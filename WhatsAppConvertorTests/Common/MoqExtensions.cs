using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace WhatsAppConvertorTests.Common
{
    public static class MoqExtensions
    {
        public static Mock<T> AsMock<T>(this T implementation) where T : class => Mock.Get(implementation);

        public static void VerifyLogWithLogLevel<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times)
        {
            static bool state(object v, Type t) => true;
            Verify(logger, logLevel, times, state);
        }

        public static void VerifyLogWithLogLevel<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Func<Times> times)
        {
            VerifyLogWithLogLevel(logger, logLevel, times());
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times, string containsMessage)
        {
            bool state(object v, Type t) {
                if (v is string s) {
                    return s.Contains(containsMessage);
                }
                return false;
            }
            Verify(logger, logLevel, times, state);
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times, Regex containsRegex)
        {
            bool state(object v, Type t) {
                if (v is string s) {
                    return containsRegex.IsMatch(s);
                }
                return false;
            }
            Verify(logger, logLevel, times, state);
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Func<Times> times, string containsMessage)
        {
            VerifyLogWithLogLevelAndContainsMessage(logger, logLevel, times(), containsMessage);
        }

        public static void VerifyLogWithLogLevelAndContainsMessage<T>(this Mock<ILogger<T>> logger, LogLevel logLevel, Func<Times> times, Regex containsRegex)
        {
            VerifyLogWithLogLevelAndContainsMessage(logger, logLevel, times(), containsRegex);
        }

        private static void Verify<T>(Mock<ILogger<T>> logger, LogLevel logLevel, Times times, Func<object, Type, bool> messageCheck)
        {
            logger
                .Verify(
                    l => l.Log(
                        It.Is<LogLevel>(ll => ll == logLevel),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => messageCheck(v, t)),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                    times,
                    "Failed to log amount/message as expected"
                );
        }
    }
}
