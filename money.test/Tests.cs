using System;
using money.web.Models.Entities;
using NUnit.Framework;

namespace money.test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void AsPersistentSession_Success()
        {
            var cookieString = "100|1234|5678|2018-08-15 08:05:32|2018-11-13 08:05:32";

            var session = cookieString.AsPersistentSession();

            Assert.AreEqual(session.UserID, 100);
            Assert.AreEqual(session.SeriesIdentifier, "1234");
            Assert.AreEqual(session.Token, "5678");
            Assert.AreEqual(session.Created, new DateTime(2018, 8, 15, 8, 5, 32));
            Assert.AreEqual(session.Expires, new DateTime(2018, 11, 13, 8, 5, 32));
        }

        [Test]
        public void AsPersistentSession_Failures()
        {
            Assert.Throws<ArgumentException>(() => default(string).AsPersistentSession());
            Assert.Throws<ArgumentException>(() => string.Empty.AsPersistentSession());
            Assert.Throws<ArgumentException>(() => "1234|5678".AsPersistentSession());
            Assert.Throws<ArgumentException>(() => "X|1234|5678|2018-08-15 08:05:32|2018-11-13 08:05:32".AsPersistentSession());
            Assert.Throws<ArgumentException>(() => "100|1234|5678|X|2018-11-13 08:05:32".AsPersistentSession());
            Assert.Throws<ArgumentException>(() => "100|1234|5678|2018-08-15 08:05:32|X".AsPersistentSession());
        }

        [Test]
        public void AsCookieString_Operation()
        {
            var session = new PersistentSession(
                userID: 123,
                seriesIdentifier: "ABCD",
                token: "EFGH",
                created: new DateTime(2018, 9, 1, 14, 42, 23),
                expires: new DateTime(2018, 11, 30, 14, 42, 23)
            );

            var expectedCookieString = "123|ABCD|EFGH|2018-09-01 14:42:23|2018-11-30 14:42:23";

            Assert.AreEqual(expectedCookieString, session.AsCookieString());
        }
    }
}
